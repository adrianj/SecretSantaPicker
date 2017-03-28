using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SecretSantaPicker
{
    public sealed partial class NewPlayerDialog : ContentDialog
    {
        public string NewPlayerName { get { return nameBox.Text; } }

        public NewPlayerDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public new IAsyncOperation<ContentDialogResult> ShowAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<ContentDialogResult>();

            nameBox.KeyDown += (sender, args) =>
            {
                if (args.Key != Windows.System.VirtualKey.Enter) return;
                tcs.TrySetResult(ContentDialogResult.Primary);
                Hide();
                args.Handled = true;
            };

            var asyncOperation = base.ShowAsync();
            asyncOperation.AsTask().ContinueWith(task => tcs.TrySetResult(task.Result));
            return tcs.Task.AsAsyncOperation();
        }

    }
}
