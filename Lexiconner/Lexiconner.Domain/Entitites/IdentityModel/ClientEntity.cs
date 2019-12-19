using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using NUlid;
using NUlid.Rng;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.IdentityModel
{
    public class ClientEntity : IIdentifiableEntity
    {
        public ClientEntity()
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
        }
        public ClientEntity(Client client)
        {
            Id = Ulid.NewUlid(new CSUlidRng()).ToString();
            this.Client = client;
        }

        public Client Client { get; set; }

        public string Id { get; set; }
    }
}
