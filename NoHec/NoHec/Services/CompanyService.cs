using NoHec.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoHec.Services
{
    public interface ICompanyService
    {
        Task<CompanyData> GetCompanyData(string symbol);
    }

    public class CompanyService : ICompanyService
    {
        private IHttpService _httpService;

        public CompanyService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<CompanyData> GetCompanyData(string symbol)
        {
            return await _httpService.Get<CompanyData>($"api/dashboard/stockDetails?symbol={symbol}");
        }
    }
}
