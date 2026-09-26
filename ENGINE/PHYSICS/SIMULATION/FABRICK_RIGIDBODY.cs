
/*
References:
https://github.com/twobitcoder101/FlatPhysics/blob/main/FlatBody.cs
*/
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;

namespace Fabrick.ENGINE.PHYSICS
{
    public enum SHAPE_TYPE
    {
        Circle = 0,
        Box = 1
    }
    public class RIGID_BODY
    {
        private Vector2 position;
        private Vector2 linearVelocity;
        private float rotation;
        private float rotationVelocity;

        private Vector2 Force;

        public readonly float Density;
        public readonly float Mass;
        public readonly float InverseMass;
        public readonly float Restitution;
        public readonly float Area;

        public readonly bool IsStatic;

        public readonly float Radius;
        public readonly float Width;
        public readonly float Height;

        private Vector2[] Vertices;
        public readonly int[]Triangles;
        private Vector2[] TransformedVertices;
        private AABB aabb;

        private bool TransformUpdateRequired;
        private bool AABBUpdateRequired;

        public readonly SHAPE_TYPE ShapeType;

        public Vector2 Position
        {
            get { return this.position; }
        }
        public Vector2 LinearVelocity
        {
            get { return this.linearVelocity; }
            internal set { this.linearVelocity = value; }
        }

        private RIGID_BODY(Vector2 Position,
                            float Density,
                            float Mass,
                            float Restitution,
                            float Area,
                            bool IsStatic,
                            float Radius,
                            float Width,
                            float Height,
                            SHAPE_TYPE ShapeType)
        {
            this.position = Position;
            this.linearVelocity = Vector2.Zero;
            this.rotation = 0.0f;
            this.rotationVelocity = 0.0f;

            this.Force = Vector2.Zero;

            this.Density = Density;
            this.Mass = Mass;
            this.Restitution = Restitution;
            this.Area = Area;

            this.IsStatic = IsStatic;
            this.Radius = Radius;
            this.Width = Width;
            this.Height = Height;
            this.ShapeType = ShapeType;

            if (!this.IsStatic)
            {
                this.InverseMass = 1f / this.Mass;
            }
            else
            {
                this.InverseMass = 0f;
            }

            if (this.ShapeType is SHAPE_TYPE.Box)
            {
                this.Vertices = RIGID_BODY.CreateBoxVertices(this.Width, this.Height);
                this.Triangles = RIGID_BODY.CreateBoxTriangles();
                this.TransformedVertices = new Vector2[this.Vertices.Length];
            }
            else
            {
                this.Vertices = null;
                this.Triangles = null;
                this.TransformedVertices = null;
            }

            this.TransformUpdateRequired = true;
            this.AABBUpdateRequired = true;
        }

        private static Vector2[] CreateBoxVertices(float Width, float Height)
        {
            float left = -Width / 2;
            float right = left + Width;
            float bottom = -Height / 2;
            float top = bottom + Height;

            Vector2[] Vertices = new Vector2[4];
            Vertices[0] = new Vector2(left, top);
            Vertices[1] = new Vector2(right, top);
            Vertices[2] = new Vector2(right, bottom);
            Vertices[3] = new Vector2(left, bottom);

            return Vertices;
        }

        private static int[] CreateBoxTriangles()
        {
            int[] Triangles = new int[6];
            Triangles[0] = 0;
            Triangles[1] = 1;
            Triangles[2] = 2;
            Triangles[3] = 0;
            Triangles[4] = 2;
            Triangles[5] = 3;
            return Triangles;
        }

        public Vector2[] GetTransformedVertices()
        {
            if (this.TransformUpdateRequired)
            {
                FABRICK_TRANSFORM transform = new FABRICK_TRANSFORM(this.Position, new Vector2(0, 0), this.rotation);

                for (int i = 0; i < this.Vertices.Length; i++)
                {
                    Vector2 v = this.Vertices[i];
                    this.TransformedVertices[i] = VECTOR_MATH.Transform(v, transform);
                }
            }

            this.TransformUpdateRequired = false;
            return this.TransformedVertices;
        }

        public AABB GetAABB()
        {
            if (this.AABBUpdateRequired)
            {
                float MinX = float.MaxValue;
                float MinY = float.MaxValue;
                float MaxX = float.MinValue;
                float MaxY = float.MinValue;

                if (this.ShapeType is SHAPE_TYPE.Box)
                {
                    Vector2[] vertices = this.GetTransformedVertices();
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        Vector2 v = vertices[i];

                        if (v.X < MinX) { MinX = v.X; }
                        if (v.X > MaxX) { MaxX = v.X; }
                        if (v.Y < MinY) { MinY = v.Y; }
                        if (v.Y > MaxY) { MaxY = v.Y; }
                    }
                }else if(this.ShapeType is SHAPE_TYPE.Circle)
                {
                    MinX = this.position.X - this.Radius;
                    MinY = this.position.Y - this.Radius;
                    MaxX = this.position.X + this.Radius;
                    MaxY = this.position.Y + this.Radius;
                }
                else
                {
                    FABRICK_DEBUG.Log("Unknown Shape Type.");
                }
                this.aabb = new AABB(MinX, MinY, MaxX, MaxY);
            }
            
            this.AABBUpdateRequired = false;
            return this.aabb;
        }
        
        internal void Step(float time, Vector2 Gravity, int Iterations)
        {
            if (this.IsStatic)
            {
                return;
            }
            
            time /= (float)Iterations;

            this.linearVelocity += Gravity * time;
            this.position += this.LinearVelocity * time;
            this.rotation += this.rotationVelocity * time;

            this.Force = Vector2.Zero;
            this.TransformUpdateRequired = true;
            this.AABBUpdateRequired = true;
        }

        public void Move(Vector2 Amount)
        {
            this.position += Amount;
            this.TransformUpdateRequired = true;
            this.AABBUpdateRequired = true;
        }

        public void MoveTo(Vector2 Position)
        {
            this.position = Position;
            this.TransformUpdateRequired = true;
            this.AABBUpdateRequired = true;
        }

        public void Rotate(float Amount)
        {
            this.rotation += Amount;
            this.TransformUpdateRequired = true;
            this.AABBUpdateRequired = true;
        }

        public void AddForce(Vector2 Amount)
        {
            this.Force = Amount;
        }

        public static bool CreateCircleBody(float Radius,
                                            Vector2 Position,
                                            float Density,
                                            bool IsStatic,
                                            float Restitution,
                                            out RIGID_BODY Body)
        {
            Body = null;

            float Area = Radius * Radius * MathF.PI;

            Restitution = FABRICK_MATH.Clamp(Restitution, 0f, 1f);

            float Mass = Area * Density;

            Body = new RIGID_BODY(Position, Density, Mass, Restitution, Area, IsStatic, Radius, 0, 0, SHAPE_TYPE.Circle);
            return true;
        }

        public static bool CreateBoxBody(float Width,
                                        float Height,
                                        Vector2 Position,
                                        float Density,
                                        bool IsStatic,
                                        float Restitution,
                                        out RIGID_BODY Body)
        {
            Body = null;

            float Area = Width * Height;

            Restitution = FABRICK_MATH.Clamp(Restitution, 0f, 1f);

            float Mass = Area * Density;

            Body = new RIGID_BODY(Position, Density, Mass, Restitution, Area, IsStatic, 0f, Width, Height, SHAPE_TYPE.Box);
            return true;
        }
    }
}