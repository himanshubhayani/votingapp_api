using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using votingapp.Controllers;
using votingapp.Infrastructure;
using votingapp.Models;
using Xunit;

namespace VotingApp.Tests
{
    public class CandidatesControllerTests
    {
        private readonly CandidatesController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public CandidatesControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Mock the repository methods
            _mockUnitOfWork.Setup(uow => uow.CandidatesRepository.GetAllAsyncList())
                .ReturnsAsync(new List<Candidate>
                {
                    new Candidate { id = 1, name = "Alice", votes = 0 },
                    new Candidate { id = 2, name = "Bob", votes = 0 }
                });

            _mockUnitOfWork.Setup(uow => uow.CandidatesRepository.AddAsync(It.IsAny<Candidate>()))
                .Returns<Candidate>(candidate =>
                {
                    candidate.id = 3; // Simulate setting the id after adding
                    return Task.FromResult(candidate);
                });

            _controller = new CandidatesController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetCandidates_ReturnsAllCandidates()
        {
            // Act
            var result = await _controller.GetCandidates();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Candidate>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnCandidates = Assert.IsType<List<Candidate>>(okResult.Value);

            Assert.Equal(2, returnCandidates.Count);
        }

        [Fact]
        public async Task AddCandidate_ReturnsCreatedCandidate()
        {
            // Arrange
            var candidate = new Candidate { name = "Charlie", votes = 0 };

            // Act
            var result = await _controller.AddCandidate(candidate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Candidate>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnCandidate = Assert.IsType<Candidate>(createdAtActionResult.Value);

            Assert.Equal(candidate.name, returnCandidate.name);
            Assert.Equal(candidate.votes, returnCandidate.votes);

            // Check if ID was assigned
            Assert.NotEqual(0, returnCandidate.id);
        }
    }
}
