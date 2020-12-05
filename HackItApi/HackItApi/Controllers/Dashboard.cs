using System.Linq;
using HackItApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackItApi.Controllers
{
    [ApiController, Route("api/dashboard"), Authorize(Roles = Role.User)]
    public class Dashboard : Controller
    {
        private readonly HackContext _context;

        public Dashboard(HackContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.Users.ToList();
            
            return Ok(items);
        }
    }
}