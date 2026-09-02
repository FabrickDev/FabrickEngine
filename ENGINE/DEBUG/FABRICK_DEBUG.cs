using SDL3;

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
    }
}