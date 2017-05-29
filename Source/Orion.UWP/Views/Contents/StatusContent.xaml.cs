﻿using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

using Orion.UWP.ViewModels.Contents;

using WinRTXamlToolkit.Controls.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Orion.UWP.Views.Contents
{
    public sealed partial class StatusContent : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(StatusViewModel), typeof(StatusContent), new PropertyMetadata(null, OnViewModelChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(StatusContent), new PropertyMetadata(false, OnIsSelectedChanged));

        public static readonly DependencyProperty IsMiniModeProperty =
            DependencyProperty.Register(nameof(IsMiniMode), typeof(bool), typeof(StatusContent), new PropertyMetadata(false, OnIsMiniModeChanged));

        public static readonly DependencyProperty IsShowIconProperty =
            DependencyProperty.Register(nameof(IsShowIcon), typeof(bool), typeof(StatusContent), new PropertyMetadata(true, OnIsShowIconChanged));

        public static readonly DependencyProperty IsShowImagePreviewsProperty =
            DependencyProperty.Register(nameof(IsShowImagePreviews), typeof(bool), typeof(StatusContent),
                                        new PropertyMetadata(true, OnIsShowImagePreviewsChanged));

        public static readonly DependencyProperty IsShowTimestampProperty =
            DependencyProperty.Register(nameof(IsShowTimestamp), typeof(bool), typeof(StatusContent), new PropertyMetadata(true, OnIsShowTimestampChanged));

        public StatusViewModel ViewModel
        {
            get => (StatusViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public bool IsMiniMode
        {
            get => (bool) GetValue(IsMiniModeProperty);
            set => SetValue(IsMiniModeProperty, value);
        }

        public bool IsShowIcon
        {
            get => (bool) GetValue(IsShowIconProperty);
            set => SetValue(IsShowIconProperty, value);
        }

        public bool IsShowImagePreviews
        {
            get => (bool) GetValue(IsShowImagePreviewsProperty);
            set => SetValue(IsShowImagePreviewsProperty, value);
        }

        public bool IsShowTimestamp
        {
            get => (bool) GetValue(IsShowTimestampProperty);
            set => SetValue(IsShowTimestampProperty, value);
        }

        public StatusContent()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            AppBar.Visibility = Visibility.Collapsed;
            AppBar.Height = 0;
            Body.IsTextSelectionEnabled = false;
        }

        private static void OnIsShowTimestampChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is StatusContent status)
                status.Timestamp.Visibility = (bool) e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnIsShowImagePreviewsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is StatusContent status)
                status.ImagePreviews.Visibility = (bool) e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnIsShowIconChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is StatusContent status)
                if ((bool) e.NewValue)
                {
                    status.Icon.Visibility = Visibility.Visible;
                    status.UserLine.Padding = status.Body.Padding = new Thickness(10, 0, 0, 0);
                }
                else
                {
                    status.Icon.Visibility = Visibility.Collapsed;
                    status.UserLine.Padding = status.Body.Padding = new Thickness(0);
                }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (IsMiniMode)
                return;

            var root = this.GetFirstAncestorOfType<ListViewItem>();
            if (root == null)
                return;
            SetBinding(IsSelectedProperty, new Binding
            {
                Source = root,
                Path = new PropertyPath(nameof(root.IsSelected)),
                Mode = BindingMode.TwoWay
            });
        }

        private static void OnViewModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null && args.NewValue != args.OldValue)
                (dependencyObject as StatusContent)?.ResetLayouts();
        }

        private static void OnIsSelectedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if ((bool) args.NewValue)
                (dependencyObject as StatusContent)?.ExpandCommandBar();
            else
                (dependencyObject as StatusContent)?.ContractCommandBar();
        }

        private static void OnIsMiniModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is StatusContent status && (bool) args.NewValue)
            {
                status.Icon.Height = status.Icon.Width = 32;
                status.Username.FontSize = status.Body.FontSize = 13;
                status.ScreenName.FontSize = status.Timestamp.FontSize = 12;
                status.ImagePreviews.ItemHeight = 40;
                status.Loaded += StatusOnLoaded;
            }
        }

        private static void StatusOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is StatusContent status)
            {
                status.ImagePreviews.ItemWidth = status.DesiredSize.Width / 2 - 10;
                status.Loaded -= StatusOnLoaded;
            }
        }

        private void ResetLayouts()
        {
            AppBar.Visibility = Visibility.Collapsed;
            AppBar.Height = 0;
            Body.IsTextSelectionEnabled = false;
            Body.Measure(new Size());
            ImagePreviews.Measure(new Size());
        }

        private void ContractCommandBar()
        {
            if (RootPanel.ActualHeight > 40)
                RootPanel.Height = RootPanel.ActualHeight - 40;
            AppBar.Visibility = Visibility.Collapsed;
            AppBar.Height = 0;
            Body.IsTextSelectionEnabled = false;
        }

        private void ExpandCommandBar()
        {
            RootPanel.Height = RootPanel.ActualHeight + 40;
            AppBar.Visibility = Visibility.Visible;
            AppBar.Height = 40;
            Body.IsTextSelectionEnabled = true;
        }
    }
}