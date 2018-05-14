using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKSandbox
{
    public class ShaderProgramException : ApplicationException
    {
        public ShaderProgramException() { }
        public ShaderProgramException(string message) : base(message) { }
    }

    public class ShaderProgram : IDisposable
    {
        public int Id { get; }

        public ShaderProgram(string vertexFileName, string fragmentFileName)
        {
            var vertexCode = ReadCode(vertexFileName);
            var fragmentCode = ReadCode(fragmentFileName);

            var vertexId = Compile(vertexCode, ShaderType.VertexShader);
            var fragmentId = Compile(fragmentCode, ShaderType.FragmentShader);
            
            Id = GL.CreateProgram();
            GL.AttachShader(Id, vertexId);
            GL.AttachShader(Id, fragmentId);
            GL.LinkProgram(Id);
            if (!CompilationSuccess(Id, Type.Program))
            {
                var infoLog = GL.GetProgramInfoLog(Id);
                throw new ShaderProgramException($"Failed linking shaders {Type.Program}. Log: {Environment.NewLine + infoLog}");
            }
            
            GL.DetachShader(Id, vertexId);
            GL.DetachShader(Id, fragmentId);
            GL.DeleteShader(vertexId);
            GL.DeleteShader(fragmentId);
        }

        public void Use() => GL.UseProgram(Id);
        public void SetUniform(string name, int value) => GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        public void SetUniform(string name, float value) => GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        public void SetUniform(string name, Vector2 value) => GL.Uniform2(GL.GetUniformLocation(Id, name), value);
        public void SetUniform(string name, Vector3 value) => GL.Uniform3(GL.GetUniformLocation(Id, name), value);
        public void SetUniform(string name, Vector4 value) => GL.Uniform4(GL.GetUniformLocation(Id, name), value);
        public void SetUniform(string name, Matrix2 value) => GL.UniformMatrix2(GL.GetUniformLocation(Id, name), false, ref value);
        public void SetUniform(string name, Matrix3 value) => GL.UniformMatrix3(GL.GetUniformLocation(Id, name), false, ref value);
        public void SetUniform(string name, Matrix4 value) => GL.UniformMatrix4(GL.GetUniformLocation(Id, name), false, ref value);

        private enum Type
        {
            Vertex,
            Fragment,
            Program
        }

        private string ReadCode(string FileName)
        {
            string source="";
            if (File.Exists(@"..\..\Shaders\" + FileName))
            {
                using (var reader = new StreamReader(@"..\..\Shaders\" + FileName))
                {
                    source = reader.ReadToEnd();
                    reader.Close();
                }
            }
            else
            {
                throw new ShaderProgramException("Shader file not found: " + @"Shaders\" + FileName);
            }
            return source;
        }

        private int Compile(string shaderCode, ShaderType shaderType)
        {
            var shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, shaderCode);
            GL.CompileShader(shaderId);
            var type = shaderType == ShaderType.VertexShader ? Type.Vertex : Type.Fragment;
            if (CompilationSuccess(shaderId, type))
                return shaderId;
            else
            {
                var infoLog = GL.GetShaderInfoLog(shaderId);
                throw new ShaderProgramException($"Failed compiling { type } shader.Log: { Environment.NewLine + infoLog}");
            }
        }

        private static bool CompilationSuccess(int id, Type type)
        {
            int success;

            switch (type)
            {
                case Type.Vertex:
                case Type.Fragment:
                    GL.GetShader(id, ShaderParameter.CompileStatus, out success);
                    if (success != 0) return true;
                    return false;
                case Type.Program:
                    GL.GetProgram(id, GetProgramParameterName.LinkStatus, out success);
                    if (success != 0) return true;
                    return false;
                default:
                    //Console.WriteLine($"{type} is unknown.");
                    return false;
            }
        }


        bool disposed = false;
        private void ReleaseUnmanagedResources()
        {
            if (!disposed)
            {
                if (Id != 0)
                    GL.DeleteProgram(Id);
                disposed = true;
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

    }

}