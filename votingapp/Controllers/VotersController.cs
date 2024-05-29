using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votingapp.Infrastructure;
using votingapp.Models;

namespace votingapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VotersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voter>>> GetVoters()
        {
            var voters = await _context.voters.ToListAsync();
            return Ok(voters); // Ensure it returns OkObjectResult
        }

        [HttpPost]
        public async Task<ActionResult<Voter>> AddVoter(Voter voter)
        {
            _context.voters.Add(voter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVoters), new { id = voter.id }, voter);
        }
    }
}
