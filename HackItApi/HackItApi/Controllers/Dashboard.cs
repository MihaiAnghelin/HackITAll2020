using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
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
             $"function=TIME_SERIES_WEEKLY&symbol={symbol}&interval=1min" +
             "&apikey=EHDV2NLKZPWWZTZG");
            
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { message = "Api call is null" });
            
            var json = await response.Content.ReadAsStringAsync();
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            
            if(obj is null)
                return BadRequest(new { message = "Api call is null" });

            #region Map Lists
            
            var closeList = new List<float>();
            foreach (var point in obj["Weekly Time Series"])
            {
                var a = point.Value["4. close"];
                closeList.Add(Convert.ToSingle(a));
            }
            #endregion

            closeList = closeList.Take(20).ToList();
            closeList.Reverse();
            
            // var random = new Random();
            // var closeList = new List<float>();
            // for (int i = 0; i < 20; i++)
            // {
            //     closeList.Add(random.Next(90, 100));
            // }
                
                
            var response2 = await _client.GetAsync(
                "https://www.alphavantage.co/query?" +
                $"function=OVERVIEW&symbol={symbol}&apikey=OLC6HWQVYQUI449V"
            );
            
            if (!response2.IsSuccessStatusCode)
                return BadRequest(new { message = "Api call is null" });
            
            var json2 = await response2.Content.ReadAsStreamAsync();
            
            var obj2 = await JsonSerializer.DeserializeAsync<CompanyData>(json2);
            
            if(obj2 is null)
                return BadRequest(new { message = "Api call is null" });
            
            
            // var obj2 = new CompanyData
            // {
            //     Symbol = "SNE",
            //     Name = "Sony Corporation",
            //     Description = "Sony Corporation designs, develops, produces, and sells electronic equipment, instruments, and devices for the consumer, professional, and industrial markets worldwide. The company distributes software titles and add-on content through digital networks by Sony Interactive Entertainment; network services related to game, video, and music content; and home and portable game consoles, packaged software, and peripheral devices. It also develops, produces, markets, and distributes recorded music; publishes music; and produces and distributes animation titles, game applications based on animation titles, and various services for music and visual products. In addition, the company offers live-action and animated motion pictures, as well as scripted and unscripted series, daytime serials, game shows, animated series, television movies, and miniseries and other television programs; operates a visual effects and animation unit; manages a studio facility; and operates television and digital networks. Further, it researches, develops, designs, produces, markets, distributes, sells, and services video and sound products; interchangeable lens, compact digital, and consumer and professional video cameras; display products, such as projectors and medical equipment; mobile phones, tablets, accessories, and applications; and metal oxide semiconductor image sensors, charge-coupled devices, large-scale integration systems, and other semiconductors. Additionally, it offers Internet broadband network services; creates and distributes content for various electronics product platforms, such as PCs and mobile phones; and provides life and non-life insurance, banking, and other services, as well as batteries, recording media, and storage media products. It has collaboration with The UNOPS. The company was formerly known as Tokyo Tsushin Kogyo Kabushiki Kaisha and changed its name to Sony Corporation in January 1958. The company was founded in 1946 and is headquartered in Tokyo, Japan.",
            //     AssetType = "Common Stock",
            //     Exchange = "NYSE",
            //     Currency = "USD",
            //     Country = "USA",
            //     Sector = "Technology",
            //     Industry = "Consumer Electronics"
            // };
            
            return Ok(new
            {
                CompanyData = obj2,
                TableData = closeList
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
                a = point.Value["4. close"];
                break;
            }
            
            var b = Convert.ToSingle(a);

            // var b = 94.4;
            
            return Ok(b);
        }

        [HttpPost("buyStonk")]
        public async Task<IActionResult> BuyStonk(string symbol)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var newStonk = new FavStonks
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Symbol = symbol
            };

            await _context.FavStonks.AddAsync(newStonk);
            await _context.SaveChangesAsync();
            
            return Ok(new
            {
                message = "Stonk Added Stuccessfully"
            });
        }

        [HttpPost("sellStonk")]
        public async Task<IActionResult> SellStonk(string id)
        {
            var stonk = _context.FavStonks.FirstOrDefault(c => c.Id == id);
            if (stonk == null)
                return BadRequest(new
                {
                    message = "Stonk is null!"
                });
            
            _context.FavStonks.Remove(stonk);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Sold stock"
            });
        }

        [HttpGet("favStonks")]
        public async Task<IActionResult> GetFavStonks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var stonks = _context.FavStonks.Where(c => c.UserId == userId).Select(c => new
            {
                c.Id,
                c.Symbol
            });

            return Ok(stonks);
        }
        
    }
}