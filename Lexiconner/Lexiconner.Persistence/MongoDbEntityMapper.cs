using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;

namespace Lexiconner.Persistence
{
    public class MongoDbEntityMapper
    {
        private static bool _isConfigured = false;
        protected MongoDbEntityMapper()
        {

        }

        public static void ConfigureMapping()
        {
            if (_isConfigured)
            {
                return;
            }

            //register [BsonIgnoreExtraElements] for all entities
            var pack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true)

            };
            ConventionRegistry.Register("MongoDbEntity", pack, t => true);

            //[BsonRepresentation(BsonType.String)] for all DateTimeOffset properties
            BsonSerializer.RegisterSerializer(typeof(DateTimeOffset), new DateTimeOffsetSerializer(BsonType.String));

            _isConfigured = true;
        }
    }
}
