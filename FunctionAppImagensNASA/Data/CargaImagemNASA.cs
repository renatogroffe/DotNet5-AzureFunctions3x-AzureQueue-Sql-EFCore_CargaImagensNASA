using System;

namespace FunctionAppImagensNASA.Data
{
    public class CargaImagemNASA
    {
        public int Id { get; set; }
        public DateTime DataImagem { get; set; }
        public DateTime DataCarga { get; set; }
        public string Titulo { get; set; }
        public string Detalhes { get; set; }
        public string UrlImagem { get; set; }
        public string BlobName { get; set; }
        public string BlobContainer { get; set; }
        public string Copyright { get; set; }
        public string MediaType { get; set; }
        public string ServiceVersion { get; set; }                
    }
}