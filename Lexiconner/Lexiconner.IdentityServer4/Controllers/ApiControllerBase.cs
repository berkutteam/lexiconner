using Lexiconner.IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected BaseApiResponseModel<T> BaseJsonResponse<T>(T data)
        {
            return new BaseApiResponseModel<T>()
            {
                Data = data,
            };
        }

        protected string GetUserId()
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            return currentUserId;
        }
    }
}
