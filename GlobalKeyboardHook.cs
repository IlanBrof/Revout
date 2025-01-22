using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace Revout
{
    public class GlobalKeyboardHook
    {
        private IKeyboardMouseEvents _globalHook;
        private MainWindow _mainWindow;

        public GlobalKeyboardHook(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
        }

        private async void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Y)
            {
                e.Handled = true;
                SendKeys.SendWait("^c");
                await Task.Delay(100);

                if (System.Windows.Clipboard.ContainsText())
                {
                    string selectedText = System.Windows.Clipboard.GetText();
                    string correctedText = _mainWindow.ConvertEnglishToHebrew(selectedText);
                    string originalClipboardText = System.Windows.Clipboard.GetText();

                    System.Windows.Clipboard.SetText(correctedText);
                    SendKeys.SendWait("^v");
                    System.Windows.Clipboard.SetText(originalClipboardText);

                    await _mainWindow.ShowHoverBar();
                }
            }
        }

        public void Dispose()
        {
            _globalHook.Dispose();
        }
    }
}
