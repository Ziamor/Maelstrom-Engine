using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maelstrom_Engine {
    public class Texture {
        public readonly int Handle;

        private Texture() {
            Handle = GL.GenTexture();
        }

        public static Texture CreateTexture(Image<Rgba32> image) {
            Texture texture = new Texture();
            texture.Use();

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

            List<byte> pixels = new List<byte>();
            foreach (Rgba32 p in tempPixels) {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texture;
        }

        public static Texture LoadTextureFromPath(string path) {
            using (Image<Rgba32> image = Image.Load<Rgba32>(path)) {
                return Texture.CreateTexture(image);               
            }
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0) {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public static Texture LoadTextureFromEmbeddedTexture(EmbeddedTexture embeddedTexture) {
            try {
                if (embeddedTexture.IsCompressed) {
                    Image<Rgba32> image = Image.Load(embeddedTexture.CompressedData);
                    return Texture.CreateTexture(image);
                }
                else {
                    Console.WriteLine("Embedded texture is NOT compressed, needs to be implemented");
                }
            } catch(Exception e) {
                Console.WriteLine("ERROR: Failed to load embedded texture " + e.Message);
            }

            return null;
        }
    }
}
