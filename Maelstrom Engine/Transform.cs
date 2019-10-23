using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace Maelstrom_Engine {
    public class Transform {
        public Vector3 position { get; private set; }
        public Vector3 rotation { get; private set; }
        public Vector3 scale { get; private set; }
        public Transform(Vector3 position, Vector3 rotation, Vector3 scale) {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
