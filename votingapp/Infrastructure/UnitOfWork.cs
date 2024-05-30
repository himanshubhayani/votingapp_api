
using Microsoft.EntityFrameworkCore.Storage;
using votingapp.Infrastructure;
using votingapp.Models;

namespace votingapp.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext DbContext;
        private IDbContextTransaction _transaction;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnitOfWork(ApplicationDbContext _dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            DbContext = _dbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public IRepository<Voter> VotersRepository => new Repository<Voter>(DbContext, _configuration);
        public IRepository<Candidate> CandidatesRepository => new Repository<Candidate>(DbContext, _configuration);
        
    }
}