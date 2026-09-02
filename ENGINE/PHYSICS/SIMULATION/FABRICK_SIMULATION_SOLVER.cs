using System.Numerics;
using Fabrick.ENGINE.MATH;

namespace Fabrick.ENGINE.PHYSICS
{
    public sealed class SIMULATION_SOLVER
    {
        public static readonly float MinBodySize = 0.01f * 0.01f;
        public static readonly float MaxBodySize = 1024f * 1024f;

        public static readonly float MinDensity = 0.5f;
        public static readonly float MaxDensity = 21.4f;

        public static readonly int MinIterations = 1;
        public static readonly int MaxIterations = 128;

        private Vector2 Gravity;
        private List<RIGID_BODY> BodyList;

        public int BodyCount
        {
            get { return this.BodyList.Count; }
        }

        public SIMULATION_SOLVER()
        {
            this.Gravity = new Vector2(0f, -9.81f);
            this.BodyList = new List<RIGID_BODY>();
        }

        public void AddBody(RIGID_BODY Body)
        {
            this.BodyList.Add(Body);
        }

        public bool RemoveBody(RIGID_BODY Body)
        {
            return this.BodyList.Remove(Body);
        }

        public bool GetBody(int Index, out RIGID_BODY Body)
        {
            Body = null;

            if (Index < 0 || Index >= this.BodyList.Count)
            {
                return false;
            }
            Body = this.BodyList[Index];
            return true;
        }

        public void Step(float Time, int Iterations)
        {
            Iterations = (int)FABRICK_MATH.Clamp(Iterations, MinIterations, MaxIterations);

            for (int It = 0; It < Iterations; It++)
            {
                for (int i = 0; i < this.BodyList.Count; i++)
                {
                    this.BodyList[i].Step(Time, this.Gravity, Iterations);
                }

                // Collision Step
                for (int i = 0; i < this.BodyList.Count - 1; i++)
                {
                    RIGID_BODY BodyA = this.BodyList[i];
                }
            }
        }
    }
}