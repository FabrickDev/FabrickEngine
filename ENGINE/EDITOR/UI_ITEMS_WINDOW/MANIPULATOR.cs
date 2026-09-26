using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.EDITOR.ITEMS
{
    public class MANIPULATOR
    {
        private Vector2 Pos = new();
        private Vector2 GrabOffset = new();
        private bool IsGrabXPos, IsGrabYPos, IsGrabXYPos = false;

        private float Length = 90.0f;
        private float RecThickness = 5.0f;
        private float TrisRadius = 15.0f;
        private float XYAxisSize = 25.0f;
        private float CollisionRange = 15.0f;

        private Box? CollisionXAxis, CollisionYAxis, CollisionXYAxis;
        private bool CollisionIsNotNull = false;

        private FABRICK_COLOR ColorX = new FABRICK_COLOR(165, 0, 0, 255);
        private FABRICK_COLOR ColorY = new FABRICK_COLOR(0, 165, 0, 255);
        private FABRICK_COLOR ColorXY = new FABRICK_COLOR(0, 0, 165, 255);
        private FABRICK_COLOR ColorOnFocus = new FABRICK_COLOR(0, 225, 225, 255);

        private bool IsActive = false;

        private FABRICK_ENTITY? ThisEntity;

        public void Initialize()
        {
            CollisionXAxis = new Box(Vector2.Zero, new Vector2(0, CollisionRange / 2), new Vector2(Length, CollisionRange));
            CollisionYAxis = new Box(Vector2.Zero, new Vector2(CollisionRange / 2, Length), new Vector2(CollisionRange, Length));
            CollisionXYAxis = new Box(Vector2.Zero, Vector2.Zero, new Vector2(XYAxisSize));
        }
        public void Update()
        {
            IsActive = false;
            ThisEntity = null;

            if (CollisionXAxis != null && CollisionYAxis != null && CollisionXYAxis != null)
            {
                CollisionIsNotNull = true;
                CollisionXAxis.SetPos(Pos);
                CollisionYAxis.SetPos(Pos);
                CollisionXYAxis.SetPos(new Vector2(Pos.X + Length - XYAxisSize, Pos.Y - Length));
            }
        }
        public FABRICK_TRANSFORM UpdateEntity(FABRICK_TRANSFORM EntityTransform)
        {
            if (IsGrabXPos || IsGrabYPos || IsGrabXYPos)
            {
                EntityTransform.SetPosition(this.Pos);
            }

            return EntityTransform;
        }
        public void Render()
        {
            if (IsActive)
            {
                // I know this is weird but I have to put any update in this function
                OnMoveHandle();
                FocusHandle();

                DrawXAxis();
                DrawYAxis();
                DrawXYAxis();
            }
        }
        public void SetActiveState(bool State)
        {
            IsActive = State;
        }
        public void SetPosition(Vector2 Pos)
        {
            if (!IsGrabXPos && !IsGrabYPos && !IsGrabXYPos)
            {
                this.Pos = Pos;
            }
        }
        private void FocusHandle()
        {
            // Not Grabbing but Change Color When Near Mouse
            if (!IsGrabXPos && !IsGrabYPos && !IsGrabXYPos && CollisionIsNotNull)
            {
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), CollisionXAxis))
                {
                    ColorX = ColorOnFocus;
                }
                else
                {
                    ColorX = new FABRICK_COLOR(165, 0, 0, 255);
                }
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), CollisionYAxis))
                {
                    ColorY = ColorOnFocus;
                }
                else
                {
                    ColorY = new FABRICK_COLOR(0, 165, 0, 255);
                }
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), CollisionXYAxis))
                {
                    ColorXY = ColorOnFocus;
                }
                else
                {
                    ColorXY = new FABRICK_COLOR(0, 0, 165, 255);
                }
            }

            // Is Grabbing X Axis
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), CollisionXAxis) && FABRICK_INPUT.IsMousePressed(SDL3.SDL.MouseButtonFlags.Left))
            {
                IsGrabXPos = true;
            }
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), CollisionYAxis) && FABRICK_INPUT.IsMousePressed(SDL3.SDL.MouseButtonFlags.Left))
            {
                IsGrabYPos = true;
            }
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), CollisionXYAxis) && FABRICK_INPUT.IsMousePressed(SDL3.SDL.MouseButtonFlags.Left))
            {
                IsGrabXYPos = true;
            }

            // Back All IsStatement
            if (FABRICK_INPUT.IsMouseReleased(SDL3.SDL.MouseButtonFlags.Left))
            {
                IsGrabXPos = false;
                IsGrabYPos = false;
                IsGrabXYPos = false;
            }
        }
        private void OnMoveHandle()
        {
            if (!IsGrabXPos && !IsGrabYPos && !IsGrabXYPos)
            {
                GrabOffset = FABRICK_INPUT.GetMousePos() - Pos;
            }

            if (IsGrabXPos)
            {
                Pos = new Vector2(FABRICK_INPUT.GetMousePos().X - GrabOffset.X, Pos.Y);
            }
            if(IsGrabYPos)
            {
                Pos = new Vector2(Pos.X, FABRICK_INPUT.GetMousePos().Y - GrabOffset.Y);
            }
            if (IsGrabXYPos)
            {
                Pos = FABRICK_INPUT.GetMousePos() - GrabOffset;
            }
        }
        private void DrawXAxis()
        {
            FABRICK_DRAW_SHAPE.DrawFillRect(Pos - new Vector2(0, RecThickness / 2), new Vector2(Length, RecThickness), ColorX);
            FABRICK_DRAW_SHAPE.DrawFillTriangle(new Vector2(Pos.X + Length, Pos.Y), TrisRadius, 0, ColorX);
        }
        private void DrawYAxis()
        {
            FABRICK_DRAW_SHAPE.DrawFillRect(Pos - new Vector2(RecThickness / 2, 0), new Vector2(RecThickness, -Length), ColorY);
            FABRICK_DRAW_SHAPE.DrawFillTriangle(new Vector2(Pos.X, Pos.Y - Length), TrisRadius, -90, ColorY);
        }
        private void DrawXYAxis()
        {
            FABRICK_DRAW_SHAPE.DrawFillRect(new Vector2(Pos.X + Length - XYAxisSize, Pos.Y - Length), new Vector2(XYAxisSize), ColorXY);
        }
    }
}