using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfXInput
{
    public partial class UserThumb : UserControl
    {
        public UserThumb()
        {
            InitializeComponent();
            SizeChanged += OnSizeChange;
            Back.Visibility = Visibility.Collapsed;
            BackR.Visibility = Visibility.Visible;
        }

        private void OnSizeChange(object sender, SizeChangedEventArgs e)
        {
            // 保证摇杆大小变化时位置也刷新
            Thumb.Width = ActualWidth / 2;
            Thumb.Height = ActualHeight / 2;
            UpdateThumbPosition();
        }

        // 注册 DependencyProperty：X
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                nameof(X),
                typeof(short),
                typeof(UserThumb),
                new PropertyMetadata((short)0, OnThumbChanged));

        public short X
        {
            get => (short)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        // 注册 DependencyProperty：Y
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                nameof(Y),
                typeof(short),
                typeof(UserThumb),
                new PropertyMetadata((short)0, OnThumbChanged));

        public short Y
        {
            get => (short)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        private static void OnThumbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UserThumb thumb)
            {
                thumb.UpdateThumbPosition();
            }
        }

        private void UpdateThumbPosition()
        {
            if (ActualWidth == 0 || ActualHeight == 0) return;

            // 修正 X 和 Y 的映射，X 和 Y 不反转
            double newX = (ActualWidth / 2) * ((X + 35535.0) / 71070.0); // X 映射到 [0, ActualWidth/2]
            double newY = (ActualHeight / 2) * ((-Y + 35535.0) / 71070.0); // Y 反转映射到 [0, ActualHeight/2]

            Thumb.Margin = new Thickness(newX, newY, 0, 0);
            Back.Margin = new Thickness(
              ActualWidth / 4 - StrokeThickness,
              ActualHeight / 4 - StrokeThickness,
              ActualWidth / 4 - StrokeThickness,
              ActualHeight / 4 - StrokeThickness
          );
           
        }

        // 保留PointSize属性以设置摇杆大小
        public double PointSize
        {
            get => point.Width;
            set
            {
                point.Width = value;
                point.Height = value;
            }
        }

        public double StrokeThickness
        {
            get => Back.StrokeThickness;
            set => Back.StrokeThickness = value;
        }
    }
}
