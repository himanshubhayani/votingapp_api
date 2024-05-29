using Microsoft.AspNetCore.Mvc;
using votingapp.Infrastructure;

namespace votingapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VotingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Vote")]
        public IActionResult Vote(int voterId, int candidateId)
        {
            var voter = _context.voters.FirstOrDefault(v => v.id == voterId);
            var candidate = _context.candidates.FirstOrDefault(c => c.id == candidateId);

            if (voter != null && candidate != null && !voter.has_voted)
            {
                candidate.votes++;
                voter.has_voted = true;

                _context.SaveChanges();

                return Ok();
            }

            return BadRequest("Vote failed. Invalid voter or candidate.");
        }
    }
}
