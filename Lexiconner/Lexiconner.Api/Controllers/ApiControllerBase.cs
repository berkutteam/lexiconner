using Lexiconner.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lexiconner.Api.Controllers
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
    }
}
