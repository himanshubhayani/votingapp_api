using Microsoft.AspNetCore.Mvc;
using votingapp.Infrastructure;
using votingapp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace votingapp.Controllers
{
    /// <summary>
    /// Controller for handling voter operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VotersController : ControllerBase
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VotersController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public VotersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves all voters.
        /// </summary>
        /// <returns>A list of voters.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voter>>> GetVoters()
        {
            var voters = await _unitOfWork.VotersRepository.GetAllAsyncList();
            return Ok(voters); // Ensure it returns OkObjectResult
        }

        /// <summary>
        /// Adds a new voter.
        /// </summary>
        /// <param name="voter">The voter to add.</param>
        /// <returns>The created voter.</returns>
        [HttpPost]
        public async Task<ActionResult<Voter>> AddVoter(Voter voter)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return a BadRequest response with the validation errors
                return BadRequest(ModelState);
            }

            // Add the voter if validation passes
            await _unitOfWork.VotersRepository.AddAsync(voter);
            return CreatedAtAction(nameof(GetVoters), new { id = voter.id }, voter);
        }

        #endregion
    }
}
