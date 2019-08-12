using Lexiconner.Domain.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Persistence.Repositories.MongoDb
{
    public class IdentityDataRepository : MongoDataRepository, IIdentityDataRepository
    {
        public IdentityDataRepository(MongoClient client, string database, ApplicationDb applicationDb) : base(client, database, applicationDb)
        {
        }
    }
}
