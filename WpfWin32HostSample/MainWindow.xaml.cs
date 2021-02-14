using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfWin32HostSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UnityFtpConnection unityConnection = new UnityFtpConnection();
        UnityHost unity;

        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => { unityConnection.Test(); });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            unity = new UnityHost("Test.exe");
            ControlHostElement.Child = unity;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            unityConnection.Send("Hello World");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            unityConnection.Stop();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void ActivateUnityWindow()
        {
            //if (unity == null) return;
            //SendMessage(unity.UnityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            //if (unity == null) return;
            //SendMessage(unity.UnityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }
    }
}
