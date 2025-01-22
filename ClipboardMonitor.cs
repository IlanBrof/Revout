using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Revout
{
    public class ClipboardMonitor
    {
        private MainWindow _mainWindow;

        public ClipboardMonitor(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            ClipboardNotification.ClipboardUpdate += OnClipboardUpdate;
            ClipboardNotification.Start();
        }

        private void OnClipboardUpdate(object sender, EventArgs e)
        {
            _mainWindow.Dispatcher.Invoke(_mainWindow.UpdateClipboardTextBox);
        }

        public void Dispose()
        {
            ClipboardNotification.ClipboardUpdate -= OnClipboardUpdate;
        }
    }

    public static class ClipboardNotification
    {
        public static event EventHandler ClipboardUpdate;

        private static NotificationForm _form = new NotificationForm();

        public static void Start()
        {
            _form.Visible = false;
        }

        private class NotificationForm : Form
        {
            public NotificationForm()
            {
                NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
                {
                    ClipboardUpdate?.Invoke(null, EventArgs.Empty);
                }
                base.WndProc(ref m);
            }
        }

        private static class NativeMethods
        {
            public const int WM_CLIPBOARDUPDATE = 0x031D;
            public static IntPtr HWND_MESSAGE = new IntPtr(-3);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        }
    }
}
