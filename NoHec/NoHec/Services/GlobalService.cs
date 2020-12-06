using NoHec.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoHec.Services
{
    public interface IGlobalService
    {
        Task Register(string email, string password, string firstName, string lastName);
        Task<List<double>> GetChardData(string symbol);

    }
    public class GlobalService : IGlobalService
    {
        private IHttpService _httpService;

        public GlobalService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task Register(string email, string password, string firstName, string lastName)
        {
            await _httpService.Post<RegisterModel>("/api/auth/register", new { email, password, firstName, lastName });
        }

        public async Task<List<double>> GetChardData(string symbol)
        {
            return await _httpService.Get<List<double>>($"api/dashboard/getGraphData?symbol={symbol}");
        }
    }


}
