using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ApplicationDbContext _context;

        public VotersControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "VotingAppTest")
                .Options;

            _context = new ApplicationDbContext(options);

            // Clear the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed the in-memory database
            _context.voters.AddRange(
                new Voter { id = 1, name = "John Doe", has_voted = false },
                new Voter { id = 2, name = "Jane Doe", has_voted = true }
            );
            _context.SaveChanges();

            _controller = new VotersController(_context);
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
            var voter = new Voter { id = 3, name = "Alice", has_voted = false };

            // Act
            var result = await _controller.AddVoter(voter);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Voter>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnVoter = Assert.IsType<Voter>(createdAtActionResult.Value);

            Assert.Equal(voter.name, returnVoter.name);
            Assert.Equal(voter.has_voted, returnVoter.has_voted);
        }
    }
}
