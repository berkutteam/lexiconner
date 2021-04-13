using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.Misc
{
    public class LanguageDto
    {
        public string LanguageFamily { get; set; }
        public string IsoLanguageName { get; set; }
        public string NativeName { get; set; }
        public string Iso639_1_Code { get; set; }
        public string CountryIsoAlpha2Code { get; set; }
    }
}
