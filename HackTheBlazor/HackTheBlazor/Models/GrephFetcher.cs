using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HackTheBlazor.Services;

namespace HackTheBlazor.Models
{
	public class GraphFetcher : INotifyPropertyChanged
	{
		
		public List<float> data;
		public string strData = "";

		public event PropertyChangedEventHandler PropertyChanged;


		private async Task loadSymbolAsyncron(string symbol)
		{
			HttpClient _client = new HttpClient();

			using (var response = await _client.GetAsync
			($"https://www.alphavantage.co/query?" +
			$"function=TIME_SERIES_INTRADAY&symbol=SNE&interval=5min" +
			$"&apikey=OLC6HWQVYQUI449V"))
			{
				data = new List<float> { };
				resetData();

				string JsonList = await response.Content.ReadAsStringAsync();
				dynamic myObject = JsonConvert.DeserializeObject<dynamic>(JsonList);
				data = new List<float> { };

				foreach (var i in myObject["Time Series (5min)"])
				{
					data.Add(float.Parse(i.Value["4. close"].ToString()));
				}
				resetData();

			}

			//"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=SNE&interval=5min&apikey=OLC6HWQVYQUI449V"

			resetData();
		}

		public async void loadSymbol(string symbol)
		{
			//string str = new string(await globalService.LoadSymbolGraph(symbol));

			//data = new List<float> { };
			//resetData();
		}


		public void init()
		{

			data = new List<float> { 10, 20, 30, 40, 20, 10, 10, 50 };

			resetData();
		}

		public void resetData()
		{
			strData = string.Join(",", data);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(strData)));
		}


	}
}
