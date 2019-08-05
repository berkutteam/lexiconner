using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites
{
    /// <summary>
    /// Stores additional info that is application specific. 
    /// <see cref="ApplicationUserEntity" /> stores basic info required for authorization and authentication.
    /// </summary>
    public class UserInfoEntity : BaseEntity
    {
        public string IdentityUserId { get; set; }
    }
}
