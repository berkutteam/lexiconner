using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApplicationSettings
{
    public class GoogleSettings
    {
        public string ProjectId { get; set; }

        public GoogleCredentialSettings WebApiServiceAccount { get; set; }

        public class GoogleCredentialSettings
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("project_id")]
            public string ProjectId { get; set; }

            [JsonProperty("private_key_id")]
            public string PrivateKeyId { get; set; }

            [JsonProperty("private_key")]
            public string PrivateKey { get; set; }

            [JsonProperty("client_email")]
            public string ClientEmail { get; set; }

            [JsonProperty("client_id")]
            public string ClientId { get; set; }

            [JsonProperty("auth_uri")]
            public string AuthUri { get; set; }

            [JsonProperty("token_uri")]
            public string TokenUri { get; set; }

            [JsonProperty("auth_provider_x509_cert_url")]
            public string AuthProviderX509CertUrl { get; set; }

            [JsonProperty("client_x509_cert_url")]
            public string ClientX509CertUrl { get; set; }
        }
    }
}
