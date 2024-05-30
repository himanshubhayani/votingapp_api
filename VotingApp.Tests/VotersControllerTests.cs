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
    public class VotersControllerTests
    {
        private readonly VotersController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public VotersControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Mock the repository methods
            _mockUnitOfWork.Setup(uow => uow.VotersRepository.GetAllAsyncList())
                .ReturnsAsync(new List<Voter>
                {
                    new Voter { id = 1, name = "Alice"},
                    new Voter { id = 2, name = "Bob"}
                });

            _mockUnitOfWork.Setup(uow => uow.VotersRepository.AddAsync(It.IsAny<Voter>()))
                .Returns<Voter>(voter =>
                {
                    voter.id = 3; // Simulate setting the id after adding
                    return Task.FromResult(voter);
                });

            _controller = new VotersController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetVoters_ReturnsAllVoters()
        {
            // Act
            var result = await _controller.GetVoters();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Voter>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnVoters = Assert.IsType<List<Voter>>(okResult.Value);

            Assert.Equal(2, returnVoters.Count);
        }

        [Fact]
        public async Task AddVoter_ReturnsCreatedVoter()
        {
            // Arrange
            var voter = new Voter { name = "Charlie" };

            // Act
            var result = await _controller.AddVoter(voter);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Voter>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnVoter = Assert.IsType<Voter>(createdAtActionResult.Value);

            Assert.Equal(voter.name, returnVoter.name);

            // Check if ID was assigned
            Assert.NotEqual(0, returnVoter.id);
        }
    }
}
