using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depot.Server.Data
{
    public class MongoDBContext
    {
        private MongoClient _client { get; set; }


        /// <summary>
        /// Options from Startup, used to setup db connection
        /// </summary>
        /// <param name="settings"></param>
        public MongoDBContext(IConfiguration configuration)
        {
            //Connect to the database
            _client = new MongoClient(configuration.GetConnectionString("MongoDBConnectionString"));

            Setup(configuration["MongoDBDatabaseName"]);
        }

        private void Setup(string database)
        {
            ConventionPack pack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("IgnoreExtraElements", pack, t => true);

            IMongoDatabase Database = _client.GetDatabase(database);

            Instances = Database.GetCollection<Dictionary<string, string>>("instances");

            //Use this on the cmd console tools:
            //db.instances.createIndex( { FolderId: 1, TypeId: 1, "$**": "text" } )

            /*
                Use this at the atlas create index page
                {
                  "FolderId":"1",
                  "TypeId":"1",
                  "$**":"text"
                }

             */
        }

        public void EnsureIndexExists()
        {
            string indexName = "instance_search";

            //I hate this loop. TODO: Find a better way to do this
            var indexes = this.Instances.Indexes.List().ToList();
            bool indexExists = false;
            foreach(var i in indexes)
            {
                if (i.ToString().Contains(indexName) == true)
                {
                    indexExists = true;
                }
            }

            if (indexExists == false)
            {
                IndexKeysDefinition<Dictionary<string, string>> keys = "{ FolderId:1, TypeId:1, '$**':'text' }";
                var indexModel = new CreateIndexModel<Dictionary<string, string>>(keys, new CreateIndexOptions() { Name = indexName });
                this.Instances.Indexes.CreateOne(indexModel);
            }
        }

        public IMongoCollection<Dictionary<string, string>> Instances { get; set; }
    }
}
