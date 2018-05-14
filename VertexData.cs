using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace OpenTKSandbox
{   
    public struct Vertex
    {
        public Vector3 Position { get; }
        public Vector3? Normal { get; }
        public Color4? Color { get; }
        public Vector2? TextureCoords { get; }
        
        public Vertex(Vector3 position, Vector3? normal = null, Color4? color = null, Vector2? textureCoords = null)
        {
            Position = position;
            Normal = normal;
            Color = color;
            TextureCoords = textureCoords;
        }
    }

    public interface IVertexData
    {        
        int Count { get; }

        int Size { get; }
        
        int[] Structure { get; }

        float[] GetBufferData();
    }
    
    public class VertexData : IVertexData
    {
        public Vertex[] Vertices { get; }
        public int Count => Vertices.Length;
        
        public int[] Structure { get; }

        private static readonly Dictionary<string, int> Dimensions = new Dictionary<string, int>
        {
            { "Position", 3 },
            { "Normal", 3 },
            { "Color", 4 },
            { "TextureCoords", 2 }
        };

        public VertexData(Vertex[] vertices)
        {
            Vertices = vertices;
            
            var structureList = new List<int> { Dimensions["Position"] };

            if (HasNormal)
                structureList.Add(Dimensions["Normal"]);
            if (HasColor)
                structureList.Add(Dimensions["Color"]);
            if (HasTextureCoords)
                structureList.Add(Dimensions["TextureCoords"]);

            Structure = structureList.ToArray();
        }

        private bool HasNormal => Vertices[0].Normal.HasValue;
        private bool HasColor => Vertices[0].Color.HasValue;
        private bool HasTextureCoords => Vertices[0].TextureCoords.HasValue;

        public float[] GetBufferData()
        {
            var bufferData = new List<float>();
            foreach (var vertex in Vertices)
            {
                bufferData.AddRange(new [] { vertex.Position.X, vertex.Position.Y, vertex.Position.Z });
                if (HasNormal)
                    bufferData.AddRange(new [] { vertex.Normal.Value.X, vertex.Normal.Value.X, vertex.Position.Z });
                if (HasColor)
                    bufferData.AddRange(new [] { vertex.Color.Value.R, vertex.Color.Value.G, vertex.Color.Value.B, vertex.Color.Value.A });
                if (HasTextureCoords)
                    bufferData.AddRange(new [] { vertex.TextureCoords.Value.X, vertex.TextureCoords.Value.Y });
            }
            return bufferData.ToArray();
        }

        public int Size => Count * sizeof(float) * (Dimensions["Position"]
                                                    + (HasNormal ? Dimensions["Normal"] : 0)
                                                    + (HasColor ? Dimensions["Color"] : 0)
                                                    + (HasTextureCoords? Dimensions["TextureCoords"] : 0));
    }
}