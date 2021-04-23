using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.UserDictionaries
{
    public class AddWordSetToUserDictionaryRequestDto
    {
        public string WordSetId { get; set; }
        public IEnumerable<string> SelectedWordIds { get; set; }
    }
}
