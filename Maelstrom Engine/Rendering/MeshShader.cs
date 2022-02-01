using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom {
    public class MeshShader : Shader {
        public MeshShader() : base("Assets/Shaders/shader.vert", "Assets/Shaders/shader.frag") { 
        }
    }
}
