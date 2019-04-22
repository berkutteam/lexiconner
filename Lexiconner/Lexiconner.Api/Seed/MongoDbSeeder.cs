using Lexiconner.Api.ImportAndExport;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Seed
{
    public class MongoDbSeeder : ISeeder
    {
        private readonly IConfiguration _configuration;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IStudyItemRepository _studyItemRepository;

        public MongoDbSeeder(IConfiguration configuration, IWordTxtImporter wordTxtImporter, IStudyItemRepository studyItemRepository)
        {
            _configuration = configuration;
            _wordTxtImporter = wordTxtImporter;
            _studyItemRepository = studyItemRepository;
        }

        public async Task Seed()
        {
            if (!(await _studyItemRepository.GetAll()).Any())
            {
                Console.WriteLine($"Seeding db...");
                var words = await _wordTxtImporter.Import();
                var entities = words.Select(x => new StudyItem
                {
                    Title = x.Word,
                    Description = x.Description,
                    ExampleText = x.ExampleText,
                    Tags = x.Tags,
                });
                await _studyItemRepository.AddAll(entities);
                Console.WriteLine($"Seed finished.");
            }
            else
            {
                Console.WriteLine($"Db already contains data. Do not seed.");
            }
        }
    }
}
