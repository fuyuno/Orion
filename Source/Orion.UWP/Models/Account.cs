﻿using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Orion.UWP.Models.Clients;
using Orion.UWP.Models.Enum;

namespace Orion.UWP.Models
{
    public class Account
    {
        /// <summary>
        ///     Unique ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Service provider
        /// </summary>
        public Provider Provider { get; set; }

        /// <summary>
        ///     Credentials
        /// </summary>
        public Credential Credential { get; set; }

        /// <summary>
        ///     Mark as default (If timeline is not associated with account, use this)
        /// </summary>
        public bool MarkAsDefault { get; set; }

        /// <summary>
        ///     Client wrapper
        /// </summary>
        [JsonIgnore]
        public BaseClientWrapper ClientWrapper { get; set; }

        public Account()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Task<bool> RestoreAsync()
        {
            ClientWrapper = CreateClientWrapper();
            return ClientWrapper.RefreshAccountAsync();
        }

        private BaseClientWrapper CreateClientWrapper()
        {
            switch (Provider.Service)
            {
                case ServiceType.Twitter:
                    return new TwitterClientWrapper(this);

                case ServiceType.Croudia:
                    return new CroudiaClientWrapper(this);

                case ServiceType.GnuSocial:
                    return new GnuSocialClientWrapper(this);

                case ServiceType.Mastodon:
                    return new MastodonClientWrapper(this);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}