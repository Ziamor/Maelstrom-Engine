using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom {
    public class LightShader : Shader {
        public LightShader() : base("Assets/Shaders/light_shader.vert", "Assets/Shaders/light_shader.frag") {

        }
    }
}
