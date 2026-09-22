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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var notification = new ToastNotificationData
            {
                Message = "Đăng nhập thành công!" + " " + count++,
                Type = ToastTypeEnum.Info,
                Duration = TimeSpan.FromSeconds(10),
                PauseOnClick = true,
                UpdateMessageAction = (currentMessage, currentTime) =>
                {
                    return $"Bạn còn {currentTime} giây";
                },
                FinishAction = () =>
                {
                    MessageBox.Show("heheboi");
                }
                
            };
            ToastNotificationManager.Instance.Show(notification);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var notification = new ToastNotificationData
            {
                Message = "Đăng nhập thành công!" + " " + count++,
                Type = ToastTypeEnum.Success
            };
            ToastNotificationManager.Instance.Show(notification);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var notification = new ToastNotificationData
            {
                Message = "Đăng nhập thành công!" + " " + count++,
                Type = ToastTypeEnum.Warning
            };
            ToastNotificationManager.Instance.Show(notification);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var notification = new ToastNotificationData
            {
                Message = "Đăng nhập thành công!" + " " + count++,
                Type = ToastTypeEnum.Error
            };
            ToastNotificationManager.Instance.Show(notification);
        }
    }
}
