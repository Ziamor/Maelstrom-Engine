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
        Vertex[] vertices = {
            new Vertex(-0.5f, -0.5f, -0.5f,  0.0f, 0.0f),
            new Vertex( 0.5f, -0.5f, -0.5f,  1.0f, 0.0f),
            new Vertex(0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            new Vertex( 0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            new Vertex(-0.5f,  0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex( -0.5f, -0.5f, -0.5f,  0.0f, 0.0f),

            new Vertex(-0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            new Vertex( 0.5f, -0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex( 0.5f,  0.5f,  0.5f,  1.0f, 1.0f),
            new Vertex( 0.5f,  0.5f,  0.5f,  1.0f, 1.0f),
            new Vertex(-0.5f,  0.5f,  0.5f,  0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f,  0.5f,  0.0f, 0.0f),

            new Vertex( -0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex(-0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex(-0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            new Vertex(-0.5f,  0.5f,  0.5f,  1.0f, 0.0f),

            new Vertex( 0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex(0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            new Vertex( 0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex( 0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex( 0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            new Vertex(0.5f,  0.5f,  0.5f,  1.0f, 0.0f),

            new Vertex(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex( 0.5f, -0.5f, -0.5f,  1.0f, 1.0f),
            new Vertex( 0.5f, -0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex( 0.5f, -0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex(-0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            new Vertex(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),

            new Vertex(-0.5f,  0.5f, -0.5f,  0.0f, 1.0f),
            new Vertex( 0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            new Vertex( 0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex( 0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            new Vertex(-0.5f,  0.5f,  0.5f,  0.0f, 0.0f),
            new Vertex(-0.5f,  0.5f, -0.5f,  0.0f, 1.0f)
        };
        uint[] indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            3, 4, 5,    // second triangle
            6,7,8,9,10,11,12,13,14,15,16,17
        };
        Shader boxShader;
        Shader lightShader;

        Texture paperTex, woodTex;

        Camera camera;

        float time = 0;

        Mesh box1, box2;
        Model cat;

        Transform box1Transform, catTransform;
        Material box1Material, catMaterial;

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

            box1.Render(box1Transform, camera, box1Material);
            cat.Render(catTransform, camera, catMaterial);

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

            boxShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            lightShader = new Shader("Shaders/light_shader.vert", "Shaders/light_shader.frag");

            paperTex = new Texture("Assets/stained_paper_texture.jpg");
            woodTex = new Texture("Assets/wood_texture.jpg");

            box1 = new Mesh(vertices, indices);
            box1Transform = new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            box1Material = new Material(new List<Texture>() { paperTex }, boxShader);

            catTransform = new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.05f, 0.05f, 0.05f));
            catMaterial = new Material(new List<Texture>(), lightShader);
            cat = new Model("cat.obj");
            camera = new Camera(this);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e) {
            boxShader.Dispose();
            GL.DeleteTexture(paperTex.Handle);
            GL.DeleteTexture(woodTex.Handle);

            lightShader.Dispose();

            base.OnUnload(e);
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }
    }
}
