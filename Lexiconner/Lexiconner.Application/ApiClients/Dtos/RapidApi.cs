using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApiClients.Dtos
{
    public class RapidApiResponseInfoDto
    {
        // X-RapidAPI-Region
        public string XRapidApiRegion { get; set; }

        // X-RapidAPI-Version
        public string XRapidApiVersion { get; set; }

        // X-RateLimit-requests-Limit
        public long XRateLimitRequestsLimit { get; set; }

        // X-RateLimit-requests-Remaining
        public long XRateLimitRequestsRemaining { get; set; }

    }
}
