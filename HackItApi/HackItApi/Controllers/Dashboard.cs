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
            
            var timeSeries = obj["Time Series (1min)"];

            var openList = new List<float>();
            foreach (var point in timeSeries)
            {
                openList.Add(float.Parse(point["1. open"]).ToString());
            }
            
            return Ok(new
            {
                openList,
                
            });
        }
        
    }
}