using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebView.Wpf;

namespace ModelViewer3D
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddBlazorWebViewDeveloperTools();
            Resources.Add("services", serviceCollection.BuildServiceProvider());
			// Wait until WebView2 is ready before opening DevTools
			webView1.BlazorWebViewInitialized  += BlazorWebView_WebViewInitialized;
			

		}
		private async void BlazorWebView_WebViewInitialized(object sender, EventArgs e)
		{
			try
			{
				// Ensure CoreWebView2 is initialized
				await webView1.WebView.EnsureCoreWebView2Async();

				// Open the browser console (DevTools)
				webView1.WebView.CoreWebView2.OpenDevToolsWindow();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error initializing WebView2: {ex.Message}");
			}
		}
    }
}
