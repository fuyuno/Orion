﻿using Windows.UI.Xaml.Controls;

using Orion.UWP.ViewModels;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Orion.UWP.Views
{
    /// <summary>
    ///     それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}