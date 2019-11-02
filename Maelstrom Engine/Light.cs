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
        float offset = 0;
        Vector4 lightColor;
        float t;
        public Light(Vector4 lightColor, float offset) {
            this.lightColor = lightColor;
            lightMesh = new Model("sphere.obj.json");
            lightMesh.OverrideMaterial(new Material(null, null, null, Game.defaultLightShader));
            transform = new Transform(new Vector3(0, 2, 0), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f));
        }

        public void Render(Transform t, Camera camera) {
            Game.defaultDiffuseShader.SetVec4("lightColor", lightColor);
            Game.defaultLightShader.SetVec4("lightColor", lightColor);
            lightMesh.Render(transform, camera);
        }

        public void Update(float deltaTime) {
            t += deltaTime;
            transform.position = new Vector3((float)Math.Sin(t) * 20 + offset, (float)Math.Sin(t) * 20 + offset, (float)Math.Cos(t) * 20 + offset);
        }
    }
}
