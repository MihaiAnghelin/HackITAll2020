using System.Linq;
using HackItApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackItApi.Controllers
{
    [ApiController, Route("api/dashboard")]
    public class Dashboard : Controller
    {
        private readonly HackContext _context;

        public Dashboard(HackContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.Items.ToList();
            
            return Ok(items);
        }
    }
}