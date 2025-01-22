using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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

                await HandleTextConversion();
            }
        }

        private async Task HandleTextConversion()
        {
            string originalClipboardText = System.Windows.Clipboard.GetText();
            if (string.IsNullOrEmpty(originalClipboardText)) return;

            string correctedText = null;
            
            // Check if text contains Hebrew characters
            if (ContainsHebrew(originalClipboardText))
            {
                correctedText = ConvertEnglishTypedAsHebrew(originalClipboardText);
            }
            else
            {
                correctedText = _mainWindow.ConvertEnglishToHebrew(originalClipboardText);
            }

            if (correctedText != null && correctedText != originalClipboardText)
            {
                System.Windows.Clipboard.SetText(correctedText);
                SendKeys.SendWait("^v");
                System.Windows.Clipboard.SetText(originalClipboardText);

                await _mainWindow.ShowHoverBar();
            }
        }

        private bool ContainsHebrew(string text)
        {
            return text.Any(c => c >= 0x0590 && c <= 0x05FF);
        }

        private bool ContainsHebrewKeyboardChars(string text)
        {
            // Hebrew keyboard layout characters that would represent English letters
            string hebrewKeyboardChars = "קראטוןםפשדגכעיחלךףזסבהנמצתץ";
            return text.Any(c => hebrewKeyboardChars.Contains(c));
        }

        private string ConvertHebrewTypedAsEnglish(string text)
        {
            // Existing conversion logic
            // ... 
            return text; // Placeholder return, actual implementation needed
        }

        private string ConvertEnglishTypedAsHebrew(string text)
        {
            Dictionary<char, char> hebrewToEnglish = new Dictionary<char, char>
            {
                {'ש', 'a'}, {'נ', 'b'}, {'ב', 'c'}, {'ג', 'd'}, {'ק', 'e'}, {'כ', 'f'}, {'ע', 'g'}, {'י', 'h'},
                {'ן', 'i'}, {'ח', 'j'}, {'ל', 'k'}, {'ך', 'l'}, {'צ', 'm'}, {'מ', 'n'}, {'ם', 'o'}, {'פ', 'p'},
                {'/', 'q'}, {'ר', 'r'}, {'ד', 's'}, {'א', 't'}, {'ו', 'u'}, {'ה', 'v'}, {'\'', 'w'}, {'ס', 'x'},
                {'ט', 'y'}, {'ז', 'z'}
            };

            StringBuilder result = new StringBuilder();
            foreach (char c in text)
            {
                if (hebrewToEnglish.ContainsKey(c))
                    result.Append(hebrewToEnglish[c]);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        public void Dispose()
        {
            _globalHook.Dispose();
        }
    }
}
