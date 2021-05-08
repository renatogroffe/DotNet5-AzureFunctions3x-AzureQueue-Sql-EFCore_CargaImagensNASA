using System.Threading.Tasks;
using Refit;
using FunctionAppImagensNASA.Models;

namespace FunctionAppImagensNASA.HttpClients
{
    public interface IImagemDiariaAPI
    {
        [Get("/apod")]
        Task<InfoImagemNASA> GetInfoAsync(string api_key, string date);         
    }
}