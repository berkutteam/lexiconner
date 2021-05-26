using Lexiconner.Domain.Dtos;
using Lexiconner.IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        protected BaseApiResponseDto<T> BaseJsonResponse<T>(T data)
        {
            return new BaseApiResponseDto<T>()
            {
                Ok = true,
                Data = data,
            };
        }

        protected IActionResult BaseResponse<T>(T data, HttpStatusCode status = HttpStatusCode.OK)
        {

            var responseModel = new BaseApiResponseDto<T>()
            {
                Ok = status == HttpStatusCode.OK,
                Data = data
            };

            return new JsonResult(responseModel)
            {
                ContentType = "application/json",
                SerializerSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                },
                StatusCode = (int)status,
            };
        }

        protected IActionResult StatusCodeBaseResponse(HttpStatusCode status = HttpStatusCode.OK)
        {
            return BaseResponse<object>(null, status);
        }

        protected string GetUserId()
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            return currentUserId;
        }
    }
}
