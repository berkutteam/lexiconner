using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Config
{
    public class GoogleSettings
    {
        public string ProjectId { get; set; }

        public GoogleCredentialSettings WebApiServiceAccount { get; set; }

        public class GoogleCredentialSettings
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("project_id")]
            public string project_id { get; set; }

            [JsonProperty("private_key_id")]
            public string private_key_id { get; set; }

            [JsonProperty("private_key")]
            public string private_key { get; set; }

            [JsonProperty("client_email")]
            public string client_email { get; set; }

            [JsonProperty("client_id")]
            public string client_id { get; set; }

            [JsonProperty("auth_uri")]
            public string auth_uri { get; set; }

            [JsonProperty("token_uri")]
            public string token_uri { get; set; }

            [JsonProperty("auth_provider_x509_cert_url")]
            public string auth_provider_x509_cert_url { get; set; }

            [JsonProperty("client_x509_cert_url")]
            public string client_x509_cert_url { get; set; }
        }
    }

    public class RapidApiSettings
    {
        public SpecificRapidApiSettings ContextualWebSearch { get; set; }

        public class SpecificRapidApiSettings
        {
            public string ApplicationKey { get; set; }
        }
    }
}
