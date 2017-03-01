using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp
{
    class ShaderProgram
    {
        private ShaderBytecode vertexShaderByteCode;
        private ShaderBytecode pixelShaderByteCode;
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private Device device;
        private ShaderSignature signature;
        private InputLayout layout;
        private DeviceContext context;

        public void compileVertexShader(string path)
        {
            vertexShaderByteCode = ShaderBytecode.CompileFromFile(path, "VS", "vs_4_0", ShaderFlags.None);
            vertexShader = new VertexShader(device, vertexShaderByteCode);
            signature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
        }

        public void compilePixelShader(string path)
        {
            pixelShaderByteCode = ShaderBytecode.CompileFromFile(path, "PS", "ps_4_0", ShaderFlags.None);
            pixelShader = new PixelShader(device, pixelShaderByteCode);
        }

        public ShaderProgram(Device device)
        {
            this.device = device;
            context = device.ImmediateContext;
        }

        public void setLayout(InputElement[] elements)
        {
            layout = new InputLayout(device, vertexShaderByteCode, elements);
        }

        public void useShader()
        {
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
        }
    }
}
