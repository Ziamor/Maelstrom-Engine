﻿using Assimp;
using Assimp.Configs;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maelstrom_Engine {
    public class Model {
        List<Mesh> meshes;

        public Model(string fileName) {
            meshes = new List<Mesh>();

            LoadModel(fileName);
        }

        public void Render(Transform transform, Camera camera) {
            for (int i = 0; i < meshes.Count; i++) {
                meshes[i].Render(transform, camera);
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
                        Console.WriteLine($"Import of {fileName} FAILED!");
                        return;
                    }

                    ProcessNode(scene.RootNode, scene);
                }
                catch (Exception e) {
                    Console.WriteLine($"ERROR: Somthing went wrong while loading the mesh {fileName}. " + e.Message);
                }
            }
        }

        private void ProcessNode(Node node, Scene scene) {
            List<int> assImpNodeMeshIndicies = node.MeshIndices;
            for (int i = 0; i < assImpNodeMeshIndicies.Count; i++) {
                Assimp.Mesh assImpMesh = scene.Meshes[i];

                Mesh newMesh = ProcessMesh(assImpMesh, scene);
                meshes.Add(newMesh);
            }

            foreach (Node child in node.Children) {
                ProcessNode(child, scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh assImpMesh, Scene scene) {
            Vertex[] vertices = ProcessVerticies(assImpMesh);
            uint[] meshIndices = ProcessIndices(assImpMesh);
            Material material = ProcessMaterials(assImpMesh, scene);

            return new Mesh(vertices, meshIndices, material);
        }

        private Vertex[] ProcessVerticies(Assimp.Mesh assImpMesh) {
            List<Assimp.Vector3D> assImpMeshVertices = assImpMesh.Vertices;
            List<Assimp.Vector3D>[] assImpMeshTextureCoords = assImpMesh.TextureCoordinateChannels;

            Vertex[] vertices = new Vertex[assImpMeshVertices.Count];
            for (int i = 0; i < assImpMeshVertices.Count; i++) {
                Assimp.Vector3D position = assImpMeshVertices[i];
                Vertex vertex = new Vertex(position.X, position.Y, position.Z, 0, 0);

                if (assImpMesh.HasTextureCoords(0)) {
                    Assimp.Vector3D texCoords = assImpMeshTextureCoords[0][i];
                    vertex.TextureCoord = new Vector2(texCoords.X, texCoords.Y);
                }

                vertices[i] = vertex;
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

        private Material ProcessMaterials(Assimp.Mesh assImpMesh, Scene scene) {
            List<Texture> textures = new List<Texture>();
            Assimp.Material assImpMeshMaterial = scene.Materials[assImpMesh.MaterialIndex];
            TextureSlot[] textureSlots = assImpMeshMaterial.GetMaterialTextures(TextureType.Diffuse);
            for (int i = 0; i < textureSlots.Length; i++) {
                TextureSlot textureSlot = textureSlots[i];
                // FBX files store the file name as the character * followed by a number the represents the index for an embedded texture
                if (Regex.IsMatch(textureSlot.FilePath, @"\\*\d+")) {
                    int embeddedTextureIndex = int.Parse(textureSlot.FilePath.TrimStart('*'));
                    Assimp.EmbeddedTexture tex = scene.Textures[embeddedTextureIndex];
                    textures.Add(Texture.LoadTextureFromEmbeddedTexture(tex));
                }
            }
            Material material = new Material(textures, Game.defaultDiffuseShader);
            return material;
        }
    }
}