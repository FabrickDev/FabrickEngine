using Fabrick.ENGINE.DEBUG;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public partial class FABRICK_SHADER_MANAGER
    {
        internal static string ShaderPath = "Shader";
        internal static string AssetDataPath = "";
        public static nint LoadShader(nint Device, string ShaderPathFile)
        {
            if (Device == nint.Zero)
            {
                FABRICK_DEBUG.SimpleMessageError("[FABRICK_SHADER_DATA] Renderer or GPU Device are null!!");
                return 0;
            }
            string FormatFile = Path.Join(AssetDataPath, ShaderPath, ShaderPathFile);
            if (!File.Exists(FormatFile))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_SHADER_DATA] Shader file not found: {FormatFile}");
                return 0;
            }
            nint CodeData = SDL.IOFromFile(FormatFile, "r");
            var ShaderInfo = new SDL.GPUShaderCreateInfo
            {
                Code = CodeData,
                Entrypoint = "main",
                Format = SDL.GPUShaderFormat.SPIRV,
                Stage = SDL.GPUShaderStage.Fragment,
                NumSamplers = 1,
                NumUniformBuffers = 0
            };
            var FragShader = SDL.CreateGPUShader(Device, ShaderInfo);
            if (FragShader == nint.Zero)
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_SHADER_DATA] Create GPU Shader error: {SDL.GetError()}");
                return 0;
            }
            return FragShader;
        }
    }
}