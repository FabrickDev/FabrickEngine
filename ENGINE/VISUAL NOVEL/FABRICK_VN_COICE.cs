using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public class FABRICK_VISUAL_NOVEL_COICE : FABRICK_ENTITY
    {
        private string CacheCategory = "VNCoice";
        public FABRICK_VISUAL_NOVEL_COICE(string Name, int Id)
        {
            this.Category = "VisualNovelCoice";
            this.Name = Name;
            this.Id = Id;
            CacheCategory = string.Join(CacheCategory, "@", Name);
        }

        private enum CoiceButtonType
        {
            BasicShape,
            Texture
        }
        internal Box CoiceBasicShape = new(Vector2.Zero, new Vector2(900 / 2, 100 / 2), new Vector2(900, 100));
        internal Dictionary<string, Box> CoiceList = new();

        private FABRICK_COLOR BoxColor = new(0xFF, 0xF3, 0xD1, 255);

        private float DistanceEachButton = 75f;
        public override void Initialize()
        {
            // Initialize Basic Shape
            
        }
        public override void Update()
        {
            
        }
        public override void Render()
        {
            if(FABRICK_SCENARIO_MANAGER.CoiceList.Count == 0) return;
            CoiceList.Clear();
            int ButtonCount = FABRICK_SCENARIO_MANAGER.CoiceList.Count;
            float TotalLength = ButtonCount * CoiceBasicShape.GetSize().Y + (ButtonCount - 1) * DistanceEachButton;
            float StartPos = Transform.GetPosition().Y - TotalLength / 2f;
            int i = 0;
            foreach (string ThisCoice in FABRICK_SCENARIO_MANAGER.CoiceList)
            {
                float BoxPosY = StartPos + CoiceBasicShape.GetSize().Y / 2 + i * (CoiceBasicShape.GetSize().Y + DistanceEachButton);
                CoiceList.Add(ThisCoice, new(new Vector2(Transform.GetPosition().X, BoxPosY), CoiceBasicShape.GetOffset(), CoiceBasicShape.GetSize()));
                i++;
            }

            // Input Response
            foreach (string ThisKey in CoiceList.Keys)
            {
                if (FABRICK_COLLISIONS.MouseClickToBox_Pressed(CoiceList[ThisKey], SDL3.SDL.MouseButtonFlags.Left))
                {
                    FABRICK_SCENARIO_MANAGER.SelectedCoice = ThisKey;
                }
            }
            foreach (string ThisKey in CoiceList.Keys)
            {
                //FABRICK_DEBUG.Log($"Pos: {CoiceList[ThisKey].GetPos()}, Size: {ThisBox.GetSize()}");
                FABRICK_VISUALIZE_SHAPE.Visualize(CoiceList[ThisKey], BoxColor, true, true, true);
                FABRICK_DRAW_TEXT.DrawTextAdvanced(ThisKey, FABRICK_DRAW_TEXT.GetDefaultFontName(), CoiceList[ThisKey].GetActualPos() - new Vector2(0, CoiceList[ThisKey].GetOffset().Y / 2), 5F, FABRICK_COLOR.Black(), Algin.Center, out _, 0, 0, true, true, true, CacheCategory);
                //FABRICK_DRAW_SHAPE.DrawFillRect_UI(ThisBox.GetActualPos(), ThisBox.GetSize(), BoxColor);
            }
        }

        public void SetCoiceTexture(string Texture)
        {
            
        }
    }
}