using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Maelstrom_Engine {
    public class Mesh {
        Vertex[] vertices;
        uint[] indices;
        Material material;

        public Mesh(Vertex[] vertices, uint[] indices, Material material) {
            this.vertices = vertices;
            this.indices = indices;
            this.material = material;

            InitMesh();
        }

        public int VAO { get; private set; }

        public int VBO { get; private set; }

        public int EBO { get; private set; }

        public void Render(Transform transform, Camera camera) {
            Matrix4 modelMatrix = Matrix4.CreateRotationX(transform.rotation.X);
            modelMatrix *= Matrix4.CreateRotationY(transform.rotation.Y);
            modelMatrix *= Matrix4.CreateRotationZ(transform.rotation.Z);

            modelMatrix *= Matrix4.CreateScale(transform.scale);
            modelMatrix *= Matrix4.CreateTranslation(transform.position);

            material.Use();

            material.shader.SetMatrix4("model", modelMatrix);
            material.shader.SetMatrix4("view", camera.ViewMatrix);
            material.shader.SetMatrix4("projection", camera.ProjectionMatrix);

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private void InitMesh() {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf(typeof(Vertex)), vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int vertexLocation = 0;
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), 0);

            int texCoordLocation = 1;
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.SizeOf(typeof(Vector3)));

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex {
        public Vector3 Position;
        public Vector2 TextureCoord;

        public Vertex(Vector3 position, Vector2 textureCoord) {
            Position = position;
            TextureCoord = textureCoord;
        }

        public Vertex(float x, float y, float z, float u, float v) {
            Position = new Vector3(x, y, z);
            TextureCoord = new Vector2(u, v);
        }
    }
}
