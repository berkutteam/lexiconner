using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Application.ApiClients.Dtos.TwinwordWordDictionary
{
    public class ReferenceResponseDto
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("entry")]
        public ReferenceEntryResponseDto Entry { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        /// <summary>
        /// HTTP code
        /// </summary>
        [JsonProperty("result_code")]
        public string ResultCode { get; set; }

        [JsonProperty("result_msg")]
        public string ResultMsg { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class ReferenceEntryResponseDto
    {
        private const char WordsDelimiter = ',';

        [JsonProperty("associations")]
        public string Associations { get; set; }

        [JsonProperty("broad_terms")]
        public string BroadTerms { get; set; }

        [JsonProperty("derived_terms")]
        public string DerivedTerms { get; set; }

        [JsonProperty("evocations")]
        public string Evocations { get; set; }

        [JsonProperty("narrow_terms")]
        public string NarrowTerms { get; set; }

        [JsonProperty("related_terms")]
        public string RelatedTerms { get; set; }

        [JsonProperty("synonyms")]
        public string Synonyms { get; set; }

        #region Helpers

        public IEnumerable<string> GetAssociationsList()
        {
            if(string.IsNullOrEmpty(this.Associations))
            {
                return new List<string>();
            }

            return this.Associations.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        public IEnumerable<string> GetBroadTermsList()
        {
            if (string.IsNullOrEmpty(this.BroadTerms))
            {
                return new List<string>();
            }
            return this.BroadTerms.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        public IEnumerable<string> GetDerivedTermsList()
        {
            if (string.IsNullOrEmpty(this.DerivedTerms))
            {
                return new List<string>();
            }
            return this.DerivedTerms.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        public IEnumerable<string> GetEvocationsList()
        {
            if (string.IsNullOrEmpty(this.Evocations))
            {
                return new List<string>();
            }
            return this.Evocations.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        public IEnumerable<string> GetNarrowTermsList()
        {
            if (string.IsNullOrEmpty(this.NarrowTerms))
            {
                return new List<string>();
            }
            return this.NarrowTerms.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        public IEnumerable<string> GetRelatedTermsList()
        {
            if (string.IsNullOrEmpty(this.RelatedTerms))
            {
                return new List<string>();
            }
            return this.RelatedTerms.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        public IEnumerable<string> GetSynonymsList()
        {
            if (string.IsNullOrEmpty(this.Synonyms))
            {
                return new List<string>();
            }
            return this.Synonyms.Split(WordsDelimiter).Select(x => x.Trim()).ToList();
        }

        #endregion
    }
}
