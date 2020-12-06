using NoHec.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoHec.Services
{
	public interface IGlobalService
	{
		Task Register(string email, string password, string firstName, string lastName);
		Task<CompDetails> GetCompanyData(string symbol);
		Task<float> GetCompanyLastPrice(string symbol);
		Task BuyStonk(string symbol);
		Task SellStonk(string id);
		Task<List<FavStonks>> GetFavStonks();
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

		public async Task<CompDetails> GetCompanyData(string symbol)
		{
			return await _httpService.Get<CompDetails>($"api/dashboard/getGraphData?symbol={symbol}");
		}

		public async Task<float> GetCompanyLastPrice(string symbol)
		{
			return await _httpService.Get<float>($"/api/dashboard/getLatestPrice?symbol={symbol}");
		}

		public async Task BuyStonk(string symbol)
        {
			await _httpService.Get<dynamic>($"/api/dashboard/buyStonk?symbol={symbol}");
        }

		public async Task SellStonk(string id)
		{
			await _httpService.Get<dynamic>($"/api/dashboard/sellStonk?id={id}");
		}

		public async Task<List<FavStonks>> GetFavStonks()
        {
			return await _httpService.Get<List<FavStonks>>($"/api/dashboard/favStonks");
        }

	}


}
