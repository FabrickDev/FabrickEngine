using System.Numerics;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public enum FABRICK_CAMERA_ALIGN
    {
        TopLeft,
        TopCenter,
        TopRight,
        MidLeft,
        MidCenter,
        MidRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    public class FABRICK_CAMERA
    {
        private Vector2 Size = new();
        private Vector2 Pos = new();
        private Vector2 CamPos = new();
        private float CamZoom = 1f;
        private float CamAngle = new();
        private FABRICK_CAMERA_ALIGN Align = FABRICK_CAMERA_ALIGN.MidCenter;

        public FABRICK_CAMERA(Vector2 Pos, Vector2 Size, FABRICK_CAMERA_ALIGN Align = FABRICK_CAMERA_ALIGN.MidCenter, float Zoom = 1, float Angle = 0)
        {
            SetCameraSize(Size);
            SetCameraPos(Pos);
            SetCameraAlign(Align);
            CamZoom = Zoom;
            CamAngle = Angle;
        }
        public void SetCameraSize(Vector2 Size)
        {
            this.Size = Size;
        }
        public void SetCameraPos(Vector2 Pos)
        {
            this.Pos = Pos;
        }
        public Vector2 GetCamPos()
        {
            return CamPos;
        }
        public Vector2 GetCamSize()
        {
            return Size;
        }
        public Vector2 GetActualCamPos()
        {
            return Pos;
        }
        public void SetCamZoom(float Zoom)
        {
            CamZoom = Zoom;
        }
        public float GetCamZoom()
        {
            return CamZoom;
        }
        public void SetCamAngle(float Angle)
        {
            CamAngle = Angle;
        }
        public float GetCamAngle()
        {
            return CamAngle;
        }
        public void Update()
        {
            SetCameraUpdate(this.Align);
        }
        public void SetCameraAlign(FABRICK_CAMERA_ALIGN Align)
        {
            this.Align = Align;
        }
        public void SetCameraUpdate(FABRICK_CAMERA_ALIGN Align)
        {
            switch (Align)
            {
                case FABRICK_CAMERA_ALIGN.TopLeft: 
                    CamPos = Pos;     
                break;
                case FABRICK_CAMERA_ALIGN.TopCenter:
                    CamPos = new Vector2(Pos.X - (Size.X / 2), Pos.Y);
                break;
                case FABRICK_CAMERA_ALIGN.TopRight:
                    CamPos = new Vector2(Pos.X - (Size.X), Pos.Y);
                break;

                case FABRICK_CAMERA_ALIGN.MidLeft:
                    CamPos = new Vector2(Pos.X, Pos.Y - (Size.Y / 2));
                break;
                case FABRICK_CAMERA_ALIGN.MidCenter:
                    CamPos = new Vector2(Pos.X - (Size.X / 2), Pos.Y - (Size.Y / 2));
                break;
                case FABRICK_CAMERA_ALIGN.MidRight:
                    CamPos = new Vector2(Pos.X - (Size.X), Pos.Y - (Size.Y / 2));
                break;

                case FABRICK_CAMERA_ALIGN.BottomLeft:
                    CamPos = new Vector2(Pos.X, Pos.Y - (Size.Y));
                break;
                case FABRICK_CAMERA_ALIGN.BottomCenter:
                    CamPos = new Vector2(Pos.X - (Size.X / 2), Pos.Y - (Size.Y));
                break;
                case FABRICK_CAMERA_ALIGN.BottomRight:
                    CamPos = new Vector2(Pos.X - (Size.X), Pos.Y - (Size.Y));
                break;
            }
        } 
    }

    public class FABRICK_CAMERA_MANAGER
    {
        private static FABRICK_ENGINE? ThisEngine;
        public static Dictionary<string, FABRICK_CAMERA> CurrentCameraList = new Dictionary<string, FABRICK_CAMERA>();
        private static string CurrentCamera = "";
        public static void Initialize(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
            FABRICK_CAMERA DefaultCamera = new FABRICK_CAMERA(Vector2.Zero, ThisEngine.GetLogicalResolution(), FABRICK_CAMERA_ALIGN.MidCenter);
            AssignCamera("Default", DefaultCamera);
            CurrentCamera = "Default";
        }
        public static void SetCurrentCamera(string Name)
        {
            if (CurrentCameraList.ContainsKey(Name))
            {
                CurrentCamera = Name;
                FABRICK_DEBUG.Log($"[CAMERA_MANAGER] Set Current Camera to: {Name}");
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[CAMERA_MANAGER] Cannot set current camera to: {Name} because theres no that camera!");
            }
        }
        public static string GetCurrentCamera()
        {
            return CurrentCamera;
        }
        public static void AssignCamera(string Name, FABRICK_CAMERA Camera)
        {
            if (CurrentCameraList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[CAMERA_MANAGER] Cannot assign new camera named: {Name} because name already used!!");
                return;
            }
            CurrentCameraList.Add(Name, Camera);
            FABRICK_DEBUG.Log($"[CAMERA_MANAGER] Added new camera to list: {Name}");
        }
        public static void Update(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
            foreach (string ThisKey in CurrentCameraList.Keys)
            {
                CurrentCameraList[ThisKey].Update();
            }
            // if (FABRICK_INPUT.IsKeyPressed(SDL.Scancode.C))
            // {
            //     foreach (string ThisCam in CurrentCameraList.Keys)
            //     {
            //         FABRICK_DEBUG.Log("[CAMERA_MANAGER] Camera: " + ThisCam);
            //     }
            // }
        }
        public static void SetCamPos(Vector2 Pos)
        {
            CurrentCameraList[CurrentCamera].SetCameraPos(Pos);
        }
        public static void SetCameraZoom(float Zoom)
        {
            CurrentCameraList[CurrentCamera].SetCamZoom(Zoom);
        }
        public static void SetCameraAngle(float Angle)
        {
            CurrentCameraList[CurrentCamera].SetCamAngle(Angle);
        }
        public static Vector2 GetCamViewport()
        {
            return CurrentCameraList[CurrentCamera].GetCamPos();
        }
        public static Vector2 GetCamPos()
        {
            return CurrentCameraList[CurrentCamera].GetCamPos();
        }
        public static Vector2 GetActualCamPos()
        {
            return CurrentCameraList[CurrentCamera].GetActualCamPos();
        }
        public static Vector2 GetCamSize()
        {
            return CurrentCameraList[CurrentCamera].GetCamSize();
        }
        public static float GetCamZoom()
        {
            return CurrentCameraList[CurrentCamera].GetCamZoom();
        }
        public static float GetCamAngle()
        {
            return CurrentCameraList[CurrentCamera].GetCamAngle();
        }
        public static float GetCamAngleRad()
        {
            return FABRICK_MATH.AngleToRad(CurrentCameraList[CurrentCamera].GetCamAngle());
        }
        public static Vector2 ScreenToWorldPos(Vector2 Pos)
        {
            Vector2 D = new Vector2(Pos.X - (GetCamSize().X / 2), Pos.Y - (GetCamSize().Y / 2));
            float CosA = MathF.Cos(GetCamAngleRad());
            float SinA = MathF.Sin(GetCamAngleRad());

            Vector2 REL = new Vector2((D.X * CosA) - (D.Y * SinA), (D.X * SinA) + (D.Y * CosA));

            Vector2 Z = REL / GetCamZoom();
            Vector2 WorldPos = Z + GetActualCamPos();

            return WorldPos;
        }
        public static Vector2 ScreenToWorkdSize(Vector2 Size)
        {
            return Size / GetCamZoom();
        }
        public static float ScreenToWorldAngle(float Angle)
        {
            return Angle + GetCamAngle();
        }
        public static FABRICK_TRANSFORM WorldToScreen(Vector2 Pos, Vector2 Size, float Angle, float Parallax = 1f)
        {
            Vector2 REL = new Vector2((Pos.X - GetActualCamPos().X * Parallax) * GetCamZoom(), 
                                        (Pos.Y - GetActualCamPos().Y * Parallax) * GetCamZoom());

            float CosA = MathF.Cos(GetCamAngleRad());
            float SinA = MathF.Sin(-GetCamAngleRad());

            Vector2 FINAL_POS = new Vector2(((REL.X * CosA) - (REL.Y * SinA)) + (GetCamSize().X / 2), 
                                            ((REL.X * SinA) + (REL.Y * CosA)) + (GetCamSize().Y / 2));
            Vector2 FINAL_SIZE = new Vector2(Size.X * GetCamZoom(), Size.Y * GetCamZoom());
            float FINAL_ANGLE = Angle - GetCamAngle();

            return new FABRICK_TRANSFORM(FINAL_POS, FINAL_SIZE, FINAL_ANGLE);
        }
        public static FABRICK_TRANSFORM WorldToScreen2(Vector2 Pos, Vector2 Size, float Angle)
        {
            Vector2 REL = new Vector2((Pos.X - GetActualCamPos().X) * GetCamZoom(), 
                                        (Pos.Y - GetActualCamPos().Y) * GetCamZoom());

            float CosA = MathF.Cos(FABRICK_MATH.AngleToRad(GetCamAngle() - Angle));
            float SinA = MathF.Sin(-FABRICK_MATH.AngleToRad(GetCamAngle() - Angle));

            Vector2 FINAL_POS = new Vector2(((REL.X * CosA) - (REL.Y * SinA)) + (GetCamSize().X / 2), 
                                            ((REL.X * SinA) + (REL.Y * CosA)) + (GetCamSize().Y / 2));
            Vector2 FINAL_SIZE = new Vector2(Size.X * GetCamZoom(), Size.Y * GetCamZoom());
            float FINAL_ANGLE = -GetCamAngle() + Angle;

            return new FABRICK_TRANSFORM(FINAL_POS, FINAL_SIZE, FINAL_ANGLE);
        }
        public static FABRICK_TRANSFORM WorldToScreen(Vector2 Pos, Vector2 Size)
        {
            return WorldToScreen(Pos, Size, 0);
        }
        public static FABRICK_TRANSFORM WorldToScreen(Vector2 Pos)
        {
            return WorldToScreen(Pos, Vector2.Zero, 0);
        }
        public static Vector2 GetMinWorldBound()
        {
            return ScreenToWorldPos(Vector2.Zero);
        }
        public static Vector2 GetMaxWorldBound()
        {
            if (ThisEngine is not null)
            {
                return ScreenToWorldPos(ThisEngine.GetLogicalResolution());
            }
            return Vector2.Zero;
        }
    }
}