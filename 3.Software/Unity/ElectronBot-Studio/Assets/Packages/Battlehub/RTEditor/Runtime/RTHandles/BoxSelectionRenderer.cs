using Battlehub.RTCommon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityObject = UnityEngine.Object;
namespace Battlehub.RTHandles
{
    public static class BoxSelectionRenderer
    {
        private static RenderTextureFormat s_renderTextureFormat;
        private static bool s_initialized;
        private static RenderTextureFormat[] s_preferredFormats = new RenderTextureFormat[]
        {
            RenderTextureFormat.ARGB32,
            RenderTextureFormat.ARGBFloat,
        };

        private static RenderTextureFormat RenderTextureFormat
        {
            get
            {
                Init();
                return s_renderTextureFormat;
            }
        }

        private static TextureFormat TextureFormat { get { return TextureFormat.ARGB32; } }
        private static Shader s_objectSelectionShader;
        private static Shader ObjectSelectionShader
        {
            get
            {
                Init();
                return s_objectSelectionShader;
            }
        }

        private static void Init()
        {
            if (s_initialized)
            {
                return;
            }

            s_initialized = true;
            s_objectSelectionShader = Shader.Find("Battlehub/RTHandles/BoxSelectionShader");

            for (int i = 0; i < s_preferredFormats.Length; i++)
            {
                if (SystemInfo.SupportsRenderTextureFormat(s_preferredFormats[i]))
                {
                    s_renderTextureFormat = s_preferredFormats[i];
                    break;
                }
            }
        }

        public static Color32[] Render(Camera camera, Renderer[] renderers, Vector2Int reqiestedTexSize,  out Vector2Int texSize)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                Material[] materials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
                {
                    MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(propertyBlock, materialIndex);
                    propertyBlock.SetColor("_SelectionColor", EncodeRGBA((uint)i + 1));
                    renderer.SetPropertyBlock(propertyBlock, materialIndex);
                }
            }

