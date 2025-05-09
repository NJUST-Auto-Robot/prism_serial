using prism_serial.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace prism_serial.Views
{
    /// <summary>
    /// View2.xaml 的交互逻辑
    /// </summary>
    public partial class View2 : UserControl
    {
        public View2(View2ViewModel view2ViewModel)
        {
            this._view2ViewModel = view2ViewModel;
            this.DataContext = view2ViewModel;
            InitializeComponent();
            web.Source = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\Common\mychart.html");
            web.NavigationCompleted += Web_NavigationCompleted;

          
            //view2ViewModel.postDelegate = new View2ViewModel.PostDelegate(web.CoreWebView2.PostWebMessageAsJson);
        }

        private View2ViewModel _view2ViewModel;

        private void Web_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            _view2ViewModel.postDelegate = new View2ViewModel.PostDelegate(web.CoreWebView2.PostWebMessageAsJson);
        }

        private void Keyenterdown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as System.Windows.Controls.TextBox;
                textbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                //MessageBox.Show("Enter key is pressed");
            }
        }
    }
}