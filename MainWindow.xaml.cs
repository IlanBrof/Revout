using System;
using System.Collections.Generic;
using System.Drawing; // For Icon
using System.Windows;
using System.Windows.Forms; // For NotifyIcon
using System.Windows.Media; // For VisualTreeHelper
using Application = System.Windows.Application; // Prevent conflicts with Forms.Application
using Gma.System.MouseKeyHook; // For global hooks
using System.Runtime.InteropServices; // For clipboard monitoring
using System.Threading.Tasks; // For Task
using System.Windows.Threading; // For Dispatcher

namespace Revout
{
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        private IKeyboardMouseEvents _globalHook;
        private HoverBar _hoverBar;

        public MainWindow()
        {
            InitializeComponent();

            // Set up the NotifyIcon (system tray icon)
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("Resources/icon.ico"), // Ensure you have an .ico file in the Resources folder
                Visible = true,
                Text = "Revout"
            };

            // Set up ContextMenuStrip for the NotifyIcon
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (sender, args) => ShowWindow());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (sender, args) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;

            // Double-click on the icon to open the app
            _notifyIcon.DoubleClick += (sender, args) => ShowWindow();

            // Start the app minimized
            Hide();

            // Set up global keyboard hook
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;

            // Set window position and size
            SetWindowPosition();

            // Initialize the hover bar
            _hoverBar = new HoverBar();

            // Start clipboard monitoring and subscribe to the event
            ClipboardNotification.ClipboardUpdate += OnClipboardUpdate;
            ClipboardNotification.Start();

            // Display the initial clipboard content
            UpdateClipboardTextBox();
        }

        private void SetWindowPosition()
        {
            // Get the working area of the primary screen (excluding taskbar)
            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            // Set the window size
            Width = 500; // Adjust as needed
            Height = 200; // Adjust as needed

            // Check if the system is using RTL layout
            bool isRTL = SystemParameters.MenuDropAlignment;

            if (isRTL)
            {
                // Set the window position to the bottom-left corner for RTL layout
                Left = workingArea.Left;
            }
            else
            {
                // Set the window position to the bottom-right corner for LTR layout
                Left = workingArea.Right - Width;
            }

            Top = workingArea.Bottom - Height;

            // Ensure the window is always on top
            Topmost = true;
        }

        private async void GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Check if Ctrl+Y is pressed
            if (e.Control && e.KeyCode == Keys.Y)
            {
                e.Handled = true; // Suppress the default behavior

                // Simulate Ctrl+C to copy the selected text
                SendKeys.SendWait("^c");

                // Wait a moment for the clipboard to update
                await Task.Delay(100);

                // Check if text is selected
                if (System.Windows.Clipboard.ContainsText())
                {
                    // Get the selected text
                    string selectedText = System.Windows.Clipboard.GetText();

                    // Convert the mistyped text from English to Hebrew
                    string correctedText = ConvertEnglishToHebrew(selectedText);

                    // Save the current clipboard content
                    string originalClipboardText = System.Windows.Clipboard.GetText();

                    // Replace the mistyped text with the corrected text
                    System.Windows.Clipboard.SetText(correctedText);

                    // Simulate Ctrl+V to paste the corrected text
                    SendKeys.SendWait("^v");

                    // Restore the original clipboard content
                    System.Windows.Clipboard.SetText(originalClipboardText);

                    // Show the hover bar
                    await ShowHoverBar();
                }
            }
        }

        private string ConvertEnglishToHebrew(string text)
        {
            // Define the mapping from English to Hebrew characters
            Dictionary<char, char> englishToHebrewMap = new Dictionary<char, char>
                {
                    {'a', 'ש'}, {'b', 'נ'}, {'c', 'ב'}, {'d', 'ג'}, {'e', 'ק'}, {'f', 'כ'}, {'g', 'ע'}, {'h', 'י'},
                    {'i', 'ן'}, {'j', 'ח'}, {'k', 'ל'}, {'l', 'ך'}, {'m', 'צ'}, {'n', 'מ'}, {'o', 'ם'}, {'p', 'פ'},
                    {'q', '/'}, {'r', 'ר'}, {'s', 'ד'}, {'t', 'א'}, {'u', 'ו'}, {'v', 'ה'}, {'w', '\''}, {'x', 'ס'},
                    {'y', 'ט'}, {'z', 'ז'}, {'A', 'ש'}, {'B', 'נ'}, {'C', 'ב'}, {'D', 'ג'}, {'E', 'ק'}, {'F', 'כ'},
                    {'G', 'ע'}, {'H', 'י'}, {'I', 'ן'}, {'J', 'ח'}, {'K', 'ל'}, {'L', 'ך'}, {'M', 'צ'}, {'N', 'מ'},
                    {'O', 'ם'}, {'P', 'פ'}, {'Q', '/'}, {'R', 'ר'}, {'S', 'ד'}, {'T', 'א'}, {'U', 'ו'}, {'V', 'ה'},
                    {'W', '\''}, {'X', 'ס'}, {'Y', 'ט'}, {'Z', 'ז'}, {'`', ' '}, {'1', '1'}, {'2', '2'}, {'3', '3'},
                    {'4', '4'}, {'5', '5'}, {'6', '6'}, {'7', '7'}, {'8', '8'}, {'9', '9'}, {'0', '0'}, {'-', '-'},
                    {'=', '='}, {'[', ' '}, {']', ' '}, {'\\', ' '}, {';', ' '}, {'\'', ' '}, {',', ' '}, {'.', ' '},
                    {'/', ' '}, {'~', ' '}, {'!', '!'}, {'@', '@'}, {'#', '#'}, {'$', '$'}, {'%', '%'}, {'^', '^'},
                    {'&', '&'}, {'*', '*'}, {'(', '('}, {')', ')'}, {'_', '_'}, {'+', '+'}, {'{', ' '}, {'}', ' '},
                    {'|', ' '}, {':', ' '}, {'"', ' '}, {'<', ' '}, {'>', ' '}, {'?', ' '}
                };

            // Convert the text
            char[] convertedText = text.ToCharArray();
            for (int i = 0; i < convertedText.Length; i++)
            {
                if (englishToHebrewMap.ContainsKey(convertedText[i]))
                {
                    convertedText[i] = englishToHebrewMap[convertedText[i]];
                }
            }

            return new string(convertedText);
        }

        private async Task ShowHoverBar()
        {
            // Get the working area of the primary screen (excluding taskbar)
            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            // Get the DPI scale factors
            var dpiScaleX = VisualTreeHelper.GetDpi(_hoverBar).DpiScaleX;
            var dpiScaleY = VisualTreeHelper.GetDpi(_hoverBar).DpiScaleY;

            // Ensure the hover bar size is measured and arranged
            _hoverBar.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            _hoverBar.Arrange(new Rect(0, 0, _hoverBar.DesiredSize.Width, _hoverBar.DesiredSize.Height));

            var hoverBarDesiredSize = _hoverBar.DesiredSize;

            // Convert hover bar size to pixels
            double hoverBarWidth = hoverBarDesiredSize.Width * dpiScaleX;
            double hoverBarHeight = hoverBarDesiredSize.Height * dpiScaleY;

            // Check if the system is using RTL layout
            bool isRTL = SystemParameters.MenuDropAlignment;

            // Calculate the left position based on layout direction
            double leftPosition;
            if (isRTL)
            {
                // RTL layout: Align to the bottom-left corner
                leftPosition = workingArea.Left;
            }
            else
            {
                // LTR layout: Align to the bottom-right corner
                leftPosition = workingArea.Right - hoverBarWidth;
            }

            // Calculate the top position (just above the taskbar)
            double topPosition = workingArea.Bottom - hoverBarHeight;

            // Convert positions back to device-independent units
            _hoverBar.Left = leftPosition / dpiScaleX;
            _hoverBar.Top = topPosition / dpiScaleY;

            // Show the hover bar
            _hoverBar.Visibility = Visibility.Visible;
            _hoverBar.Show();

            // Hide the hover bar after 3.5 seconds
            await Task.Delay(3500);

            // Hide the hover bar
            _hoverBar.Visibility = Visibility.Hidden;
            _hoverBar.Hide();
        }

        private void OnClipboardUpdate(object sender, EventArgs e)
        {
            // Update the TextBox on the UI thread
            Dispatcher.Invoke(UpdateClipboardTextBox);
        }

        private void UpdateClipboardTextBox()
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                string clipboardText = System.Windows.Clipboard.GetText();
                ClipboardTextBox.Text = clipboardText;
            }
            else
            {
                ClipboardTextBox.Text = "Clipboard does not contain text.";
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Button Clicked!");
        }

        private void OnMinimizeToTrayClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void ExitApplication()
        {
            // Unsubscribe from the clipboard update event
            ClipboardNotification.ClipboardUpdate -= OnClipboardUpdate;

            _notifyIcon.Dispose();
            _globalHook.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from the clipboard update event
            ClipboardNotification.ClipboardUpdate -= OnClipboardUpdate;

            _notifyIcon.Dispose();
            _globalHook.Dispose();
            base.OnClosed(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // Cancel the close event
            Hide(); // Minimize to system tray instead
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            // Do nothing to prevent window dragging
        }
    }

    public static class ClipboardNotification
    {
        public static event EventHandler ClipboardUpdate;

        private static NotificationForm _form = new NotificationForm();

        public static void Start()
        {
            // Show the form (it's hidden), so it can receive messages
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
