using Fabrick.ENGINE.DEBUG;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public partial class FABRICK_SHADER_MANAGER
    {
        public static Dictionary<string, FABRICK_SHADER_DATA> ShaderDataList = new();
        internal static string ShaderPath = "Shader";
        internal static string AssetDataPath = "";
        internal static nint GPUDevice;
        internal static nint renderer;

        public static void LoadShader(string ShaderName, string ShaderFile, SDL.GPUShaderFormat Format = SDL.GPUShaderFormat.SPIRV, SDL.GPUShaderStage Stage = SDL.GPUShaderStage.Fragment, string EntryPoint = "main", uint NumSamplers = 1, uint NumUniformBuffers = 0, uint NumStoragebuffers = 0, uint NumStorageTextures = 0, int NumSamplerBindings = 0)
        {
            if (FABRICK_SHADER_MANAGER.GPUDevice == nint.Zero)
            {
                FABRICK_DEBUG.SimpleMessageError("[FABRICK_SHADER_DATA] Renderer or GPU Device are null!!");
                return;
            }
            string FormatFile = Path.Join(FABRICK_SHADER_MANAGER.AssetDataPath, FABRICK_SHADER_MANAGER.ShaderPath, ShaderFile);
            if (!File.Exists(FormatFile))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_SHADER_DATA] Shader file not found: {FormatFile}");
                return;
            }
            if (ShaderDataList.ContainsKey(ShaderName))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_SHADER_DATA] Shader already exist: {ShaderName}");
                return;
            }
            FABRICK_SHADER_DATA ThisData = new()
            {
                ShaderName = ShaderName
            };
            nint _ShaderFile = SDL.IOFromFile(FormatFile, "rb");
            nint ShaderData = SDL.LoadFileIO(_ShaderFile, out nuint ShaderSize, true);
            if (ShaderData == 0)
            {
                FABRICK_DEBUG.SimpleMessageError("[FABRICK_ENGINE] " + SDL.GetError());
                return;
            }

            SDL.GPUShaderCreateInfo ShaderInfo = new()
            {
                CodeSize = ShaderSize,
                Entrypoint = EntryPoint,
                Code = ShaderData,
                Format = Format,
                Stage = Stage,
                NumSamplers = NumSamplers,
                NumUniformBuffers = NumUniformBuffers,
                NumStorageBuffers = NumStoragebuffers,
                NumStorageTextures = NumStorageTextures
            };

            ThisData.Shader = SDL.CreateGPUShader(GPUDevice, ShaderInfo);

            SDL.GPURenderStateCreateInfo StateInfo = new()
            {
                FragmentShader = ThisData.Shader,
                NumSamplerBindings = 0
            };
            ThisData.RenderState = SDL.CreateGPURenderState(renderer, in StateInfo);
            ShaderDataList.Add(ShaderName, ThisData);
            FABRICK_DEBUG.Log($"[FABRICK_SHADER_MANAGER] Shader Adder: {ShaderName}");
        }
    }
}