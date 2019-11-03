using Assimp;
using Assimp.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maelstrom {
    public class Model : Renderable {
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

        public void OverrideMaterial(Material newMaterial) {
            for (int i = 0; i < meshes.Count; i++) {
                meshes[i].Material = newMaterial;
            }
        }

        private void LoadModel(string fileName) {

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", fileName);
            string jsonData = File.ReadAllText(path);
            MeshData[] meshesData = JsonConvert.DeserializeObject<MeshData[]>(jsonData);
            for (int i = 0; i < meshesData.Length; i++) {
                MeshData meshdata = meshesData[i];
                meshes.Add(ProcessMeshData(meshdata));
            }
        }

        private Mesh ProcessMeshData(MeshData meshData) {
            Texture diffuse;
            if (meshData.materialData.diffuse.fileName != null && !meshData.materialData.diffuse.fileName.Equals("")) {
                diffuse = Texture.LoadTextureFromPath("Assets\\" + Path.GetFileName(meshData.materialData.diffuse.fileName));
            } else {
                diffuse = Game.defaultDiffuseTexture;
            }

            Texture specular;
            if (meshData.materialData.specularMap.fileName != null && !meshData.materialData.specularMap.fileName.Equals("")) {
                specular = Texture.LoadTextureFromPath("Assets\\" + Path.GetFileName(meshData.materialData.specularMap.fileName));
            }
            else {
                specular = Game.defaultSpecularTexture;
            }

            Material material = new Material(diffuse, null, specular, Game.defaultDiffuseShader);
            Mesh mesh = new Mesh(meshData.vertices, meshData.indices, material);

            return mesh;
        }

        private static Texture LoadTexture(Scene scene, TextureSlot textureSlot) {
            Texture diffuse;
            // For FBX, textures can be embedded and have a filepath value in the format * followed by a number which is the index for the texture in the scene
            if (Regex.IsMatch(textureSlot.FilePath, @"\*\d+")) {
                int embeddedTextureIndex = int.Parse(textureSlot.FilePath.TrimStart('*'));
                Assimp.EmbeddedTexture tex = scene.Textures[embeddedTextureIndex];
                diffuse = Texture.LoadTextureFromEmbeddedTexture(tex);
            }
            else {
                diffuse = Texture.LoadTextureFromPath("Assets\\" + Path.GetFileName(textureSlot.FilePath));
            }

            return diffuse;
        }
    }


    public class VertexConvertor : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var vertices = value as Vertex[];

            writer.WriteStartObject();
            writer.WritePropertyName("size");
            writer.WriteValue(vertices.Length);

            writer.WritePropertyName("values");
            writer.WriteStartArray();
            for (int i = 0; i < vertices.Length; i++) {
                writer.WriteStartObject();

                writer.WritePropertyName("position");
                WriteVector3(vertices[i].Position, writer);

                writer.WritePropertyName("normal");
                WriteVector3(vertices[i].Normal, writer);

                writer.WritePropertyName("texCoord");
                WriteVector2(vertices[i].TextureCoord, writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JObject jObject = JObject.Load(reader);
            var properties = jObject.Properties().ToList();
            Vertex[] vertices = new Vertex[(int)properties[0].Value];

            JToken verticesJToken = properties[1].Value;
            for (int i = 0; i < vertices.Length; i++) {
                JToken vertexToken = verticesJToken[i];
                Vector3 position = ReadVector3(vertexToken["position"]);
                Vector3 normal = ReadVector3(vertexToken["normal"]);
                Vector2 texCoord = ReadVector2(vertexToken["texCoord"]);

                vertices[i] = new Vertex(position, normal, new Vector3(), texCoord);
            }
            return vertices;
        }

        private Vector2 ReadVector2(JToken token) {
            Vector2 vec = new Vector2((float)token[0], (float)token[1]);
            return vec;
        }
        private Vector3 ReadVector3(JToken token) {
            Vector3 vec = new Vector3((float)token[0], (float)token[1], (float)token[2]);
            return vec;
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Vertex);
        }

        private void WriteVector3(Vector3 vec, JsonWriter writer) {
            writer.WriteStartArray();
            writer.WriteValue(vec.X);
            writer.WriteValue(vec.Y);
            writer.WriteValue(vec.Z);
            writer.WriteEndArray();
        }

        private void WriteVector2(Vector2 vec, JsonWriter writer) {
            writer.WriteStartArray();
            writer.WriteValue(vec.X);
            writer.WriteValue(vec.Y);
            writer.WriteEndArray();
        }
    }

    public struct MeshData {
        [JsonConverter(typeof(VertexConvertor))]
        public Vertex[] vertices;
        public uint[] indices;
        public MaterialData materialData;

        public MeshData(Vertex[] vertices, uint[] indices, MaterialData materialData) {
            this.vertices = vertices;
            this.indices = indices;
            this.materialData = materialData;
        }
    }

    public struct MaterialData {
        public TextureData diffuse;
        public TextureData normalMap;
        public TextureData specularMap;

        public MaterialData(TextureData diffuse, TextureData normalMap, TextureData specularMap) {
            this.diffuse = diffuse;
            this.normalMap = normalMap;
            this.specularMap = specularMap;
        }
    }

    public struct TextureData {
        public string fileName;
    }
}