            Texture2D tex = Render(camera, ObjectSelectionShader, renderers, reqiestedTexSize.x, reqiestedTexSize.y);
            Color32[] texPizels = tex.GetPixels32();
            texSize = new Vector2Int(tex.width, tex.height);
            UnityObject.DestroyImmediate(tex);
            return texPizels;
        }

        public static Renderer[] PickRenderersInRect(Camera camera, Rect selectionRect, Renderer[] renderers, Color32[] texPixels, Vector2Int texSize)
        {
            selectionRect.width /= camera.rect.width;
            selectionRect.height /= camera.rect.height;
            selectionRect.x = (selectionRect.x - camera.pixelRect.x) / camera.rect.width;
            selectionRect.y = (selectionRect.y - (texSize.y - (camera.pixelRect.y + camera.pixelRect.height))) / camera.rect.height;

            int ox = System.Math.Max(0, Mathf.FloorToInt(selectionRect.x));
            int oy = System.Math.Max(0, Mathf.FloorToInt(texSize.y - selectionRect.y - selectionRect.height));

            int width = Mathf.FloorToInt(selectionRect.width);
            int height = Mathf.FloorToInt(selectionRect.height);

            List<Renderer> selectedRenderers = new List<Renderer>();
            HashSet<int> used = new HashSet<int>();

            for (int y = oy; y < System.Math.Min(oy + height, texSize.y); y++)
            {
                for (int x = ox; x < System.Math.Min(ox + width, texSize.x); x++)
                {
                    int index = (int)DecodeRGBA(texPixels[y * texSize.x + x]) - 1;
                    if (index < 0 || index >= renderers.Length)
                    {
                        continue;
                    }

                    if (used.Add(index))
                    {
                        Renderer selectedRenderer = renderers[index];
                        selectedRenderers.Add(selectedRenderer);

                        if (selectedRenderers.Count == renderers.Length)
                        {
                            return selectedRenderers.ToArray();
                        }
                    }
                }
            }

            return selectedRenderers.ToArray();
        }

        public static Renderer[] PickRenderersInRect(
            Camera camera,
            Rect selectionRect,
            Renderer[] renderers,
            int renderTextureWidth = -1,
            int renderTextureHeight = -1)
        {

            Vector2Int texSize;
            Color32[] pixels = Render(camera, renderers, new Vector2Int(renderTextureWidth, renderTextureHeight), out texSize);
            return PickRenderersInRect(camera, selectionRect, renderers, pixels, texSize);
        }

     
        private static uint DecodeRGBA(Color32 color)
        {
            uint r = color.r;
            uint g = color.g;
            uint b = color.b;

            if (System.BitConverter.IsLittleEndian)
            {
                return r << 16 | g << 8 | b;
            }

            return r << 24 | g << 16 | b << 8;
        }

        private static Color32 EncodeRGBA(uint hash)
        {
            if (System.BitConverter.IsLittleEndian)
                return new Color32(
                    (byte)(hash >> 16 & 0xFF),
                    (byte)(hash >> 8 & 0xFF),
                    (byte)(hash & 0xFF),
                    (byte)(255));
            else
                return new Color32(
                    (byte)(hash >> 24 & 0xFF),
                    (byte)(hash >> 16 & 0xFF),
                    (byte)(hash >> 8 & 0xFF),
                    (byte)(255));
        }

        private static Texture2D Render(
            Camera camera,
            Shader shader,
            Renderer[] renderers,
            int width = -1,
            int height = -1)
        {

            bool autoSize = width < 0 || height < 0;

            int _width = autoSize ? (int)camera.pixelRect.width : width;
            int _height = autoSize ? (int)camera.pixelRect.height : height;

            GameObject go = new GameObject();
            Camera renderCam = go.AddComponent<Camera>();
            renderCam.CopyFrom(camera);

            renderCam.renderingPath = RenderingPath.Forward;
            renderCam.enabled = false;
            renderCam.clearFlags = CameraClearFlags.SolidColor;
            renderCam.backgroundColor = Color.white;
            renderCam.cullingMask = 0;

            IRenderPipelineCameraUtility cameraUtility = IOC.Resolve<IRenderPipelineCameraUtility>();
            if (cameraUtility != null)
            {
                cameraUtility.EnablePostProcessing(renderCam, false);
                cameraUtility.SetBackgroundColor(renderCam, Color.white);
            }

            renderCam.allowHDR = false;
            renderCam.allowMSAA = false;
            renderCam.forceIntoRenderTexture = true;

            float aspect = renderCam.aspect;
            renderCam.rect = new Rect(Vector2.zero, Vector2.one);
            renderCam.aspect = aspect;

            RenderTextureDescriptor descriptor = new RenderTextureDescriptor()
            {
                width = _width,
                height = _height,
                colorFormat = RenderTextureFormat,
                autoGenerateMips = false,
                depthBufferBits = 16,
                dimension = TextureDimension.Tex2D,
                enableRandomWrite = false,
                memoryless = RenderTextureMemoryless.None,
                sRGB = true,
                useMipMap = false,
                volumeDepth = 1,
                msaaSamples = 1
            };
            RenderTexture rt = RenderTexture.GetTemporary(descriptor);

            RenderTexture prev = RenderTexture.active;
            renderCam.targetTexture = rt;
            RenderTexture.active = rt;

            Material replacementMaterial = new Material(shader);

            IRTEGraphics graphics = IOC.Resolve<IRTEGraphics>();
            IRTECamera rteCamera = graphics.CreateCamera(renderCam, CameraEvent.AfterForwardAlpha, false, true);
            rteCamera.RenderersCache.MaterialOverride = replacementMaterial;
            rteCamera.Camera.name = "BoxSelectionCamera";
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; ++i)
                {
                    if (materials[i] != null)
                    {
                        rteCamera.RenderersCache.Add(renderer);
                    }
                }
            }
            rteCamera.RefreshCommandBuffer();

            if (RenderPipelineInfo.Type != RPType.Standard)
            {
                bool invertCulling = GL.invertCulling;
                GL.invertCulling = true;
                renderCam.projectionMatrix *= Matrix4x4.Scale(new Vector3(1, -1, 1));
                renderCam.Render();
                GL.invertCulling = invertCulling;
            }
            else
            {
                renderCam.Render();
            }

            Texture2D img = new Texture2D(_width, _height, TextureFormat, false, false);
            img.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            img.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            UnityObject.DestroyImmediate(go);
            UnityObject.Destroy(replacementMaterial);

            rteCamera.Destroy();
            //System.IO.File.WriteAllBytes("Assets/box_selection.png", img.EncodeToPNG());

            return img;
        }
    }
}

