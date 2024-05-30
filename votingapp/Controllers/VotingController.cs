using Microsoft.AspNetCore.Mvc;
using votingapp.Infrastructure;

namespace votingapp.Controllers
{
    /// <summary>
    /// Controller for handling voting operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VotingController : ControllerBase
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VotingController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public VotingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Casts a vote for a candidate by a voter.
        /// </summary>
        /// <param name="voterId">The ID of the voter.</param>
        /// <param name="candidateId">The ID of the candidate.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the vote.</returns>
        [HttpPost("Vote")]
        public IActionResult Vote(int voterId, int candidateId)
        {
            // Retrieve voter and candidate from the repositories
            var voter = _unitOfWork.VotersRepository.Find(v => v.id == voterId);
            var candidate = _unitOfWork.CandidatesRepository.Find(c => c.id == candidateId);

            // Check if both voter and candidate exist and the voter has not already voted
            if (voter != null && candidate != null && !voter.has_voted)
            {
                // Increment the candidate's vote count and update the repository
                candidate.votes++;
                _unitOfWork.CandidatesRepository.Update(candidate, candidate.id);

                // Set the voter's has_voted flag to true and update the repository
                voter.has_voted = true;
                _unitOfWork.VotersRepository.Update(voter, voter.id);

                // Return an OK response
                return Ok();
            }

            // Return a BadRequest response if the vote could not be cast
            return BadRequest("Vote failed. Invalid voter or candidate.");
        }

        #endregion
    }
}
