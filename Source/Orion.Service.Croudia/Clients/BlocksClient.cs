﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Orion.Service.Croudia.Models;
using Orion.Service.Shared;

namespace Orion.Service.Croudia.Clients
{
    public class BlocksClient : ApiClient<CroudiaClient>
    {
        internal BlocksClient(CroudiaClient client) : base(client) { }

        public Task<User> CreateAsync(string screenName = null, int? userId = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrWhiteSpace(screenName))
                parameters.Add(new KeyValuePair<string, object>("screen_name", screenName));
            else if (userId.HasValue)
                parameters.Add(new KeyValuePair<string, object>("user_id", userId.Value));
            else
                throw new ArgumentNullException();

            return AppClient.PostAsync<User>("blocks/create.json", parameters, true);
        }

        public Task<User> DestroyAsync(string screenName = null, int? userId = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrWhiteSpace(screenName))
                parameters.Add(new KeyValuePair<string, object>("screen_name", screenName));
            else if (userId.HasValue)
                parameters.Add(new KeyValuePair<string, object>("user_id", userId.Value));
            else
                throw new ArgumentNullException();

            return AppClient.PostAsync<User>("blocks/destroy.json", parameters, true);
        }

        public Task<UsersWithCursor> ListAsync(int? cursor = null, bool? trimUser = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (cursor.HasValue)
                parameters.Add(new KeyValuePair<string, object>("cursor", cursor.Value));
            if (trimUser.HasValue)
                parameters.Add(new KeyValuePair<string, object>("trim_user", trimUser.Value.ToString().ToLower()));

            return AppClient.GetAsync<UsersWithCursor>("blocks/list.json", parameters, true);
        }

        public Task<UsersWithCursor> IdsAsync(int? cursor = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (cursor.HasValue)
                parameters.Add(new KeyValuePair<string, object>("cursor", cursor.Value));

            return AppClient.GetAsync<UsersWithCursor>("blocks/ids.json", parameters, true);
        }
    }
}