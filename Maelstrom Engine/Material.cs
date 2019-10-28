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
        Texture diffuse, normalMap, specularMap;
        public Shader shader { get; private set; }

        public Material(Texture diffuse, Texture normalMap, Texture specularMap, Shader shader) {
            this.diffuse = diffuse;
            this.normalMap = normalMap;
            this.specularMap = specularMap;
            this.shader = shader;
        }

        public void Use() {
            if (diffuse != null)
                diffuse.Use(TextureUnit.Texture0);
            if (normalMap != null)
                normalMap.Use(TextureUnit.Texture1);
            if (specularMap != null)
                specularMap.Use(TextureUnit.Texture2);

            shader.Use();
        }
    }
}
