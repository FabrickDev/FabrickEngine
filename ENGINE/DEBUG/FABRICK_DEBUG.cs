/*
    REFERENCES:
    1. https://www.csharp-examples.net/inputbox/
    2. https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.textboxbase.wordwrap?view=windowsdesktop-10.0
*/

using SDL3;
using System;

namespace Fabrick.ENGINE.DEBUG
{
    public class FABRICK_DEBUG
    {
        public static nint window;
        public static void SimpleMessageError(string message)
        {
            SDL.ShowSimpleMessageBox(SDL.MessageBoxFlags.Error, "Fabrick Dev Error!!", message, window);
            Log("[ERROR]" + message);
        }
        public static void SimpleMessageWarning(string message)
        {
            SDL.ShowSimpleMessageBox(SDL.MessageBoxFlags.Warning, "Fabrick Dev Warning!!", message, window);
            Log("[WARNING] " + message);
        } 
        public static void SimpleMessageInfo(string message)
        {
            SDL.ShowSimpleMessageBox(SDL.MessageBoxFlags.Information, "Fabrick Dev Information!!", message, window);
            Log("[INFO] " + message);
        } 
        public static void Log(string message)
        {
            SDL.Log(message);
        }
        public static string GetTextConsole(string message)
        {
            SDL.Log("[INPUT] " + message + ": ");
            string? input = Console.ReadLine();
            if (input is null)
            {
                SimpleMessageError("Ya hafta fill it propperlie matey!!");
                return "";
            }
            else
            {
                return input;
            }
        }
        public static string DialogueInputText(string title, string promptText, string Value, bool Multiline = false)
        {
            string val = Value;
            _ = DialogueInputText(title, promptText, ref val, Multiline);
            return val;
        }
        #if WINDOWS
        public static bool DialogueInputText(string title, string promptText, ref string value, bool Multiline = false)
        {
            if (InputText(title, promptText, ref value, Multiline) == DialogResult.OK)
            {
                return true;
            }
            return false;
        }
        public static DialogResult InputText(string title, string promptText, ref string currentText, bool Multiline = false)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            //TextBox Setting
            if (Multiline)
            {
                textBox.Multiline = true;
                textBox.ScrollBars = ScrollBars.Vertical;
                textBox.AcceptsReturn = true;
                textBox.AcceptsTab = true;
                textBox.WordWrap = true;
            }

            form.Text = title;
            label.Text = promptText;
            textBox.Text = currentText;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 60, 620, 200);
            buttonOk.SetBounds(228, 300, 75, 23);
            buttonCancel.SetBounds(309, 300, 120, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(640, 360);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            //form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            currentText = textBox.Text;
            return dialogResult;
        }
        public static bool DialogueBoolInput(string title, string promptText)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            buttonOk.SetBounds(50, 80, 75, 23);
            buttonCancel.SetBounds(160, 80, 120, 23);

            label.AutoSize = true;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(300, 120);
            form.Controls.AddRange(new Control[] { label, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            bool Result = false;

            DialogResult dialogResult = form.ShowDialog();
            if(dialogResult == DialogResult.OK)
            {
                Result = true;
                Log(promptText + ": Agreed");
            }
            else
            {
                Log(promptText + ": Declined");
            }
            return Result;
        }
        #elif ANDORID
        public static bool DialogueInputText(string title, string promptText, ref string value, bool Multiline = false)
        {
            if (InputText(title, promptText, ref value, Multiline) == DialogResult.OK)
            {
                return true;
            }
            return false;
        }
        public static bool InputText(string title, string promptText, ref string currentText, bool Multiline = false)
        {
            return true;
        }
        public static bool DialogueBoolInput(string title, string promptText)
        {
            return true;
        }
        #endif
    }
}