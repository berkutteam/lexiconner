﻿using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using MongoDB.Bson;
using NUlid;
using NUlid.Rng;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.IdentityModel
{
    public class ApiResourceEntity : IIdentifiableEntity
    {
        public ApiResourceEntity()
        {
            Id = ObjectId.GenerateNewId().ToString();
            ApiResource = new ApiResource();
        }

        public ApiResourceEntity(ApiResource resource)
        {
            Id = ObjectId.GenerateNewId().ToString();
            ApiResource = resource;
        }

        public ApiResource ApiResource { get; set; }

        public string Id { get; set; }
    }
}
