using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using System.Diagnostics;

namespace Sharp
{
    class Game
    {
        private Device device;
        private DeviceContext context;
        private Stopwatch clock;
        public Matrix proj, view, viewProj;
        private Vector4 eyePosition;
        private CBufferPerFrame cBufferPerFrame;
        private Buffer constantBufferPerFrame;
        private List<Mesh<VertexNormalTexcoord>> meshes;
        public Game(Device device)
        {
            this.device = device;
            context = device.ImmediateContext;
        }
        public void Initialize()
        {
            // Compile Vertex and Pixel shaders
            var shaderProgram = new ShaderProgram(device);
            shaderProgram.compileVertexShader("shader.fx");
            shaderProgram.compilePixelShader("shader.fx");

            // Layout from VertexShader input signature
            shaderProgram.setLayout(new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0)
                    });

            meshes = new List<Mesh<VertexNormalTexcoord>>();
            Mesh<VertexNormalTexcoord> mesh = new Mesh<VertexNormalTexcoord>(device);
            mesh.Initialize(new[]
                                  {
                                      new VertexNormalTexcoord(new Vector3(-1.0f, -1.0f, -1.0f),  new Vector3(-1,-1,-1), new Vector2(0,0)), //1
                                      new VertexNormalTexcoord(new Vector3(-1.0f,  1.0f, -1.0f), new Vector3(-1,1,-1), new Vector2(0,0)), //2
                                      new VertexNormalTexcoord(new Vector3( 1.0f,  1.0f, -1.0f),  new Vector3(1,1,-1), new Vector2(0,0)), //3
                                      new VertexNormalTexcoord(new Vector3( 1.0f, -1.0f, -1.0f),  new Vector3(1,-1,-1), new Vector2(0,0)), //4
                                      new VertexNormalTexcoord(new Vector3(-1.0f, -1.0f,  1.0f),  new Vector3(-1,-1,1), new Vector2(0,0)), //5
                                      new VertexNormalTexcoord(new Vector3( 1.0f,  1.0f,  1.0f),  new Vector3(1,1,1), new Vector2(0,0)), //6
                                      new VertexNormalTexcoord(new Vector3(-1.0f,  1.0f,  1.0f),  new Vector3(-1,1,1), new Vector2(0,0)), //7
                                      new VertexNormalTexcoord(new Vector3( 1.0f, -1.0f,  1.0f), new Vector3(1,-1,1), new Vector2(0,0)), //8
                            },new int[] {
                                0,1,2,0,2,3,
                                4,5,6,4,7,5,
                                1,6,5,1,5,2,
                                0,7,4,0,3,7,
                                0,4,6,0,6,1,
                                3,5,7,3,2,5 });

            meshes.Add(mesh);

            // Create Constant Buffer
            constantBufferPerFrame = new Buffer(device, Utilities.SizeOf<CBufferPerFrame>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            cBufferPerFrame = new CBufferPerFrame();

            // Prepare All the stages
            shaderProgram.useShader();
            
            context.VertexShader.SetConstantBuffer(0, constantBufferPerFrame);
            
            // Prepare matrices
            eyePosition = new Vector4(0, 3, -5, 0);
            view = Matrix.LookAtRH(new Vector3(eyePosition.X, eyePosition.Y, eyePosition.Z), new Vector3(0, 0, 0), Vector3.UnitY);
            proj = Matrix.Identity;

            // Use clock
            clock = new Stopwatch();
            clock.Start();
        }

        public void Update()
        {
            var time = clock.ElapsedMilliseconds / 1000.0f;
            viewProj = Matrix.Multiply(view, proj);
            viewProj.Transpose();

            cBufferPerFrame.viewProj = viewProj;
            cBufferPerFrame.eyePosition = eyePosition;

            context.UpdateSubresource(ref cBufferPerFrame, constantBufferPerFrame);

            foreach (Mesh<VertexNormalTexcoord> mesh in meshes)
            {
                mesh.update(ref time);
            }
        }

        public void Render()
        {
            foreach (Mesh<VertexNormalTexcoord> mesh in meshes)
            {
                mesh.render();
            }
        }

        public void Clean()
        {

        }
    }

    //Constant Buffers
    struct CBufferPerObject
    {
        public Matrix world;
        public Matrix worldIT;
    };

    struct CBufferPerFrame
    {
        public Matrix viewProj;
        public Vector4 eyePosition;
    }
}
