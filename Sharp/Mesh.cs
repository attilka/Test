using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Sharp
{
    class Mesh<T> where T : struct
    {
        private Buffer vertexBuffer;
        private Buffer constantBuffer;
        private Buffer indexBuffer;
        private T[] vertices;
        private int[] indices;
        private CBufferPerObject cBuffer;
        private Matrix world;
        private Device device;
        private DeviceContext context;

        public Mesh(Device device)
        {
            this.device = device;
            context = device.ImmediateContext;
            constantBuffer = new Buffer(device, Utilities.SizeOf<CBufferPerObject>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            cBuffer = new CBufferPerObject();
        }

        public void Initialize(T[] vertices, int[] indices)
        {
            this.vertices = vertices;
            vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            this.indices = indices;
            indexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);
        }
        public void update(ref float time)
        {
            // Update WorldViewProj Matrix
            world = Matrix.RotationY(time);

            world.Transpose();

            var worldIT = world;
            worldIT.Invert();

            cBuffer.world = world;
            cBuffer.worldIT = worldIT;
        }

        public void render()
        {
            context.VertexShader.SetConstantBuffer(1, constantBuffer);
            context.UpdateSubresource(ref cBuffer, constantBuffer);

            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<T>(), 0));
            context.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_SInt, 0);
            context.DrawIndexed(indices.Length, 0, 0);
        }

    }
}
