using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Win32Api;

namespace WpfWin32HostSample
{
    class ControlHost : HwndHost
    {
        public IntPtr hwndListBox
        {
            get { return hwndControl; }
        }

        IntPtr hwndControl;
        IntPtr hwndHost;
        private Process process;
        int hostHeight, hostWidth;

        internal const int
          WS_CHILD = 0x40000000,
          WS_VISIBLE = 0x10000000,
          LBS_NOTIFY = 0x00000001,
          HOST_ID = 0x00000002,
          LISTBOX_ID = 0x00000001,
          WS_VSCROLL = 0x00200000,
          WS_BORDER = 0x00800000;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        public ControlHost(double height, double width)
        {
            hostHeight = (int)height;
            hostWidth = (int)width;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            hwndControl = IntPtr.Zero;
            hwndHost = IntPtr.Zero;

            hwndHost = CreateWindowEx(0, "static", "",
                                      WS_CHILD | WS_VISIBLE,
                                      0, 0,
                                      hostWidth, hostHeight,
                                      hwndParent.Handle,
                                      (IntPtr)HOST_ID,
                                      IntPtr.Zero,
                                      0);

            //hwndControl = CreateWindowEx(0, "listbox", "",
            //                              WS_CHILD | WS_VISIBLE | LBS_NOTIFY
            //                                | WS_VSCROLL | WS_BORDER,
            //                              0, 0,
            //                              hostWidth, hostHeight,
            //                              hwndHost,
            //                              (IntPtr)LISTBOX_ID,
            //                              IntPtr.Zero,
            //                              0);

            try
            {
                process = new Process();
                process.StartInfo.FileName = "Game.exe";
                process.StartInfo.Arguments = "-parentHWND " + hwndHost + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.WaitForInputIdle();
                // Doesn't work for some reason ?!
                //unityHWND = process.MainWindowHandle;
                EnumChildWindows(hwndHost, WindowEnum, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
            }

            return new HandleRef(this, hwndHost);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            hwndControl = hwnd;
            SendMessage(hwndControl, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
            return 0;
        }

        //PInvoke declarations
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
                                                      string lpszClassName,
                                                      string lpszWindowName,
                                                      int style,
                                                      int x, int y,
                                                      int width, int height,
                                                      IntPtr hwndParent,
                                                      IntPtr hMenu,
                                                      IntPtr hInst,
                                                      [MarshalAs(UnmanagedType.AsAny)] object pvParam);


        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //switch ((WindowsMessages)msg)
            //{
            //    case WindowsMessages.WM_SIZE:
                    //MoveWindow(listControl.hwndListBox, 0, 0, Convert.ToInt32(ControlHostElement.ActualWidth), Convert.ToInt32(ControlHostElement.ActualHeight), true);
                    //Debug.WriteLine("Изменение размера");
            //        break;
            //}

            handled = false;
            return IntPtr.Zero;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);

            try
            {
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (process.HasExited == false)
                    process.Kill();
            }
            catch (Exception)
            {

            }
        }
    }
}
