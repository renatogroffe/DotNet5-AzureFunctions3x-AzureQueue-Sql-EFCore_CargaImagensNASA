using System.Linq;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using FunctionAppImagensNASA.Data;

namespace FunctionAppImagensNASA
{
    public class Imagens
    {
        private readonly NASAContext _context;

        public Imagens(NASAContext context)
        {
            _context = context;
        }

        [Function("Imagens")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Imagens");

            
            var dados = _context.CargasImagemNASA
                .OrderByDescending(c => c.Id).ToList();
            logger.LogInformation($"Número de cargas realizadas: {dados.Count}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(dados).AsTask().Wait();
            return response;
        }
    }
}