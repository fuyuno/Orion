﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;

using Orion.UWP.Models;
using Orion.UWP.Models.Clients;
using Orion.UWP.Mvvm;

using Reactive.Bindings;

namespace Orion.UWP.ViewModels
{
    public class AuthorizationDialogViewModel : ViewModel
    {
        private BaseClientWrapper _clientWrapper;
        public ReadOnlyCollection<Provider> Providers => Constants.Providers;

        public ReactiveProperty<Provider> SelectedProvider { get; }
        public ReactiveProperty<bool> HasHost { get; }
        public ReactiveProperty<string> Host { get; }
        public ReactiveProperty<bool> HasApiKey { get; }
        public ReactiveProperty<string> ConsumerKey { get; }
        public ReactiveProperty<string> ConsumerSecret { get; }
        public ReactiveProperty<Uri> Source { get; }
        public ReactiveProperty<string> VerifierCode { get; }
        public ReactiveCommand GoAuthorizePageCommand { get; }
        public ReactiveCommand AuthorizeCommand { get; }

        public AuthorizationDialogViewModel()
        {
            Title = "アプリケーションの認証 (1/2)";
            IsFirstPage = true;
            CanClose = false;
            SelectedProvider = new ReactiveProperty<Provider>();
            SelectedProvider.Subscribe(_ => UpdateCanExecuteGoAuthorizePage()).AddTo(this);
            HasHost = SelectedProvider.Select(w => w?.RequireHost ?? false).ToReactiveProperty();
            Host = new ReactiveProperty<string>();
            Host.Subscribe(_ => UpdateCanExecuteGoAuthorizePage()).AddTo(this);
            HasApiKey = SelectedProvider.Select(w => w?.RequireApiKeys ?? false).ToReactiveProperty();
            ConsumerKey = new ReactiveProperty<string>();
            ConsumerKey.Subscribe(_ => UpdateCanExecuteGoAuthorizePage()).AddTo(this);
            ConsumerSecret = new ReactiveProperty<string>();
            ConsumerSecret.Subscribe(_ => UpdateCanExecuteGoAuthorizePage()).AddTo(this);
            Source = new ReactiveProperty<Uri>(new Uri("https://ori.kokoiroworks.com/start"));
            Source.Subscribe(w =>
            {
                var regex = SelectedProvider?.Value?.ParseRegex;
                if (regex == null || !regex.IsMatch(w.ToString()))
                    return;
                VerifierCode.Value = regex.Match(w.ToString()).Groups["verifier"].Value;
                AuthorizeCommand.Execute();
            });
            VerifierCode = new ReactiveProperty<string>();
            GoAuthorizePageCommand = new ReactiveCommand();
            GoAuthorizePageCommand.Subscribe(async _ =>
            {
                Title = "アプリケーションの認証 (2/2)";
                IsFirstPage = false;
                var provider = SelectedProvider.Value;
                provider.Configure(Host.Value, ConsumerKey.Value, ConsumerSecret.Value);
                _clientWrapper = provider.CreateClientWrapper();
                IsEnableVerifierInput = provider.ParseRegex == null;
                Source.Value = new Uri(await _clientWrapper.GetAuthorizeUrlAsync());
            }).AddTo(this);
            AuthorizeCommand = VerifierCode.Select(w => !string.IsNullOrWhiteSpace(w)).ToReactiveCommand();
            AuthorizeCommand.Subscribe(async _ =>
            {
                await _clientWrapper.AuthorizeAsync(VerifierCode.Value);
                Debug.WriteLine(_clientWrapper.Account);
                CanClose = true;
            }).AddTo(this);
        }

        private void UpdateCanExecuteGoAuthorizePage()
        {
            CanExecuteGoAuthorizePage = SelectedProvider?.Value?.ValidateConfiguration(Host?.Value, ConsumerKey?.Value, ConsumerSecret?.Value) ?? false;
        }

        #region Title

        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #endregion

        #region CanExecuteGoAuthorizePage

        private bool _canExecuteGoAuthorizePage;

        public bool CanExecuteGoAuthorizePage
        {
            get => _canExecuteGoAuthorizePage;
            set => SetProperty(ref _canExecuteGoAuthorizePage, value);
        }

        #endregion

        #region IsFirstPage

        private bool _isFirstPage;

        public bool IsFirstPage
        {
            get => _isFirstPage;
            set => SetProperty(ref _isFirstPage, value);
        }

        #endregion

        #region IsEnableVerifierInput

        private bool _isEnableVerifierInput;

        public bool IsEnableVerifierInput
        {
            get => _isEnableVerifierInput;
            set => SetProperty(ref _isEnableVerifierInput, value);
        }

        #endregion

        #region CanClose

        private bool _canClose;

        public bool CanClose
        {
            get => _canClose;
            set => SetProperty(ref _canClose, value);
        }

        #endregion
    }
}