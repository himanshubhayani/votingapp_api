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
    public class CandidatesControllerTests
    {
        private readonly CandidatesController _controller;
        private readonly ApplicationDbContext _context;

        public CandidatesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "VotingAppTest")
                .Options;

            _context = new ApplicationDbContext(options);

            // Clear the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed the in-memory database
            _context.candidates.AddRange(
                new Candidate { id = 1, name = "Alice", votes = 0 },
                new Candidate { id = 2, name = "Bob", votes = 0 }
            );
            _context.SaveChanges();

            _controller = new CandidatesController(_context);
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
            var candidate = new Candidate { name = "Charlie", votes = 0 }; // Do not set id

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
