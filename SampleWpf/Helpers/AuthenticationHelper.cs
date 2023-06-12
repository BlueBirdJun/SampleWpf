using System.Windows;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using SampleWpf.Core.Helpers;
using SampleWpf.Properties;

namespace SampleWpf.Helpers;

internal static class AuthenticationHelper
{
    internal static async Task ShowLoginErrorAsync(LoginResultType loginResult)
    {
        var metroWindow = Application.Current.MainWindow as MetroWindow;
        switch (loginResult)
        {
            case LoginResultType.NoNetworkAvailable:
                await metroWindow.ShowMessageAsync(Resources.DialogNoNetworkAvailableContent, Resources.DialogAuthenticationTitle);
                break;
            case LoginResultType.UnknownError:
                await metroWindow.ShowMessageAsync(Resources.DialogAuthenticationTitle, Resources.DialogStatusUnknownErrorContent);
                break;
        }
    }
}
