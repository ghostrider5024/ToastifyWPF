using System.Windows;
using ToastifyWPF.Enums;
using ToastifyWPF.Managers;
using ToastifyWPF.Models;


namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        int count = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var notification = new ToastNotificationData
            {
                Message = "Đăng nhập thành công!" + " " + count++,
                Type = ToastTypeEnum.Info
            };
            ToastNotificationManager.Instance.Show(notification);
        }
    }
}