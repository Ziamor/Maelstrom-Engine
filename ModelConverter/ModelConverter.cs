using Assimp;
using Assimp.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MaelstromEngine;
using Mesh = MaelstromEngine.Mesh;
using Material = MaelstromEngine.Material;
using System.Text.RegularExpressions;
using OpenTK;
using Newtonsoft.Json;

namespace ModelConverter {
    public class ModelConverter {
        string fileName;
        List<MeshData> meshes;

        public ModelConverter(string fileName) {
            this.fileName = fileName;
            meshes = new List<MeshData>();

            LoadModel(fileName);
        }

        private void LoadModel(string fileName) {

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", fileName);

            using (AssimpContext importer = new AssimpContext()) {
                try {
                    importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                    Scene scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeQuality | PostProcessSteps.FlipWindingOrder);

                    if (scene != null && scene.HasMeshes)
                        Console.WriteLine($"Loading {fileName}");
                    else {
                        Console.WriteLine($"ERROR: Failed to load {fileName}");
                        return;
                    }

                    ProcessNode(scene.RootNode, scene);

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    settings.Formatting = Formatting.Indented;

                    string modelData = JsonConvert.SerializeObject(meshes, settings);
                    System.IO.File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", fileName + ".json"), modelData);

                    Console.WriteLine($"Finished loading {fileName}");
                }
                catch (Exception e) {
                    Console.WriteLine($"ERROR: Somthing went wrong while loading the mesh {fileName}. " + e.Message);
                }
            }
        }

        private void ProcessNode(Node node, Scene scene) {
            List<int> assImpNodeMeshIndicies = node.MeshIndices;
            for (int i = 0; i < assImpNodeMeshIndicies.Count; i++) {
                Assimp.Mesh assImpMesh = scene.Meshes[assImpNodeMeshIndicies[i]];

                MeshData newMesh = ProcessMesh(assImpMesh, scene);
                meshes.Add(newMesh);
            }

            foreach (Node child in node.Children) {
                ProcessNode(child, scene);
            }
        }

        private MeshData ProcessMesh(Assimp.Mesh assImpMesh, Scene scene) {
            Vertex[] vertices = ProcessVerticies(assImpMesh);
            uint[] meshIndices = ProcessIndices(assImpMesh);
            MaterialData material = ProcessMaterials(assImpMesh, scene);

            return new MeshData(vertices, meshIndices, material);
        }

        private Vertex[] ProcessVerticies(Assimp.Mesh assImpMesh) {
            List<Assimp.Vector3D> assImpMeshVertices = assImpMesh.Vertices;
            List<Assimp.Vector3D> assImpMeshNormals = assImpMesh.Normals;
            List<Assimp.Vector3D> assImpMeshTangents = assImpMesh.Tangents;
            List<Assimp.Vector3D>[] assImpMeshTextureCoords = assImpMesh.TextureCoordinateChannels;

            Vertex[] vertices = new Vertex[assImpMeshVertices.Count];
            for (int i = 0; i < assImpMeshVertices.Count; i++) {
                Assimp.Vector3D position = assImpMeshVertices[i];
                Vector3 pos = new Vector3(position.X, position.Y, position.Z);
                Vector3 normals = new Vector3(0, 0, 0);
                Vector3 tangent = new Vector3(0, 0, 0);
                Vector2 texCoords = new Vector2(0, 0);

                if (assImpMesh.HasNormals) {
                    Assimp.Vector3D assImpMeshNormal = assImpMeshNormals[i];
                    normals.X = assImpMeshNormal.X;
                    normals.Y = assImpMeshNormal.Y;
                    normals.Z = assImpMeshNormal.Z;
                }

                /*if (assImpMesh.HasTangentBasis) {
                    Assimp.Vector3D assImpMeshTangent = assImpMeshTangents[i];
                    tangent.X = assImpMeshTangent.X;
                    tangent.Y = assImpMeshTangent.Y;
                    tangent.Z = assImpMeshTangent.Z;
                }*/

                if (assImpMesh.HasTextureCoords(0)) {
                    Assimp.Vector3D assImpMeshTexCoord = assImpMeshTextureCoords[0][i];
                    texCoords.X = assImpMeshTexCoord.X;
                    texCoords.Y = assImpMeshTexCoord.Y;
                }

                vertices[i] = new Vertex(pos, normals, tangent, texCoords);
            }

            return vertices;
        }

        private uint[] ProcessIndices(Assimp.Mesh assImpMesh) {
            List<Face> assImpMeshFaces = assImpMesh.Faces;
            List<uint> meshIndices = new List<uint>(assImpMesh.FaceCount * 3);
            for (int i = 0; i < assImpMeshFaces.Count; i++) {
                List<int> faceIndices = assImpMeshFaces[i].Indices;
                for (int j = 0; j < faceIndices.Count; j++) {
                    meshIndices.Add((uint)faceIndices[j]);
                }
            }

            return meshIndices.ToArray();
        }

        private MaterialData ProcessMaterials(Assimp.Mesh assImpMesh, Scene scene) {
            Assimp.Material assImpMeshMaterial = scene.Materials[assImpMesh.MaterialIndex];
            TextureData diffuse = ProcessTexture(TextureType.Diffuse, scene, assImpMeshMaterial);
            TextureData normalMap = ProcessTexture(TextureType.Height, scene, assImpMeshMaterial);
            TextureData specularMap = ProcessTexture(TextureType.Specular, scene, assImpMeshMaterial);

            MaterialData material = new MaterialData(diffuse, normalMap, specularMap);

            return material;
        }

        private static TextureData ProcessTexture(TextureType textureType, Scene scene, Assimp.Material assImpMeshMaterial) {
            TextureData texture = new TextureData();
            TextureSlot textureSlot;
            assImpMeshMaterial.GetMaterialTexture(textureType, 0, out textureSlot);

            if (textureSlot.FilePath != null)
                texture.fileName = textureSlot.FilePath;

            return texture;
        }
    }
}