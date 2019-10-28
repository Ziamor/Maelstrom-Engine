using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Maelstrom_Engine {
    public class Shader : IDisposable {
        public readonly int Handle;
        private readonly Dictionary<string, int> uniformLocations;

        public Shader(string vertexPath, string fragmentPath) {
            string VertexShaderSource;

            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8)) {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8)) {
                FragmentShaderSource = reader.ReadToEnd();
            }

            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != System.String.Empty)
                System.Console.WriteLine(infoLogVert);

            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);

            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);

            uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++) {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                uniformLocations.Add(key, location);
            }
        }

        public void Use() {
            SetInt("diffuseTex", 0);
            SetInt("normalMapTex", 1);
            SetInt("specularMapTex", 2);

            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName) {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void SetInt(string name, int value) {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform1(location, value);
        }

        internal void SetMatrix4(string name, Matrix4 matrix) {
            int location;
            if (uniformLocations.TryGetValue(name, out location)) {
                GL.UseProgram(Handle);
                GL.UniformMatrix4(location, true, ref matrix);
            }
        }

        internal void SetVec3(string name, Vector3 vec3) {
            int location;
            if (uniformLocations.TryGetValue(name, out location)) {
                GL.ProgramUniform3(Handle, location, ref vec3);
            }
        }

        internal void SetVec4(string name, Vector4 vec4) {
            int location;
            if (uniformLocations.TryGetValue(name, out location)) {
                GL.ProgramUniform4(Handle, location, ref vec4);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                try {
                    GL.DeleteProgram(Handle);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }

                disposedValue = true;
            }
        }

        ~Shader() {
            Dispose(true);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
