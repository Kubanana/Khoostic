using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using OpenTK.Graphics.OpenGL4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Khoostic.Rendering
{
    public static class TextureCache
    {
        private static readonly Dictionary<string, int> _cache = new();

        public static int GetOrCreateTexture(byte[] imageData)
        {
            string hash = Convert.ToBase64String(SHA1.Create().ComputeHash(imageData));

            if (_cache.TryGetValue(hash, out int existingId))
            {
                return existingId;
            }

            int textureId = LoadTextureFromBytes(imageData);
            _cache[hash] = textureId;

            return textureId;
        }

        private static int LoadTextureFromBytes(byte[] imageData)
        {
            var image = Image.Load<Rgba32>(imageData);

            byte[] pixels = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];
            image.CopyPixelDataTo(MemoryMarshal.Cast<byte, Rgba32>(pixels));

            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                level: 0,
                internalformat: PixelInternalFormat.Rgba,
                width: image.Width,
                height: image.Height,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: pixels
            );

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return texture;
        }
    }
}