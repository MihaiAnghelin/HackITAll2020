using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HackItApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HackItApi.Controllers
{
    [ApiController, Route("api/dashboard"), Authorize(Roles = Role.User)]
    public class Dashboard : Controller
    {
        private readonly HackContext _context;
        private static HttpClient _client = new HttpClient();

        public Dashboard(HackContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.Users.ToList();
            
            return Ok(items);
        }

        [HttpGet("stockDetails")]
        public async Task<IActionResult> StockDetails(string symbol)
        {
            var response = await _client.GetAsync(
                "https://www.alphavantage.co/query?" +
                $"function=OVERVIEW&symbol={symbol}&apikey=OLC6HWQVYQUI449V"
            );

            if (!response.IsSuccessStatusCode)
                return BadRequest(new { message = "Api call is null" });

            var json = await response.Content.ReadAsStreamAsync();

            var obj = await JsonSerializer.DeserializeAsync<CompanyData>(json);
            
            if(obj is null)
                return BadRequest(new { message = "Api call is null" });
            
            return Ok(obj);
        }

        [HttpGet("getGraphData")]
        public async Task<IActionResult> GetGraphData1Min(string symbol)
        {
            var response = await _client.GetAsync
            ("https://www.alphavantage.co/query?" +
             $"function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min" +
             "&apikey=OLC6HWQVYQUI449V");
            
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { message = "Api call is null" });
            
            var json = await response.Content.ReadAsStringAsync();
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            if(obj is null)
                return BadRequest(new { message = "Api call is null" });

            #region Map Lists

            var openList = new List<float>();
            foreach (var point in obj["Time Series (1min)"])
            {
                var a = point.Value["1. open"].ToString();
                openList.Add(float.Parse(a));
            }
            
            var highList = new List<float>();
            foreach (var point in obj["Time Series (1min)"])
            {
                var a = point.Value["2. high"].ToString();
                highList.Add(float.Parse(a));
            }
            
            var lowList = new List<float>();
            foreach (var point in obj["Time Series (1min)"])
            {
                var a = point.Value["3. low"].ToString();
                lowList.Add(float.Parse(a));
            }
            
            var closeList = new List<float>();
            foreach (var point in obj["Time Series (1min)"])
            {
                var a = point.Value["4. close"].ToString();
                closeList.Add(float.Parse(a));
            }
            
            var volumeList = new List<float>();
            foreach (var point in obj["Time Series (1min)"])
            {
                var a = point.Value["5. volume"].ToString();
                volumeList.Add(float.Parse(a));
            }

            #endregion
            
            return Ok(new
            {
                openList,
                highList,
                lowList,
                closeList,
                volumeList
            });
        }

        [HttpGet("getLatestPrice")]
        public async Task<IActionResult> GetLastestPrice(string symbol)
        {
            var response = await _client.GetAsync
            ("https://www.alphavantage.co/query?" +
             $"function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min" +
             "&apikey=OLC6HWQVYQUI449V");
            
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { message = "Api call is null" });

            var json = await response.Content.ReadAsStringAsync();
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            
            if(obj is null)
                return BadRequest(new { message = "Api call is null" });

            dynamic a = null;
            foreach (var point in obj["Time Series (1min)"])
            {
                a = point.Value["4. close"].ToString();
                break;
            }

            var b = float.Parse(a);

            return Ok(b);
        }
        
        
        
    }
}