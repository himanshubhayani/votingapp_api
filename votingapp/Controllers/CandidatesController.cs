using Microsoft.AspNetCore.Mvc;
using votingapp.Infrastructure;
using votingapp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace votingapp.Controllers
{
    /// <summary>
    /// Controller for handling candidate operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidatesController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public CandidatesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves all candidates.
        /// </summary>
        /// <returns>A list of candidates.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            var candidates = await _unitOfWork.CandidatesRepository.GetAllAsyncList();
            return Ok(candidates); // Ensure it returns OkObjectResult
        }

        /// <summary>
        /// Adds a new candidate.
        /// </summary>
        /// <param name="candidate">The candidate to add.</param>
        /// <returns>The created candidate.</returns>
        [HttpPost]
        public async Task<ActionResult<Candidate>> AddCandidate(Candidate candidate)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return a BadRequest response with the validation errors
                return BadRequest(ModelState);
            }

            // Add the candidate if validation passes
            await _unitOfWork.CandidatesRepository.AddAsync(candidate);
            return CreatedAtAction(nameof(GetCandidates), new { id = candidate.id }, candidate);
        }

        #endregion
    }
}
