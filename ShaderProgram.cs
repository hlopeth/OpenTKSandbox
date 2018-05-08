using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKSandbox
{
    public class ShaderProgram : IDisposable
    {
        public int Id { get; }

        public ShaderProgram(string vertexFileName, string fragmentFileName)
        {
            var vertexCode = "";
            var fragmentCode = "";

            try
            {
                using (var reader = new StreamReader(@"..\..\Shaders\" + vertexFileName))
                {
                    vertexCode = reader.ReadToEnd();
                    reader.Close();
                }

                using (var reader = new StreamReader(@"..\..\Shaders\" + fragmentFileName))
                {
                    fragmentCode = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Shader file not found: " + ex.FileName);
                return;
            }

            var vertexId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexId, vertexCode);
            GL.CompileShader(vertexId);
            if (!IsCompilationSuccess(vertexId, Type.Vertex)) return;

            var fragmentId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentId, fragmentCode);
            GL.CompileShader(fragmentId);
            if (!IsCompilationSuccess(vertexId, Type.Vertex)) return;

            Id = GL.CreateProgram();
            GL.AttachShader(Id, vertexId);
            GL.AttachShader(Id, fragmentId);
            GL.LinkProgram(Id);
            if (!IsCompilationSuccess(Id, Type.Program)) return;
            
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

        private static bool IsCompilationSuccess(int id, Type type)
        {
            int success;
            string infoLog;

            switch (type)
            {
                case Type.Vertex:
                case Type.Fragment:
                    GL.GetShader(id, ShaderParameter.CompileStatus, out success);
                    if (success != 0) return true;
                    infoLog = GL.GetShaderInfoLog(id);
                    Console.WriteLine($"Failed compiling {type} shader. Log: {Environment.NewLine + infoLog}");
                    return false;
                case Type.Program:
                    GL.GetProgram(id, GetProgramParameterName.LinkStatus, out success);
                    if (success != 0) return true;
                    infoLog = GL.GetProgramInfoLog(id);
                    Console.WriteLine($"Failed compiling shader {type}. Log: {Environment.NewLine + infoLog}");
                    return false;
                default:
                    Console.WriteLine($"{type} is unknown.");
                    return false;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (Id != 0)
                GL.DeleteProgram(Id);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ShaderProgram() => ReleaseUnmanagedResources();        
    }

//    [Serializable]
       //    public class ShaderProgramLinkingException : Exception
       //    {
       //        public ShaderProgramLinkingException() { }
       //
       //        public ShaderProgramLinkingException(string message)
       //            : base("Failed linking shader program: " + message) { }
       //    }
}