﻿using System;
using System.Threading.Tasks;
using MarkPad.Core;
using MarkPad.Sources.LocalFiles;
using MarkPad.ViewModel;
using MarkPad.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MarkPad
{
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();
            App.Current.RequestedTheme = ApplicationTheme.Light;
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            if (args.Arguments != string.Empty)
            {
                LoadFromPinned(args.Arguments);
            }

            Window.Current.Activate();
        }

        private async Task LoadFromPinned(string args)
        {
            var locator = (ViewModelLocator)Resources["Locator"];
            var doc = await LocalDocument.FromPath(args);
            doc.Token = args;
            locator.Main.Open(doc);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            var locator = (ViewModelLocator)Resources["Locator"];
            var files = args.Files;
            foreach (StorageFile file in files)
            {
                var x = new LocalDocument(file);
                x.Load();
                locator.Main.Open(x);
            }

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
            }

            Window.Current.Activate();
        }
    }
}
