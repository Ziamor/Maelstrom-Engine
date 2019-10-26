using Assimp;
using Assimp.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom_Engine {
    public class Model {
        List<Mesh> meshes;

        public Model(string fileName) {
            meshes = new List<Mesh>();

            LoadModel(fileName);
        }

        public void Render(Transform transform, Camera camera, Material material) {
            for (int i = 0; i < meshes.Count; i++) {
                meshes[i].Render(transform, camera, material);
            }
        }

        private void LoadModel(string fileName) {

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", fileName);

            using (AssimpContext importer = new AssimpContext()) {
                try {
                    importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                    Scene scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeQuality | PostProcessSteps.FlipWindingOrder);

                    if (scene != null && scene.HasMeshes)
                        Console.WriteLine($"Import of {fileName} SUCCESSFUL!");
                    else {
                        Console.WriteLine("Import FAILED!");
                        return;
                    }

                    ProcessNode(scene.RootNode, scene);
                }
                catch (Exception e) {
                    Console.WriteLine("ERROR: Somethign went wrong while processing the node");
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void ProcessNode(Node node, Scene scene) {
            List<int> meshIndicies = node.MeshIndices;
            for (int i = 0; i < meshIndicies.Count; i++) {
                Assimp.Mesh assImpMesh = scene.Meshes[i];
                Mesh mesh = ProcessMesh(assImpMesh);
                if (mesh != null) {
                    meshes.Add(mesh);
                }
            }

            foreach (Node child in node.Children) {
                ProcessNode(child, scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh assImpMesh) {
            try {
                List<Assimp.Vector3D> assImpMeshVertices = assImpMesh.Vertices;
                Vertex[] vertices = new Vertex[assImpMeshVertices.Count];
                for (int i = 0; i < assImpMeshVertices.Count; i++) {
                    Assimp.Vector3D position = assImpMeshVertices[i];
                    vertices[i] = new Vertex(position.X, position.Y, position.Z, 0, 0);
                }

                List<Face> assImpMeshFaces = assImpMesh.Faces;
                List<uint> meshIndices = new List<uint>(assImpMesh.FaceCount * 3);
                for (int i = 0; i < assImpMeshFaces.Count; i++) {
                    List<int> faceIndices = assImpMeshFaces[i].Indices;
                    for (int j = 0; j < faceIndices.Count; j++) {
                        meshIndices.Add((uint)faceIndices[j]);
                    }
                }

                return new Mesh(vertices, meshIndices.ToArray());
            }
            catch (Exception e) {
                Console.WriteLine("ERROR: Somethign went wrong while processing the mesh");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
