using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FunctionAppImagensNASA.HttpClients;
using FunctionAppImagensNASA.Data;

namespace FunctionAppImagensNASA
{
    public class CargaImagemQueueTrigger
    {
        private readonly IImagemDiariaAPI _apiImagemDiaria;
        private readonly NASAContext _context;

        public CargaImagemQueueTrigger(IImagemDiariaAPI apiImagemDiaria,
            NASAContext context)
        {
            _apiImagemDiaria = apiImagemDiaria;
            _context = context;
        }

        [Function("CargaImagemQueueTrigger")]
        public void Run([QueueTrigger("queue-imagemnasa", Connection = "AzureWebJobsStorage")] string myQueueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger("CargaImagemQueueTrigger");

            if (!Regex.IsMatch(myQueueItem, @"^\d{4}-\d{2}-\d{2}$") ||
                !DateTime.TryParse(myQueueItem, out var dataConvertida) ||
                dataConvertida > DateTime.Now || dataConvertida.Year < 2000)
            {
                logger.LogError(
                    $"A data da imagem ({myQueueItem}) deve estar no formato aaaa-mm-dd, " +
                    "ser menor ou igual à data atual e a partir do ano 2000!");
                return;
            }

            var infoImagem = _apiImagemDiaria.GetInfoAsync(
                Environment.GetEnvironmentVariable("APIKeyNASA"),
                myQueueItem).Result;
            var urlImagem = infoImagem.Url;
            var nomeArquivoImagem = Path.GetFileName(urlImagem);
            if (infoImagem.Media_type == "image")
            {
                var storageAccount = CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("BlobContainerImagensNASA"));
                container.CreateIfNotExists();

                var dataAtual = DateTime.Now;
                var blockBlob = container.GetBlockBlobReference(
                    $"{dataAtual:yyyyMMdd_HHmmss}-{infoImagem.Date}-{nomeArquivoImagem}");

                using var client = new HttpClient();
                using var stream = client.GetStreamAsync(urlImagem).Result;
                blockBlob.UploadFromStream(stream);

                _context.CargasImagemNASA.Add(new ()
                {
                    DataImagem = dataConvertida,
                    DataCarga = dataAtual,
                    Titulo = infoImagem.Title,
                    Detalhes = infoImagem.Explanation,
                    UrlImagem = urlImagem,
                    BlobName = blockBlob.Name,
                    BlobContainer = container.Name,
                    Copyright = infoImagem.Copyright,
                    MediaType = infoImagem.Media_type,
                    ServiceVersion = infoImagem.Service_version
                });
                _context.SaveChanges();
                
                logger.LogInformation(
                    $"O arquivo {nomeArquivoImagem} " +
                    $"foi carregado no Blob Container {container.Name} com o nome {blockBlob.Name}");
            }
            else
            {
                logger.LogError(
                    $"O arquivo {nomeArquivoImagem} " +
                    "não será carregado pois não corresponde a uma imagem");
           }
        }
    }
}