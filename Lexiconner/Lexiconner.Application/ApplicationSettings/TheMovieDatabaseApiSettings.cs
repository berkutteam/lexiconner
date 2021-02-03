using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApplicationSettings
{
    /// <summary>
    /// https://www.themoviedb.org/settings/api
    /// </summary>
    public class TheMovieDatabaseApiSettings
    {
        /// <summary>
        /// Normal key
        /// </summary>
        public string ApiKeyV3Auth { get; set; }

        /// <summary>
        /// API Read Access Token
        /// </summary>
        public string ApiKeyV4Auth { get; set; }
    }
}
