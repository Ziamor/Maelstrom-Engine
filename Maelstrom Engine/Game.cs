using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Assimp;
using System.Reflection;
using System.IO;
using Assimp.Configs;

namespace Maelstrom_Engine {
    class Game : GameWindow {
        public static Shader defaultDiffuseShader;

        Shader lightShader;

        Camera camera;

        float time = 0;

        Model nanoSuit, axe, lamp;

        Transform nanoSuitTransform, axeTransform, lampTransform;

        private readonly Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);        

        public Game(int width, int height, string title)
            : base(width,
                  height,
                  OpenTK.Graphics.GraphicsMode.Default,
                  title,
                  GameWindowFlags.Default,
                  DisplayDevice.Default,
                  4,
                  0,
                  OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible) {
            Console.WriteLine("Created window");
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            float deltaTime = (float)e.Time;

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyState.IsKeyDown(Key.Escape)) {
                Exit();
            }

            camera.Update(deltaTime, keyState, mouse);

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            time += (float)e.Time * 25f;
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //nanoSuit.Render(nanoSuitTransform, camera);
            axe.Render(axeTransform, camera);
            lamp.Render(lampTransform, camera);

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            if (Focused) // check to see if the window is focused  
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnLoad(EventArgs e) {
            CursorVisible = false;

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            defaultDiffuseShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            lightShader = new Shader("Shaders/light_shader.vert", "Shaders/light_shader.frag");

            nanoSuitTransform = new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.05f, 0.05f, 0.05f));
            axeTransform = new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f));
            lampTransform = new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f));

            //nanoSuit = new Model("scene.fbx");
            lamp = new Model("lamp.obj");
            axe = new Model("Viking_Axe_Straight.fbx");

            camera = new Camera(this);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e) {
            defaultDiffuseShader.Dispose();

            lightShader.Dispose();

            base.OnUnload(e);
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
    }
}
