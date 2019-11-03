using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom {
    public class Light : Renderable, Updatable {
        Model lightMesh;
        public Transform transform;
        public Vector3 lightColor;

        float offset = 0;
        float dist = 10;
        float t;
        public Light(Vector3 lightColor, float offset) {
            this.lightColor = lightColor;
            this.offset = offset;

            lightMesh = new Model("sphere.obj.json");
            lightMesh.OverrideMaterial(new Material(null, null, null, Game.lightShader));
            transform = new Transform(new Vector3(0, 2, 0), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f));
        }

        public void Render(Transform t, Camera camera) {
            Game.lightShader.SetVec3("lightColor", lightColor);
            lightMesh.Render(transform, camera);
        }

        public void Update(float deltaTime) {
            t += deltaTime;
            transform.position = new Vector3((float)Math.Sin(t + offset) * dist, dist, (float)Math.Cos(t + offset) * dist);
        }
    }
}
