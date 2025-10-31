using Infrastructure.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly EFContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(EFContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("agents")]
        public async Task<IActionResult> GetAgents()
        {
            var agents = await _context.Agents.ToListAsync();
            return Ok(agents);
        }

        [HttpGet("teams")]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams.Include(t => t.Agents).ToListAsync();
            return Ok(teams);
        }
    }
}
