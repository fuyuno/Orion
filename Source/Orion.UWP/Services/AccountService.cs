﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Security.Credentials;

using Newtonsoft.Json;

using Orion.UWP.Models;
using Orion.UWP.Services.Interfaces;

namespace Orion.UWP.Services
{
    internal class AccountService : IAccountService
    {
        private readonly ObservableCollection<Account> _accounts;
        private int _counter;

        public AccountService()
        {
            _accounts = new ObservableCollection<Account>();
            _counter = 0;
        }

        public ReadOnlyObservableCollection<Account> Accounts => new ReadOnlyObservableCollection<Account>(_accounts);

        public Task ClearAsync()
        {
            _accounts.Clear();
            var vault = new PasswordVault();
            foreach (var credential in vault.RetrieveAll())
                vault.Remove(credential);
            return Task.CompletedTask;
        }

        public Task RegisterAsync(Account account)
        {
            try
            {
                if (_accounts.Count == 0)
                    account.MarkAsDefault = true;
                var vault = new PasswordVault();
                vault.Add(new PasswordCredential("Orion.Accounts", $"{_counter++}-{account.Provider.Name}", JsonConvert.SerializeObject(account)));
                _accounts.Add(account);
            }
            catch
            {
                // ignored
            }
            return Task.CompletedTask;
        }

        public async Task RestoreAsync()
        {
            try
            {
                var vault = new PasswordVault();
                foreach (var credential in vault.RetrieveAll())
                {
                    credential.RetrievePassword();
                    var account = JsonConvert.DeserializeObject<Account>(credential.Password);
                    if (await account.RestoreAsync())
                        await RegisterAsync(account);
                    else
                        vault.Remove(credential);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}