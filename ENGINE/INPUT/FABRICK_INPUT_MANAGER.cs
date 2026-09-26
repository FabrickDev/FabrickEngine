using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.RENDERER;
using SDL3;

/*
References:
https://github.com/raysan5/raylib/blob/c04e57399acfc860a68ac0c11ff6d1c77da9cfda/src/rcore.c#L1822
*/

namespace Fabrick.ENGINE.INPUT
{
    public class FABRICK_INPUT
    {
        private static FABRICK_ENGINE? ThisEngine;
        private static Dictionary<int, bool> PreviousKeyState = new Dictionary<int, bool>();
        private static Dictionary<int, bool> CurrentKeyState = new Dictionary<int, bool>();
        private static Dictionary<int, bool> WasKeyStatePressed = new Dictionary<int, bool>();

        private static int MouseButtonOffset = 0;

        private static Vector2 Scroll = new Vector2();


        public static void EngineUpdate(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
        }
        public static void InitializeKeyState()
        {
            Array enumKey = Enum.GetValues(typeof(SDL.Scancode));
            int _i = 0;
            // Keyboard
            foreach (int i in enumKey)
            {
                PreviousKeyState[i] = false;
                CurrentKeyState[i] = false;
                WasKeyStatePressed[i] = false;
                _i = i;
            }
            MouseButtonOffset = _i;
            // Add mouse button count
            Array enumMouseButton = Enum.GetValues(typeof(SDL.MouseButtonFlags));
            foreach (uint i in enumMouseButton)
            {
                int enumMouseButton_Offset = MouseButtonOffset + (int)i;

                PreviousKeyState[enumMouseButton_Offset] = false;
                CurrentKeyState[enumMouseButton_Offset] = false;
                WasKeyStatePressed[enumMouseButton_Offset] = false;
            }
        }
        
        public static bool IsKeyDown(SDL.Scancode keycode)
        {
            var Key = SDL.GetKeyboardState(out _);
            int ThisKey = (int)keycode;

            if (Key[ThisKey])
            {
                PreviousKeyState[ThisKey] = true;
                CurrentKeyState[ThisKey] = true;
                return true;
            }
            CurrentKeyState[ThisKey] = false;
            return false;
        }
        public static bool IsKeyUp(SDL.Scancode keycode)
        {
            return !CurrentKeyState[(int)keycode];
        }
        public static bool IsKeyPressed(SDL.Scancode keycode)
        {
            if (IsKeyDown(keycode) && WasKeyStatePressed[(int)keycode] == false)
            {
                WasKeyStatePressed[(int)keycode] = true;
                return true;
            }else if (IsKeyUp(keycode))
            {
                WasKeyStatePressed[(int)keycode] = false;
            }
            return false;
        }

        // What Purpose This Function??
        public static bool IsKeyReleased(SDL.Scancode keycode)
        {
            int ThisKey = (int)keycode;

            if (PreviousKeyState[ThisKey] == true && CurrentKeyState[ThisKey] == false)
            {
                PreviousKeyState[ThisKey] = false;
                return true;
            }
            return false;
        }

        public static bool IsMouseDown(SDL.MouseButtonFlags keycode)
        {
            int MouseButtonI = MouseButtonOffset + (int)keycode;
            if (SDL.GetMouseState(out _, out _) == keycode)
            {
                PreviousKeyState[MouseButtonI] = true;
                CurrentKeyState[MouseButtonI] = true;
                return true;
            }
            CurrentKeyState[MouseButtonI] = false;
            return false;
        }
        public static bool IsMouseUp(SDL.MouseButtonFlags keycode)
        {
            return !CurrentKeyState[MouseButtonOffset + (int)keycode];
        }
        public static bool IsMousePressed(SDL.MouseButtonFlags keycode)
        {
            int MouseButtonI = MouseButtonOffset + (int)keycode;
            if (IsMouseDown(keycode) && WasKeyStatePressed[MouseButtonI] == false)
            {
                WasKeyStatePressed[MouseButtonI] = true;
                return true;
            }
            if (CurrentKeyState[MouseButtonI] == false)
            {
                WasKeyStatePressed[MouseButtonI] = false;
            }
            return false;
        }
        public static bool IsMouseReleased(SDL.MouseButtonFlags keycode)
        {
            int MouseButtonI = MouseButtonOffset + (int)keycode;

            if (PreviousKeyState[MouseButtonI] == true && CurrentKeyState[MouseButtonI] == false)
            {
                return true;
            }
            return false;
        }
        public static Vector2 GetMousePos()
        {
            if (ThisEngine is not null)
            {
                Vector2 ActualRes = ThisEngine.GetActualResolution();
                Vector2 LogicalPresen = ThisEngine.GetLogicalPresentationResolution();
                Vector2 Ratio = ThisEngine.GetLogicalResolution() / ThisEngine.GetLogicalPresentationResolution();
                SDL.GetMouseState(out float X, out float Y);

                Vector2 ModifiedPosiiton = new Vector2(X - (ActualRes.X - LogicalPresen.X) / 2, Y - (ActualRes.Y - LogicalPresen.Y) / 2) * Ratio;

                return FABRICK_CAMERA_MANAGER.ScreenToWorldPos(ModifiedPosiiton);
            }
            else
            {
                return Vector2.Zero;
            }
        }
        public static Vector2 GetMouseScroll()
        {
            return Scroll;
        }
        public static void MouseScrollEventHandle(SDL.Event Event)
        {
            Scroll = Vector2.Zero;
            if ((SDL.EventType)Event.Type == SDL.EventType.MouseWheel)
            {
                Scroll.X = Event.Wheel.X;
                Scroll.Y = Event.Wheel.Y;
            }
        }
        public static Vector2 GetMousePos_UI()
        {
            if (ThisEngine is not null)
            {
                Vector2 ActualRes = ThisEngine.GetActualResolution();
                Vector2 LogicalPresen = ThisEngine.GetLogicalPresentationResolution();
                Vector2 Ratio = ThisEngine.GetLogicalResolution() / ThisEngine.GetLogicalPresentationResolution();
                SDL.GetMouseState(out float X, out float Y);

                Vector2 ModifiedPosiiton = new Vector2(X - (ActualRes.X - LogicalPresen.X) / 2, Y - (ActualRes.Y - LogicalPresen.Y) / 2) * Ratio;

                return ModifiedPosiiton;
            }
            else
            {
                return Vector2.Zero;
            }
        }
    }   
}