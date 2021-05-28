using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Lexiconner.Application.Exceptions;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Dtos.Identity.Account;
using Lexiconner.Domain.Entitites;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.IdentityServer4.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationSettings _config;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly SignInManager<ApplicationUserEntity> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IUserClaimsPrincipalFactory<ApplicationUserEntity> _principalFactory;
        private readonly ITokenService _tokenService;
        private readonly IdentityServerOptions _identityServerOptions;
        private readonly IRefreshTokenService _refreshTokenService;

        public AccountService(
            IOptions<ApplicationSettings> config,
            UserManager<ApplicationUserEntity> userManager,
            SignInManager<ApplicationUserEntity> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IIdentityServerConfig identityServerConfig,
            IUserClaimsPrincipalFactory<ApplicationUserEntity> principalFactory,
            ITokenService tokenService,
            IdentityServerOptions identityServerOptions,
            IRefreshTokenService refreshTokenService
        )
        {
            _config = config.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _identityServerConfig = identityServerConfig;
            _principalFactory = principalFactory;
            _tokenService = tokenService;
            _identityServerOptions = identityServerOptions;
            _refreshTokenService = refreshTokenService;
        }

        #region BrowserExtension

        /// <summary>
        /// Custom login for browser extensions
        /// </summary>
        public async Task<BrowserExtensionLoginResponseDto> BrowserExtensionLoginAsync(BrowserExtensionLoginRequestDto dto)
        {
            const string clientId = "browserExtension";
            var client = _identityServerConfig.GetClients(_config).FirstOrDefault(x => x.ClientId == clientId);
            if (client == null)
            {
                throw new InternalErrorException($"Can't find client with id {clientId}.");
            }
            if (client.ClientId != dto.ClientId)
            {
                throw new BadRequestException($"Client id {dto.ClientId} is invalid.");
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            var signinResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!signinResult.Succeeded || signinResult.IsNotAllowed)
            {
                throw new UnauthorizedException("Login failed. Email or password is invalid.");
            }

            return await BrowserExtensionGenerateTokensAsync(user.Id, dto.ExtensionVersion);
        }

        /// <summary>
        /// Custom tokens refresh for browser extensions
        /// </summary>
        public async Task<BrowserExtensionLoginResponseDto> BrowserExtensionRefreshTokensAsync(BrowserExtensionRefreshTokensRequestDto dto)
        {
            const string clientId = "browserExtension";
            var client = _identityServerConfig.GetClients(_config).FirstOrDefault(x => x.ClientId == clientId);
            if (client == null)
            {
                throw new InternalErrorException($"Can't find client with id {clientId}.");
            }
            if (client.ClientId != dto.ClientId)
            {
                throw new BadRequestException($"Client id {dto.ClientId} is invalid.");
            }

            var isRefreshTokenValid = await _refreshTokenService.ValidateRefreshTokenAsync(dto.RefreshToken, client);
            if (isRefreshTokenValid.IsError)
            {
                throw new BadRequestException($"Refresh token is invalid: {isRefreshTokenValid.Error}. {isRefreshTokenValid.ErrorDescription}.");
            }

            // decode JWT token
            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.ReadJwtToken(dto.AccessToken);
            string userId = accessToken.Subject;

            return await BrowserExtensionGenerateTokensAsync(userId, dto.ExtensionVersion);
        }

        private async Task<BrowserExtensionLoginResponseDto> BrowserExtensionGenerateTokensAsync(string userId, string extensionVersion)
        {
            const string clientId = "browserExtension";
            var client = _identityServerConfig.GetClients(_config).FirstOrDefault(x => x.ClientId == clientId);
            if (client == null)
            {
                throw new InternalErrorException($"Can't find client with id {clientId}.");
            }

            // check extension version is supported
            if (!BrowserExtensionConfig.Versions.Any(x => x.Version == extensionVersion && x.IsSupported))
            {
                throw new AccessDeniedException($"Browser extension version {extensionVersion} is not found or not supported.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"User not found.");
            }

            // create tokens with user claims and additional info or managing extensions access
            // NB: additional claims are prefixed with "client_" which is configured in client settings
            var tokenCreationRequest = new TokenCreationRequest();
            var identityPricipal = await _principalFactory.CreateAsync(user);
            var identityUser = new IdentityServerUser(user.Id.ToString());

            identityUser.AdditionalClaims = identityPricipal.Claims.ToList();
            identityUser.DisplayName = user.UserName;
            identityUser.AuthenticationTime = DateTime.UtcNow;
            identityUser.IdentityProvider = IdentityServerConstants.LocalIdentityProvider;

            // store extension version in token to be able to forbid access for old versions
            const string browserExtensionVersionClaim = "browser_extension_version";
            identityUser.AdditionalClaims.Add(new Claim(browserExtensionVersionClaim, extensionVersion));

            tokenCreationRequest.Subject = identityUser.CreatePrincipal();
            tokenCreationRequest.IncludeAllIdentityClaims = true;
            tokenCreationRequest.ValidatedRequest = new ValidatedRequest();
            tokenCreationRequest.ValidatedRequest.AccessTokenType = AccessTokenType.Jwt;
            //tokenCreationRequest.ValidatedRequest.AccessTokenLifetime = ; // read from client configuration
            tokenCreationRequest.ValidatedRequest.Subject = tokenCreationRequest.Subject;
            tokenCreationRequest.ValidatedRequest.SetClient(client);
            tokenCreationRequest.ValidatedRequest.Options = _identityServerOptions;
            tokenCreationRequest.ValidatedRequest.ClientClaims = identityUser.AdditionalClaims;
            tokenCreationRequest.ValidatedResources = new ResourceValidationResult(
                new Resources(
                    identityResources: (new List<IdentityResource>()).Concat(_identityServerConfig.GetIdentityResources().ToList()),
                    apiResources: (new List<ApiResource>()).Concat(_identityServerConfig.GetApiResources().ToList()),
                    apiScopes: (new List<ApiScope>()).Concat(client.AllowedScopes.Select(scope => new ApiScope(scope)))
                )
                {
                    OfflineAccess = true,
                }
            );

            var tokenAccess = await _tokenService.CreateAccessTokenAsync(tokenCreationRequest);
            var tokenIdentity = await _tokenService.CreateIdentityTokenAsync(tokenCreationRequest);

            var tokenValueAccess = await _tokenService.CreateSecurityTokenAsync(tokenAccess);
            var tokenValueIdentity = await _tokenService.CreateSecurityTokenAsync(tokenIdentity);
            var tokenValueRefresh = await _refreshTokenService.CreateRefreshTokenAsync(
                tokenCreationRequest.Subject,
                tokenAccess,
                client
            );

            return new BrowserExtensionLoginResponseDto()
            {
                IdentityToken = tokenValueIdentity,
                AccessToken = tokenValueAccess,
                RefreshToken = tokenValueRefresh,
            };
        }

        #endregion
    }
}
