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
using Maelstrom.UI;

namespace Maelstrom {
    class Game : GameWindow {
        public static MeshShader meshShader;
        public static LightShader lightShader;

        public static Texture defaultDiffuseTexture, defaultSpecularTexture;

        Camera camera;

        float time = 0;

        Model nanoSuit, axe, plane;
        List<Light> lights;

        Transform nanoSuitTransform, axeTransform, planeTransform;

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

            for (int i = 0; i < lights.Count; i++) {
                lights[i].Update(deltaTime);
            }

            camera.Update(deltaTime, keyState, mouse);

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            time += (float)e.Time;
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < lights.Count; i++) {
                Light light = lights[i];
                light.Render(null, camera);

                meshShader.SetFloat("directionalLight.ambientStrength", 0.1f);
                meshShader.SetVec3("directionalLight.diffuse", light.lightColor);
                meshShader.SetVec3("directionalLight.dir", new Vector3(0.5f, 0.25f, 0));

                meshShader.SetVec3($"pointLights[{i}].diffuse", light.lightColor);
                meshShader.SetVec3($"pointLights[{i}].position", light.transform.position);
                meshShader.SetFloat($"pointLights[{i}].constant", 1f);
                meshShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                meshShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }            

            //nanoSuit.Render(nanoSuitTransform, camera);
            axe.Render(axeTransform, camera);
            plane.Render(planeTransform, camera);

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

        private Vector3 HexToRgb(int hex) {
            float r = ((hex & 0xff0000) >> 16) / 256f;
            float g = ((hex & 0xff00) >> 8) / 256f;
            float b = (hex & 0xff) / 256f;

            return new Vector3(r, g, b);
        }

        protected override void OnLoad(EventArgs e) {
            CursorVisible = false;

            GL.ClearColor(13 / 256f, 16 / 256f, 28 / 256f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            meshShader = new MeshShader();
            lightShader = new LightShader();

            defaultDiffuseTexture = Texture.CreateTexture(new Image<Rgba32>(SixLabors.ImageSharp.Configuration.Default, 1, 1, Rgba32.White));
            defaultSpecularTexture = Texture.CreateTexture(new Image<Rgba32>(SixLabors.ImageSharp.Configuration.Default, 1, 1, Rgba32.White));

            lights = new List<Light>();
            lights.Add(new Light(HexToRgb(0xfff187), 0));
            lights.Add(new Light(HexToRgb(0x8589ff), 3.14f));
            nanoSuitTransform = new Transform(new Vector3(0, 0, -10), new Vector3(0, 45, 0), new Vector3(1f, 1f, 1f));
            axeTransform = new Transform(new Vector3(10, 0, 0), new Vector3(0, 0, 0), new Vector3(10f, 10f, 10f));
            planeTransform = new Transform(new Vector3(0, -1, 0), new Vector3(0, 0, 0), new Vector3(10f, 1f, 10f));

            //nanoSuit = new Model("model.dae");
            axe = new Model("Viking_Axe_Straight.fbx.json");

            plane = new Model("plane.obj.json");

            camera = new Camera(this);

            Font font = new Font("Arial");
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e) {
            meshShader.Dispose();

            lightShader.Dispose();

            base.OnUnload(e);
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
    }
}
