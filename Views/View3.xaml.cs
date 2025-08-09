using System.Windows.Controls;
using System.Windows.Input;

namespace prism_serial.Views
{
    /// <summary>
    /// View3.xaml 的交互逻辑
    /// </summary>
    public partial class View3 : UserControl
    {
        public View3()
        {
            InitializeComponent();
        }
        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tb = sender as TextBox;

                // 提交绑定值到 ViewModel
                var binding = tb.GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();

                e.Handled = true; // 防止回车产生“叮”声
            }
        }
    }
}