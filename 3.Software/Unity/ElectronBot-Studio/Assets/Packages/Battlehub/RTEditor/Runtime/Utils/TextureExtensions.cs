using UnityEngine;

namespace Battlehub.Utils
{
    public static class TextureExtensions
    {
        public static bool IsReadable(this Texture2D texture)
        {
            if (texture == null)
            {
                return false;
            }
            try
            {
                texture.GetPixel(0, 0);
                return true;
            }
            catch (UnityException)
            {
                return false;
            }
        }

        public static Texture2D DeCompress(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.sRGB);
            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        // Converts this RenderTexture to a RenderTexture in ARGB32 format.
        // Also return the original RenderTexture if it's already in ARGB32 format.
        // The resulting temporary RenderTexture should be released if no longer used to prevent a memory leak?

        public static RenderTexture ConvertToARGB32(this RenderTexture self)
        {
            if (self.format == RenderTextureFormat.ARGB32) return self;
            RenderTexture result = RenderTexture.GetTemporary(self.width, self.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(self, result);
            return result;
        }
    }


}

