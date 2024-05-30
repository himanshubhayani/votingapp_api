using votingapp.Models;

namespace votingapp.Infrastructure
{


    public interface IUnitOfWork  
    {
        IRepository<Voter> VotersRepository { get; }
        IRepository<Candidate> CandidatesRepository { get; }

    }
}