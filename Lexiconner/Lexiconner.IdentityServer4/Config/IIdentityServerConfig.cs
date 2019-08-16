using IdentityServer4.Models;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Config
{
    public interface IIdentityServerConfig
    {
        IEnumerable<IdentityResource> GetIdentityResources();
        IEnumerable<ApiResource> GetApiResources();
        IEnumerable<Client> GetClients(ApplicationSettings config);

        string DefaultUserPassword { get; }
        List<ApplicationRoleEntity> GetInitialIdentityRoles();
        List<ApplicationUserEntity> GetInitialdentityUsers();
    }
}
