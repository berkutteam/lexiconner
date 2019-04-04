using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lexiconner.Web.Models;
using Microsoft.Extensions.Options;

namespace Lexiconner.Web.Controllers
{
    public class ConfigController : Controller
    {
        private readonly ApplicationClientSettings _settings;

        public ConfigController(IOptions<ApplicationClientSettings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            return Json(_settings);
        }
    }
}
