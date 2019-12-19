using Lexiconner.Domain.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Persistence.Repositories.MongoDb
{
    public class SharedCacheDataRepository : MongoDataRepository, ISharedCacheDataRepository
    {
        public SharedCacheDataRepository(MongoClient client, string database, ApplicationDb applicationDb) : base(client, database, applicationDb)
        {
        }
    }
}
