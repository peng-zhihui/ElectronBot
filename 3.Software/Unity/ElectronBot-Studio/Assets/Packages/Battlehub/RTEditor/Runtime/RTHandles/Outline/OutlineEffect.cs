using Battlehub.RTCommon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles
{
    public class OutlineEffect : MonoBehaviour
    {
        public Color OutlineColor = new Color(1, 0.35f, 0, .05f);
        public CameraEvent BufferDrawEvent = CameraEvent.BeforeImageEffects;
        [Range(0, 1)]
        public int Downsample = 0;
        [Range(0.0f, 3.0f)]
        public float BlurSize = 0.9f;

        private CommandBuffer m_commandBuffer;
        private int m_outlineRTID;
        private int m_blurredRTID;
        private int m_temporaryRTID;
        private int m_depthRTID;
        private int m_idRTID;

        private List<Renderer> m_objectRenderers;
        private HashSet<Renderer> m_objectRenderersHs;

        private List<ICustomOutlinePrepass> m_customObjectRenderers;
        private HashSet<ICustomOutlinePrepass> m_customObjectRenderersHs;

        private Material m_outlineMaterial;
        private Camera m_camera;
        private int m_rtWidth = 512;
        private int m_rtHeight = 512;

        private Rect m_prevRect;

        public bool ContainsRenderer(Renderer renderer)
        {
            return m_objectRenderersHs.Contains(renderer);
        }

        public void AddRenderers(Renderer[] renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                if (!m_objectRenderersHs.Contains(renderer))
                {
                    m_objectRenderers.Add(renderer);
                    m_objectRenderersHs.Add(renderer);
                }
            }

            RecreateCommandBuffer();
        }

        public void RemoveRenderers(Renderer[] renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                m_objectRenderers.Remove(renderer);
                m_objectRenderersHs.Remove(renderer);
            }

            RecreateCommandBuffer();
        }

        public void AddRenderers(ICustomOutlinePrepass[] renderers)
        {
            foreach (ICustomOutlinePrepass renderer in renderers)
            {
                if (!m_customObjectRenderersHs.Contains(renderer))
                {
                    m_customObjectRenderers.Add(renderer);
                    m_customObjectRenderersHs.Add(renderer);
                }
            }

            RecreateCommandBuffer();
        }

        public void RemoveRenderers(ICustomOutlinePrepass[] renderers)
        {
            foreach (ICustomOutlinePrepass renderer in renderers)
            {
                m_customObjectRenderers.Remove(renderer);
                m_customObjectRenderersHs.Remove(renderer);
            }

            RecreateCommandBuffer();
        }

        public void ClearOutlineData()
        {
            m_objectRenderers.Clear();
            m_objectRenderersHs.Clear();
            RecreateCommandBuffer();
        }

        private void Awake()
        {
            m_objectRenderers = new List<Renderer>();
            m_objectRenderersHs = new HashSet<Renderer>();
            m_customObjectRenderers = new List<ICustomOutlinePrepass>();
            m_customObjectRenderersHs = new HashSet<ICustomOutlinePrepass>();

            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.name = "UnityOutlineFX Command Buffer";

            m_depthRTID = Shader.PropertyToID("_DepthRT");
            m_outlineRTID = Shader.PropertyToID("_OutlineRT");
            m_blurredRTID = Shader.PropertyToID("_BlurredRT");
            m_temporaryRTID = Shader.PropertyToID("_TemporaryRT");
            m_idRTID = Shader.PropertyToID("_idRT");

            m_outlineMaterial = new Material(Shader.Find("Hidden/UnityOutline"));

            m_camera = GetComponent<Camera>();

            m_camera.depthTextureMode = DepthTextureMode.Depth;
            m_camera.AddCommandBuffer(BufferDrawEvent, m_commandBuffer);
        }

        public void RecreateCommandBuffer()
        {
            if(m_camera == null || m_commandBuffer == null)
            {
                return;
            }

            int antialiasing = m_camera.allowMSAA ? Mathf.Max(1, RenderPipelineInfo.MSAASampleCount) : 1;
            RenderTargetIdentifier depthRTID = BuiltinRenderTextureType.Depth;
            if (m_camera.targetTexture != null && RenderPipelineInfo.Type == RPType.Standard)
            {
                m_rtWidth = Screen.width;
                m_rtHeight = Screen.height;
                depthRTID = BuiltinRenderTextureType.CurrentActive;
            }
            else
            {
                m_rtWidth = m_camera.pixelWidth;
                m_rtHeight = m_camera.pixelHeight;
          
                if (antialiasing != 1)
                {
                    depthRTID = BuiltinRenderTextureType.CurrentActive;
                }
            }

            m_commandBuffer.Clear();

            if (m_objectRenderers.Count == 0 && m_customObjectRenderers.Count == 0)
            {
                return;
            }

          
            FilterMode filterMode = FilterMode.Point;
            // initialization
            m_commandBuffer.GetTemporaryRT(m_depthRTID, m_rtWidth, m_rtHeight, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antialiasing);
            m_commandBuffer.SetRenderTarget(m_depthRTID, depthRTID);
            m_commandBuffer.ClearRenderTarget(false, true, Color.clear);

            if (m_camera.targetTexture != null && RenderPipelineInfo.Type == RPType.Standard)
            {
                m_commandBuffer.SetViewport(m_camera.pixelRect);
            }


            // render selected objects into a mask buffer, with different colors for visible vs occluded ones 
            float id = 0f;
            for (int i = m_objectRenderers.Count - 1; i >= 0; --i)
            {
                Renderer renderer = m_objectRenderers[i];
                if (renderer != null)
                {
                    if (((1 << renderer.gameObject.layer) & m_camera.cullingMask) != 0 && renderer.enabled)
                    {
                        id += 0.25f;
                        m_commandBuffer.SetGlobalFloat("_ObjectId", id);

                        int submeshCount = renderer.sharedMaterials.Length;
                        for (int s = 0; s < submeshCount; ++s)
                        {
                            m_commandBuffer.DrawRenderer(renderer, m_outlineMaterial, s, 1);
                            m_commandBuffer.DrawRenderer(renderer, m_outlineMaterial, s, 0);
                        }
                    }
                }
                else
                {
                    m_objectRenderers.Remove(renderer);
                    m_objectRenderersHs.Remove(renderer);
                }

            }

            for (int i = m_customObjectRenderers.Count - 1; i >= 0; --i)
            {
                ICustomOutlinePrepass renderer = m_customObjectRenderers[i];
                if (renderer != null && renderer.GetRenderer() != null)
                {
                    if (((1 << renderer.GetRenderer().gameObject.layer) & m_camera.cullingMask) != 0 && renderer.GetRenderer().enabled)
                    {
                        id += 0.25f;
                        m_commandBuffer.SetGlobalFloat("_ObjectId", id);

                        int submeshCount = renderer.GetRenderer().sharedMaterials.Length;
                        for (int s = 0; s < submeshCount; ++s)
                        {
                            m_commandBuffer.DrawRenderer(renderer.GetRenderer(), renderer.GetOutlinePrepassMaterial(), s);
                        }
                    }
                }
                else
                {
                    m_customObjectRenderers.Remove(renderer);
                    m_customObjectRenderersHs.Remove(renderer);
                }

            }

            // object ID edge dectection pass
            m_commandBuffer.GetTemporaryRT(m_idRTID, m_rtWidth, m_rtHeight, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antialiasing);
            m_commandBuffer.Blit(m_depthRTID, m_idRTID, m_outlineMaterial, 3);

            // Blur
            int rtW = m_rtWidth >> Downsample;
            int rtH = m_rtHeight >> Downsample;

            m_commandBuffer.GetTemporaryRT(m_temporaryRTID, rtW, rtH, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antialiasing);
            m_commandBuffer.GetTemporaryRT(m_blurredRTID, rtW, rtH, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antialiasing);

            m_commandBuffer.Blit(m_idRTID, m_blurredRTID);

            m_commandBuffer.SetGlobalVector("_BlurDirection", new Vector2(BlurSize, 0));
            m_commandBuffer.Blit(m_blurredRTID, m_temporaryRTID, m_outlineMaterial, 2);
            m_commandBuffer.SetGlobalVector("_BlurDirection", new Vector2(0, BlurSize));
            m_commandBuffer.Blit(m_temporaryRTID, m_blurredRTID, m_outlineMaterial, 2);

            // final overlay
            m_commandBuffer.SetGlobalColor("_OutlineColor", OutlineColor);
            m_commandBuffer.Blit(m_blurredRTID, BuiltinRenderTextureType.CameraTarget, m_outlineMaterial, 4);

            // release tempRTs
            m_commandBuffer.ReleaseTemporaryRT(m_blurredRTID);
            m_commandBuffer.ReleaseTemporaryRT(m_outlineRTID);
            m_commandBuffer.ReleaseTemporaryRT(m_temporaryRTID);
            m_commandBuffer.ReleaseTemporaryRT(m_depthRTID);
        }

        private void OnPreRender()
        {
            if (m_camera.pixelRect != m_prevRect)
            {
                m_prevRect = m_camera.pixelRect;
                RecreateCommandBuffer();
            }
        }
    }
}
