using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.Repositories
{
    public interface IStudyItemRepository : IRepository<StudyItem>
    {
        Task<IEnumerable<StudyItem>> GetAll(int offset = 0, int limit = 10, string search = "");
    }
}
