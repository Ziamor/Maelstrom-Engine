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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maelstrom_Engine {
    class Game : GameWindow {
        public static Shader defaultDiffuseShader;
        public static Shader defaultLightShader;

        public static Texture defaultSpecular;

        Camera camera;

        float time = 0;

        Model nanoSuit, axe;
        Light light;

        Transform nanoSuitTransform, axeTransform;

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

            if (keyState.IsKeyDown(Key.R)) {
                //defaultDiffuseShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            }

            //light.Update(deltaTime);

            camera.Update(deltaTime, keyState, mouse);

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            time += (float)e.Time;
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            light.Render(null, camera);

            defaultDiffuseShader.SetVec4("lightColor", new Vector4(1f, .9f, .8f, 1));
            defaultDiffuseShader.SetVec3("lightPos", light.transform.position);

            nanoSuit.Render(nanoSuitTransform, camera);
            axe.Render(axeTransform, camera);

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

            GL.ClearColor(13 / 256f, 16 / 256f, 28 / 256f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            defaultDiffuseShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            defaultLightShader = new Shader("Shaders/light_shader.vert", "Shaders/light_shader.frag");

            defaultSpecular = Texture.CreateTexture(new Image<Rgba32>(SixLabors.ImageSharp.Configuration.Default, 1, 1, Rgba32.White));
            light = new Light();

            nanoSuitTransform = new Transform(new Vector3(0, 0, -10), new Vector3(0, 45, 0), new Vector3(1f, 1f, 1f));
            axeTransform = new Transform(new Vector3(10, 0, 0), new Vector3(0, 0, 45), new Vector3(10f, 10f, 10f));

            nanoSuit = new Model("model.dae");
            axe = new Model("Viking_Axe_Straight.fbx");

            camera = new Camera(this);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e) {
            defaultDiffuseShader.Dispose();

            defaultLightShader.Dispose();

            base.OnUnload(e);
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
    }
}
