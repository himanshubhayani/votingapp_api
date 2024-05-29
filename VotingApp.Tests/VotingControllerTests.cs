using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using votingapp.Controllers;
using votingapp.Infrastructure;
using votingapp.Models;
using Xunit;

namespace VotingApp.Tests
{
    public class VotingControllerTests
    {
        private readonly VotingController _controller;
        private readonly ApplicationDbContext _context;

        public VotingControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "VotingAppTest")
                .Options;

            _context = new ApplicationDbContext(options);
            ClearDatabase();

            // Seed the in-memory database
            SeedDatabase();

            _controller = new VotingController(_context);
        }

        private void ClearDatabase()
        {
            _context.voters.RemoveRange(_context.voters);
            _context.candidates.RemoveRange(_context.candidates);
            _context.SaveChanges();
        }

        private void SeedDatabase()
        {
            _context.voters.AddRange(
                new Voter { id = 1, name = "John Doe", has_voted = false },
                new Voter { id = 2, name = "Jane Doe", has_voted = false }
            );

            _context.candidates.AddRange(
                new Candidate { id = 1, name = "Alice", votes = 0 },
                new Candidate { id = 2, name = "Bob", votes = 0 }
            );

            _context.SaveChanges();
        }

        [Fact]
        public void Vote_ValidVoterAndCandidate_ReturnsOk()
        {
            // Act
            var result = _controller.Vote(1, 1);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            var voter = _context.voters.First(v => v.id == 1);
            var candidate = _context.candidates.First(c => c.id == 1);

            Assert.True(voter.has_voted);
            Assert.Equal(1, candidate.votes);
        }

        [Fact]
        public void Vote_AlreadyVoted_ReturnsBadRequest()
        {
            // Arrange
            var voter = _context.voters.First(v => v.id == 1);
            voter.has_voted = true;
            _context.SaveChanges();

            // Act
            var result = _controller.Vote(1, 1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vote failed. Invalid voter or candidate.", badRequestResult.Value);
        }

        [Fact]
        public void Vote_InvalidVoter_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Vote(999, 1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vote failed. Invalid voter or candidate.", badRequestResult.Value);
        }

        [Fact]
        public void Vote_InvalidCandidate_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Vote(1, 999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vote failed. Invalid voter or candidate.", badRequestResult.Value);
        }
    }
}
