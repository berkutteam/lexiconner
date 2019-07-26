using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Cache
{
    public abstract class DataCacheBaseEntity : BaseEntity
    {
        // prop to search cached document
        public string CacheKey { get; set; }

        /// <summary>
        /// Returns key by which cache item can be compared
        /// </summary>
        /// <returns></returns>
        public virtual string GetCacheKey()
        {
            throw new NotImplementedException();
        }
    }
}
