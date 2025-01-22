using System;
using System.Drawing;
using System.Windows.Forms;

namespace Revout
{
    public class SystemTrayIcon
    {
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;

        public SystemTrayIcon(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            Initialize();
        }

        private void Initialize()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/icon.ico"),
                Visible = true,
                Text = "Revout"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (sender, args) => _mainWindow.ShowWindow());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (sender, args) => _mainWindow.ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.DoubleClick += (sender, args) => _mainWindow.ShowWindow();
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }
    }
}
