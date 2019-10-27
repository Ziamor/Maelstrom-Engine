using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom_Engine
{
    public class Camera
    {
        public float speed = 10f;
        public float mouseSensitivity = 0.05f;

        float yaw = 270, pitch;

        bool firstMove = false;

        Vector3 position = new Vector3(0.0f, 0.0f, 3f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        Vector2 lastPos;

        GameWindow parentWindow;

        bool enableMouseLook = true;

        public Camera(GameWindow parentWindow) {
            this.parentWindow = parentWindow;
        }

        public void Update(float deltaTime, KeyboardState input, MouseState mouse)
        {
            if (input.IsKeyDown(Key.Q)) {
                enableMouseLook = !enableMouseLook;
            }

            if (input.IsKeyDown(Key.W))
            {
                position += front * speed * deltaTime; //Forward 
            }

            if (input.IsKeyDown(Key.S))
            {
                position -= front * speed * deltaTime; //Backwards
            }

            if (input.IsKeyDown(Key.A))
            {
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * deltaTime; //Left
            }

            if (input.IsKeyDown(Key.D))
            {
                position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * deltaTime; //Right
            }

            if (input.IsKeyDown(Key.Space))
            {
                position += up * speed * deltaTime; //Up 
            }

            if (input.IsKeyDown(Key.LShift))
            {
                position -= up * speed * deltaTime; //Down
            }

            if (firstMove) // this bool variable is initially set to true
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }

            float deltaX = mouse.X - lastPos.X;
            float deltaY = mouse.Y - lastPos.Y;
            lastPos = new Vector2(mouse.X, mouse.Y);

            if (enableMouseLook) {
                yaw += deltaX * mouseSensitivity;
                pitch -= deltaY * mouseSensitivity;

                front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
                front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
                front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
                front = Vector3.Normalize(front);
            }
        }

        public Matrix4 ViewMatrix {
            get {
                return  Matrix4.LookAt(position, position + front, up);;
            }
        }

        public Matrix4 ProjectionMatrix {
            get {
                return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), parentWindow.Width / (float)parentWindow.Height, 0.1f, 100f);
            }
        }
    }
}
