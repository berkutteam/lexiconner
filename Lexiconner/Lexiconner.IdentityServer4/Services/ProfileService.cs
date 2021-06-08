using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MoreLinq.Extensions.DistinctByExtension;

namespace Lexiconner.IdentityServer4.Services
{
    public class ProfileService : IProfileService
    {
        protected ApplicationSettings _config;
        protected UserManager<ApplicationUserEntity> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUserEntity> _claimsFactory;
        protected IDataRepository _dataRepository;

        public ProfileService(
            IOptions<ApplicationSettings> config,
            UserManager<ApplicationUserEntity> userManager,
            IUserClaimsPrincipalFactory<ApplicationUserEntity> claimsFactory,
            IDataRepository dataRepository
        )
        {
            _config = config.Value;
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _dataRepository = dataRepository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Extend here for custom data  and claims like email from user database
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            // add custom claims
            var claims = principal.Claims.DistinctBy(x => x.Type).ToList();
            context.IssuedClaims.AddRange(claims);

            foreach (var role in user.Roles)
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
