using HackTheBlazor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HackTheBlazor.Services
{
	public interface IGlobalService
	{
		Task<IEnumerable<User>> GetAll();

		Task<dynamic> LoadSymbolGraph(string symbol);
	}

	public class GlobalService : IGlobalService
	{
		private IHttpService _httpService;

		public GlobalService(IHttpService httpService)
		{
			_httpService = httpService;
		}

		public async Task<IEnumerable<User>> GetAll()
		{
			return await _httpService.Get<IEnumerable<User>>("/api/dashboard");
		}

		public async Task<dynamic> LoadSymbolGraph(string symbol)
		{
			//todo 

			return await _httpService.Get<dynamic>(
				"$https://www.alphavantage.co/query?" +
				$"function=TIME_SERIES_INTRADAY&symbol=SNE&interval=5min" +
				$"&apikey=OLC6HWQVYQUI449V"
				);

		}
	}
}