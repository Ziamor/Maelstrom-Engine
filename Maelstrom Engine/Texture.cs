using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maelstrom_Engine
{
    public class Texture
    {
        public readonly int Handle;

        public Texture(string path)
        {
            Handle = GL.GenTexture();
            Use();

            using (Image<Rgba32> image = Image.Load<Rgba32>(path))
            {

                image.Mutate(x => x.Flip(FlipMode.Vertical));

                Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

                List<byte> pixels = new List<byte>();
                foreach (Rgba32 p in tempPixels)
                {
                    pixels.Add(p.R);
                    pixels.Add(p.G);
                    pixels.Add(p.B);
                    pixels.Add(p.A);
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
