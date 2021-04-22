using Lexiconner.Domain.Dtos.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.UserDictionaries
{
    public class UserDictionaryDto
    {
        public UserDictionaryDto()
        {
        }

        public string Id { get; set; }
        public string WordsLanguageCode { get; set; }
        public List<UserDictionaryWordSetDto> WordSets { get; set; }
    }

    public class UserDictionaryWordSetDto
    {
        public UserDictionaryWordSetDto()
        {
            Images = new List<GeneralImageDto>();
        }

        public string SourceWordSetId { get; set; }
        public string Name { get; set; }
        public string WordsLanguageCode { get; set; }
        public string MeaningsLanguageCode { get; set; }
        public List<GeneralImageDto> Images { get; set; }
    }
}
