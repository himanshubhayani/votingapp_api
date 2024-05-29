using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votingapp.Infrastructure;
using votingapp.Models;

namespace votingapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            var candidates = await _context.candidates.ToListAsync();
            return Ok(candidates); // Ensure it returns OkObjectResult
        }

        [HttpPost]
        public async Task<ActionResult<Candidate>> AddCandidate(Candidate candidate)
        {
            _context.candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCandidates), new { id = candidate.id }, candidate);
        }
    }

}
