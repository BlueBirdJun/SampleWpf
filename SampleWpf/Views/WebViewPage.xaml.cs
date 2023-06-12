using System.Windows;
using System.Windows.Controls;

using Microsoft.Web.WebView2.Core;

using SampleWpf.ViewModels;

namespace SampleWpf.Views;

public partial class WebViewPage : UserControl
{
    private WebViewViewModel ViewModel
        => DataContext as WebViewViewModel;

    public WebViewPage()
    {
        InitializeComponent();
    }

    private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        => ViewModel.OnNavigationCompleted(sender, e);

    private void WebViewPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel?.Initialize(webView);
    }
}
