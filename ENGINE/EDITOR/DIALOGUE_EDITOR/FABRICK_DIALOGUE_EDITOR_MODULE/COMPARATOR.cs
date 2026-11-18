using System.Numerics;
using System.Reflection;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.VISUAL_NOVEL;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR
{
    internal partial class FABRICK_DIALOGUE_EDITOR_MODULE
    {
        int SelectedCompareCondition = -1;
        private void Comparator()
        {
            /*
            Select First Global Variable
            Select Condition
            Select Is Use Custom Value (CompareTo)
            Select Global Variable or Input Yerself 
            */

            Vector2 _Pos = Pos + new Vector2(10, 10 + MoverSize.Y) - new Vector2(SizeModule.X / 2, 0);
            Vector2 _Size = new Vector2(SizeModule.X - 20, 30);
            Vector2 _Pos2 = _Pos + new Vector2(0, _Size.Y) + new Vector2(0, 5);
            Vector2 _Pos3 = _Pos2 + new Vector2(0, _Size.Y) + new Vector2(0, 5);
            Vector2 _Pos4 = _Pos3 + new Vector2(0, _Size.Y) + new Vector2(0, 5);

            // Draw Purpose
            string FirstVariable = "Select Variable..";
            string SecondVariable = "Select Variable or Input";
            string CompareTo = "Compare to..";
            string CompareCondition = "Condition..";
            // Update Above When Available
            if (ModuleData.FirstGlobalVariableName.Length > 0)
            {
                FirstVariable = ModuleData.FirstGlobalVariableName;
                if (ModuleData.UseConditionGlobalVariable)
                {
                    CompareTo = "Global Variable";
                }
                else
                {
                    CompareTo = "Custom Value";
                }
            }
            if (ModuleData.ConditionGlobalVariableName.Length > 0 || ModuleData.ConditionModuleVariableValue.Length > 0)
            {
                if (ModuleData.UseConditionGlobalVariable)
                {
                    SecondVariable = ModuleData.ConditionGlobalVariableName;
                }
                else
                {
                    SecondVariable = ModuleData.ConditionModuleVariableValue;
                }
            }

            List<string> FirstVarList, SecondVarList = new();

            /*
            Remember:
                If: -String: Same As & Not Same As
                    -Number: All Comparator Condition
                    -Bool Same As & Not Same As
                    -Keyboard: Same As & Not Same As
            */
            List<string> ComparatorConditionList = new()
            {
                "Same As",
                "Not Same As",
                "Greater Than",
                "Greater Than Equal",
                "Less Than",
                "Less Than Equal"
            };

            if (ModuleData.GlobalVariableType != GLOBAL_VARIABLE_TYPE.Number)
            {
                ComparatorConditionList = new()
                {
                    "Same As",
                    "Not Same As"
                };
            }
            List<string> ValueToCompare = new()
            {
                "Custom Value",
                "Global Variable"
            };

            if (SelectedCompareCondition != -1)
            {
                CompareCondition = ComparatorConditionList[SelectedCompareCondition];
            }
            
            // Fetching First Variable
            FirstVarList = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList.Keys.Where(x => x.StartsWith(FABRICK_SCENARIO_MANAGER.GlobalVariableTag)).ToList();

            // Fetching Second Variable (By Tag and Type)
            if (ModuleData.FirstGlobalVariableName.Length > 0)
            {
                Dictionary<string, FABRICK_GLOBAL_VARIABLE_DATA> _Temp = new();
                _Temp = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList.Where(x => 
                    x.Key.StartsWith(FABRICK_SCENARIO_MANAGER.GlobalVariableTag) &&
                    x.Key != ModuleData.FirstGlobalVariableName &&
                    x.Value.Type == ModuleData.GlobalVariableType
                )   .ToDictionary();
                SecondVarList = _Temp.Keys.ToList();
            }
            // Draw
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos2, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos3, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos4, _Size, EDITOR_MANAGER.Color3);

            FABRICK_DRAW_TEXT.DrawTextAdvanced(FirstVariable, "DefaultFont",    _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            FABRICK_DRAW_TEXT.DrawTextAdvanced(CompareCondition, "DefaultFont", _Pos2, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            FABRICK_DRAW_TEXT.DrawTextAdvanced(CompareTo, "DefaultFont",        _Pos3, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            FABRICK_DRAW_TEXT.DrawTextAdvanced(SecondVariable, "DefaultFont",   _Pos4, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");

            // Response
            if (!IsFocushed) return;
            Box box1 = new(_Pos, Vector2.Zero, _Size); // First Global Variable
            Box box2 = new(_Pos2, Vector2.Zero, _Size); // Condition
            Box box3 = new(_Pos3, Vector2.Zero, _Size); // Compare To
            Box box4 = new(_Pos4, Vector2.Zero, _Size); // Input Value

            if (FABRICK_COLLISIONS.MouseClickToBox_Pressed(box1, SDL.MouseButtonFlags.Left, false))
            {
                int Selected = FABRICK_DEBUG.DialogueList("Comparator", "First Global Variable", ref FirstVarList);
                if(Selected != -1)
                {
                    ModuleData.FirstGlobalVariableName = FirstVarList[Selected];
                    ModuleData.GlobalVariableType = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ModuleData.FirstGlobalVariableName].Type;
                    ModuleData.Condition = SCENARIO_MODULE_CONDITION.Same; // Cuz its safe
                    ModuleData.UseConditionGlobalVariable = false;
                    ModuleData.ConditionGlobalVariableName = "";
                    ModuleData.ConditionModuleVariableValue = "";
                }
            }
            if (FABRICK_COLLISIONS.MouseClickToBox_Pressed(box2, SDL.MouseButtonFlags.Left, false))
            {
                int Selected = FABRICK_DEBUG.DialogueList("Comparator", "Condition", ref ComparatorConditionList);
                SelectedCompareCondition = Selected;
                if(Selected != -1)
                {
                    SCENARIO_MODULE_CONDITION[] Conditions = Enum.GetValues<SCENARIO_MODULE_CONDITION>();
                    SCENARIO_MODULE_CONDITION Condition = Conditions[Selected];

                    ModuleData.Condition = Condition;
                    ModuleData.UseConditionGlobalVariable = false;
                    ModuleData.ConditionGlobalVariableName = "";
                    ModuleData.ConditionModuleVariableValue = "";
                }
            }
            if (FABRICK_COLLISIONS.MouseClickToBox_Pressed(box3, SDL.MouseButtonFlags.Left, false))
            {
                int Selected = FABRICK_DEBUG.DialogueList("Comparator", "Value to Compare From", ref ValueToCompare);
                if(Selected != -1)
                {
                    ModuleData.UseConditionGlobalVariable = Convert.ToBoolean(Selected);
                    ModuleData.ConditionGlobalVariableName = "";
                    ModuleData.ConditionModuleVariableValue = "";
                }
            }
            if (FABRICK_COLLISIONS.MouseClickToBox_Pressed(box4, SDL.MouseButtonFlags.Left, false))
            {
                if (ModuleData.UseConditionGlobalVariable)
                {
                    int Selected = FABRICK_DEBUG.DialogueList("Comparator", "Second Global variable", ref SecondVarList);
                    if(Selected != -1)
                    {
                        ModuleData.ConditionGlobalVariableName = SecondVarList[Selected];
                        ModuleData.ConditionModuleVariableValue = "";
                    }
                }
                else
                {
                    string _val = FABRICK_DEBUG.DialogueInputText("Comparator", "Enter Value Here", ModuleData.ConditionModuleVariableValue);
                    ModuleData.ConditionGlobalVariableName = "";
                    ModuleData.ConditionModuleVariableValue = _val;
                }
            }
        }
    }
}