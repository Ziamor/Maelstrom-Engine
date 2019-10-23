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
    public class Material {
        List<Texture> textures;
        public Shader shader { get; private set; }

        public Material(List<Texture> textures, Shader shader) {
            this.textures = textures;
            this.shader = shader;

            Init();
        }

        private void Init() {
            //TODO this wont work for multi texture as it will bind to the same texture uniform
            foreach (Texture texture in textures) {
                shader.SetInt("texture0", 0);
            }
        }

        public void Use() {
            //TODO this wont work for multi texture as it will bind to the same texture unit
            foreach (Texture texture in textures) {
                texture.Use();
            }

            shader.Use();
        }
    }
}
