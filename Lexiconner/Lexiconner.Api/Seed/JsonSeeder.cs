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
    public class JsonSeeder : ISeeder
    {
        private readonly IConfiguration _configuration;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IStudyItemJsonRepository _studyItemJsonRepository;

        public JsonSeeder(IConfiguration configuration, IWordTxtImporter wordTxtImporter, IStudyItemJsonRepository studyItemJsonRepository)
        {
            _configuration = configuration;
            _wordTxtImporter = wordTxtImporter;
            _studyItemJsonRepository = studyItemJsonRepository;
        }

        public async Task Seed()
        {
            var storePath = _configuration.GetValue<string>("JsonStorePath");
            if((await _studyItemJsonRepository.GetAll()).Count() == 0)
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
                await _studyItemJsonRepository.AddAll(entities);
                Console.WriteLine($"Seed finished.");
            }
            else
            {
                Console.WriteLine($"Db already contains data. Do not seed.");
            }
        }
    }
}
