using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKSandbox
{
    public class SceneObject : IDisposable
    {
        public int Vao { get; private set; }
        public int Vbo { get; private set; }
        public int Ebo { get; private set; }

        public int ShaderProgramId { get; }
        public List<int> TextureIds { get; private set; }

        public SceneObject(int shaderProgramId, float[] vertices, params int[] structure)
        {
            ShaderProgramId = shaderProgramId;
            
            GenerateVao();

            GenerateVbo(vertices);

            LinkVertexAttributes(structure);
            
            GL.BindVertexArray(0);
        }

        public SceneObject(int shaderProgramId, float[] vertices, float[] indices, params int[] structure)
        {
            ShaderProgramId = shaderProgramId;

            GenerateVao();

            GenerateVbo(vertices);

            LinkVertexAttributes(structure);
            
            GenerateEbo(indices);
            
            GL.BindVertexArray(0);
        }

        private void GenerateVao()
        {
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);
        }

        private void GenerateVbo(float[] vertices)
        {
            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        private void GenerateEbo(float[] indices)
        {
            Ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        }

        private void LinkVertexAttributes(int[] structure)
        {   
            var structureLength = structure.Length;
            if (structureLength == 0)
                throw new ArgumentException("No vertex attributes passed!");

            var stride = structure.Sum();
            var shift = 0;
            
            for (var index = 0; index < structureLength; index++)
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, structure[index], VertexAttribPointerType.Float, false,
                    stride * sizeof(float), shift * sizeof(float));
                shift += structure[index];
            }
        }

        public void SetTextureIds(params int[] textureIds)
        {
            if (textureIds.Length == 0)
                throw new ArgumentException("No texture ids passed!");
            TextureIds = new List<int>(textureIds);
        }

        public void Use()
        {
            GL.BindVertexArray(Vao);
            
            GL.UseProgram(ShaderProgramId);

            if (TextureIds == null) return;
            foreach (var textureId in TextureIds)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + textureId);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (Vao != 0)
                GL.DeleteVertexArray(Vao);
            if (Vbo != 0)
                GL.DeleteBuffer(Vbo);
            if (Ebo != 0)
                GL.DeleteBuffer(Ebo);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SceneObject() => ReleaseUnmanagedResources();
    }
}