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
        Texture diffuseTexture, normalMapTexture, specularMapTexture;

        public Shader shader { get; private set; }

        public Material(Texture diffuseTexture, Texture normalMapTexture, Texture specularMapTexture, Shader shader) {
            this.diffuseTexture = diffuseTexture;
            this.normalMapTexture = normalMapTexture;
            this.specularMapTexture = specularMapTexture;
            this.shader = shader;
        }

        public void Use() {
            if (diffuseTexture != null)
                diffuseTexture.Use(TextureUnit.Texture0);
            if (normalMapTexture != null)
                normalMapTexture.Use(TextureUnit.Texture1);
            if (specularMapTexture != null)
                specularMapTexture.Use(TextureUnit.Texture2);

            shader.Use();
        }
    }
}
