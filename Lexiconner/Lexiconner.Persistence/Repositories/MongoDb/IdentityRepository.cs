using Lexiconner.Persistence.Repositories.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Persistence.Repositories.MongoDb
{
    public class IdentityRepository : MongoRepository, IIdentityRepository
    {
        public IdentityRepository(MongoClient client, string database) : base(client, database)
        {
        }
    }
}
