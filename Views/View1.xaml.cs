using prism_serial.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace prism_serial.Views
{
    /// <summary>
    /// View1.xaml 的交互逻辑
    /// </summary>
    public partial class View1 : UserControl
    {
        public View1(View1ViewModel viewModel)
        {
            InitializeComponent();
            this._view1ViewModel = viewModel;
            this.DataContext = viewModel;
        }

        private View1ViewModel _view1ViewModel;

        public void OnTxBox_KeyDownCommand(object sender, KeyEventArgs e)
        {
            _view1ViewModel.OnTxBox_KeyDownCommand(sender, e);
        }

    }
}