using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp
{
    struct Vertex
    {
        private Vector3 position;
        public Vertex(Vector3 position)
        {
            this.position = position;
        }
    }

    struct VertexColored
    {
        private Vector3 position;
        private Vector3 color;

        public VertexColored(Vector3 position, Vector3 color)
        {
            this.position = position;
            this.color = color;
        }

    }

    struct VertexColoredNormal
    {
        private Vector3 position;
        private Vector3 color;
        private Vector3 normal;

        public VertexColoredNormal(Vector3 position, Vector3 color, Vector3 normal)
        {
            this.position = position;
            this.color = color;
            this.normal = normal;
        }

    }

    struct VertexNormalTexcoord
    {
        private Vector3 position;
        private Vector3 normal;
        private Vector2 texcoord;

        public VertexNormalTexcoord(Vector3 position, Vector3 normal, Vector2 texcoord)
        {
            this.position = position;
            this.normal = normal;
            this.texcoord = texcoord;
        }
    }
}
