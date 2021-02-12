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
        int selectedItem;
        IntPtr hwndListBox;
        ControlHost listControl;
        Application app;
        Window myWindow;
        int itemCount;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //app = Application.Current;
            //myWindow = app.MainWindow;
            //myWindow.SizeToContent = SizeToContent.WidthAndHeight;

            var unityControl = new UnityHwndHost("Game.exe");
            ControlHostElement.Child = unityControl;

            //listControl = new ControlHost(ControlHostElement.ActualHeight, ControlHostElement.ActualWidth);
            //ControlHostElement.Child = listControl;


            //listControl.MessageHook += new HwndSourceHook(ControlMsgFilter);
            //hwndListBox = listControl.hwndListBox;
            //for (int i = 0; i < 15; i++) //populate listbox
            //{
            //    string itemText = "Item" + i.ToString();
            //    SendMessage(hwndListBox, LB_ADDSTRING, IntPtr.Zero, itemText);
            //}
            //itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            //numItems.Text = "" + itemCount.ToString();
        }

        private IntPtr ControlMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int textLength;

            handled = false;
            if (msg == WM_COMMAND)
            {
                switch ((uint)wParam.ToInt32() >> 16 & 0xFFFF) //extract the HIWORD
                {
                    case LBN_SELCHANGE: //Get the item text and display it
                        selectedItem = SendMessage(listControl.hwndListBox, LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
                        textLength = SendMessage(listControl.hwndListBox, LB_GETTEXTLEN, IntPtr.Zero, IntPtr.Zero);
                        StringBuilder itemText = new StringBuilder();
                        SendMessage(hwndListBox, LB_GETTEXT, selectedItem, itemText);
                        selectedText.Text = itemText.ToString();
                        handled = true;
                        break;
                }
            }
            return IntPtr.Zero;
        }

        private void AppendText(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(txtAppend.Text))
            {
                SendMessage(hwndListBox, LB_ADDSTRING, IntPtr.Zero, txtAppend.Text);
            }
            itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            numItems.Text = "" + itemCount.ToString();
        }
        private void DeleteText(object sender, EventArgs args)
        {
            selectedItem = SendMessage(listControl.hwndListBox, LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
            if (selectedItem != -1) //check for selected item
            {
                SendMessage(hwndListBox, LB_DELETESTRING, (IntPtr)selectedItem, IntPtr.Zero);
            }
            itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            numItems.Text = "" + itemCount.ToString();
        }

        internal const int
          LBN_SELCHANGE = 0x00000001,
          WM_COMMAND = 0x00000111,
          LB_GETCURSEL = 0x00000188,
          LB_GETTEXTLEN = 0x0000018A,
          LB_ADDSTRING = 0x00000180,
          LB_GETTEXT = 0x00000189,
          LB_DELETESTRING = 0x00000182,
          LB_GETCOUNT = 0x0000018B;

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void ControlHostElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (listControl == null) return;

            MoveWindow(listControl.hwndListBox, 0, 0, Convert.ToInt32(ControlHostElement.ActualWidth), Convert.ToInt32(ControlHostElement.ActualHeight), true);
            ActivateUnityWindow();
        }

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        private void ActivateUnityWindow()
        {
            SendMessage(listControl.hwndListBox, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);


        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
                                               int msg,
                                               IntPtr wParam,
                                               IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
                                               int msg,
                                               int wParam,
                                               [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hwnd,
                                                  int msg,
                                                  IntPtr wParam,
                                                  string lParam);
    }
}
