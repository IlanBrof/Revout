using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Revout
{
    public partial class MainWindow : Window
    {
        private SystemTrayIcon _systemTrayIcon;
        private ClipboardMonitor _clipboardMonitor;
        private GlobalKeyboardHook _globalKeyboardHook;
        private HoverBar _hoverBar;
        private const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "Revout";

        public MainWindow()
        {
            InitializeComponent();

            _systemTrayIcon = new SystemTrayIcon(this);
            _clipboardMonitor = new ClipboardMonitor(this);
            _globalKeyboardHook = new GlobalKeyboardHook(this);

            SetWindowPosition();
            _hoverBar = new HoverBar();
            UpdateClipboardTextBox();
            InitializeLaunchOnStartupCheckBox();
        }

        private void SetWindowPosition()
        {
            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            Width = 500;
            Height = 200;
            bool isRTL = SystemParameters.MenuDropAlignment;

            if (isRTL)
            {
                Left = workingArea.Left;
            }
            else
            {
                Left = workingArea.Right - Width;
            }

            Top = workingArea.Bottom - Height;
            Topmost = true;
        }

        public string ConvertEnglishToHebrew(string text)
        {
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

        public async Task ShowHoverBar()
        {
            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            var dpiScaleX = VisualTreeHelper.GetDpi(_hoverBar).DpiScaleX;
            var dpiScaleY = VisualTreeHelper.GetDpi(_hoverBar).DpiScaleY;

            _hoverBar.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            _hoverBar.Arrange(new Rect(0, 0, _hoverBar.DesiredSize.Width, _hoverBar.DesiredSize.Height));

            var hoverBarDesiredSize = _hoverBar.DesiredSize;
            double hoverBarWidth = hoverBarDesiredSize.Width * dpiScaleX;
            double hoverBarHeight = hoverBarDesiredSize.Height * dpiScaleY;
            bool isRTL = SystemParameters.MenuDropAlignment;

            double leftPosition;
            if (isRTL)
            {
                leftPosition = workingArea.Left;
            }
            else
            {
                leftPosition = workingArea.Right - hoverBarWidth;
            }

            double topPosition = workingArea.Bottom - hoverBarHeight;
            _hoverBar.Left = leftPosition / dpiScaleX;
            _hoverBar.Top = topPosition / dpiScaleY;

            _hoverBar.Visibility = Visibility.Visible;
            _hoverBar.Show();

            await Task.Delay(3500);

            _hoverBar.Visibility = Visibility.Hidden;
            _hoverBar.Hide();
        }

        public void UpdateClipboardTextBox()
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

        public void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        public void ExitApplication()
        {
            _clipboardMonitor.Dispose();
            _systemTrayIcon.Dispose();
            _globalKeyboardHook.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            // Do nothing to prevent window dragging
        }

        private void InitializeLaunchOnStartupCheckBox()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, false))
            {
                if (key != null)
                {
                    LaunchOnStartupCheckBox.IsChecked = key.GetValue(AppName) != null;
                }
            }
        }

        private void LaunchOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
            {
                if (key != null)
                {
                    key.SetValue(AppName, $"\"{System.Reflection.Assembly.GetExecutingAssembly().Location}\"");
                }
            }
        }

        private void LaunchOnStartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true))
            {
                if (key != null)
                {
                    key.DeleteValue(AppName, false);
                }
            }
        }

        private void OnLaunchOnStartupPanelClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LaunchOnStartupCheckBox.IsChecked = !LaunchOnStartupCheckBox.IsChecked;
        }
    }
}
