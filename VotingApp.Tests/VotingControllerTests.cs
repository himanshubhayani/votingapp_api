using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq.Expressions;
using votingapp.Controllers;
using votingapp.Infrastructure;
using votingapp.Models;
using Xunit;

namespace VotingApp.Tests
{
    public class VotingControllerTests
    {
        private readonly VotingController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public VotingControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _controller = new VotingController(_mockUnitOfWork.Object);
        }

        [Fact]
        public void Vote_SuccessfullyCastsVote()
        {
            // Arrange
            var voter = new Voter { id = 1, has_voted = false };
            var candidate = new Candidate { id = 1, votes = 0 };

            _mockUnitOfWork.Setup(uow => uow.VotersRepository.Find(It.IsAny<Expression<Func<Voter, bool>>>()))
                .Returns(voter);

            _mockUnitOfWork.Setup(uow => uow.CandidatesRepository.Find(It.IsAny<Expression<Func<Candidate, bool>>>()))
                .Returns(candidate);

            _mockUnitOfWork.Setup(uow => uow.VotersRepository.Update(It.IsAny<Voter>(), It.IsAny<int>()))
                .Callback<Voter, int>((v, id) => voter.has_voted = v.has_voted);

            _mockUnitOfWork.Setup(uow => uow.CandidatesRepository.Update(It.IsAny<Candidate>(), It.IsAny<int>()))
                .Callback<Candidate, int>((c, id) => candidate.votes = c.votes);

            // Act
            var result = _controller.Vote(voter.id, candidate.id);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.True(voter.has_voted);
            Assert.Equal(1, candidate.votes);
        }

        [Fact]
        public void Vote_ReturnsBadRequestForInvalidVoterOrCandidate()
        {
            // Arrange
            _mockUnitOfWork.Setup(uow => uow.VotersRepository.Find(It.IsAny<Expression<Func<Voter, bool>>>()))
                .Returns((Voter)null);

            _mockUnitOfWork.Setup(uow => uow.CandidatesRepository.Find(It.IsAny<Expression<Func<Candidate, bool>>>()))
                .Returns((Candidate)null);

            // Act
            var result = _controller.Vote(1, 1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Vote failed. Invalid voter or candidate.", badRequestResult.Value);
        }
    }
}
