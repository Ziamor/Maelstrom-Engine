using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom_Engine {
    public class Light : Renderable, Updatable {
        Model lightMesh;
        public Transform transform;

        float t;
        public Light() {
            lightMesh = new Model("sphere.obj");
            lightMesh.OverrideMaterial(new Material(new List<Texture>(), Game.defaultLightShader));
            transform = new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f));
        }

        public void Render(Transform t, Camera camera) {
            lightMesh.Render(transform, camera);
        }

        public void Update(float deltaTime) {
            t += deltaTime;
            transform.position = new Vector3((float)Math.Sin(t) * 20, 0, (float)Math.Cos(t) * 20);
        }
    }
}
