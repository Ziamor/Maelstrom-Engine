using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Maelstrom_Engine {
    public class Mesh {
        float[] vertices;

        public Mesh(float[] vertices) {
            this.vertices = vertices;

            InitMesh();
        }

        public int VAO { get; private set; }

        public int VBO { get; private set; }

        public void Render(Transform transform, Camera camera, Material material) {
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
        }

        private void InitMesh() {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int vertexLocation = 0;
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = 1;
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
