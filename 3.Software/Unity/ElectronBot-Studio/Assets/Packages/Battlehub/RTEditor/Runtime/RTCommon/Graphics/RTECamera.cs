using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
    public interface IRTECamera
    {
        event Action<IRTECamera> CommandBufferRefresh;

        Camera Camera
        {
            get;
        }

        CommandBuffer CommandBuffer
        {
            get;
        }

        CommandBuffer CommandBufferOverride
        {
            get;
            set;
        }

        CameraEvent Event
        {
            get;
            set;
        }

        IRenderersCache RenderersCache
        {
            get;
        }

        IMeshesCache MeshesCache
        {
            get;
        }

        void RefreshCommandBuffer();
        void Destroy();
    }

    public class RTECamera : MonoBehaviour, IRTECamera
    {
        public static event Action<IRTECamera> Created;
        public static event Action<IRTECamera> Destroyed;

        public event Action<IRTECamera> CommandBufferRefresh;

        private Camera m_camera;
        public Camera Camera
        {
            get { return m_camera; }
        }
        
        private CommandBuffer m_commandBuffer;
        public CommandBuffer CommandBuffer
        {
            get { return m_commandBufferOverride != null ? m_commandBufferOverride :  m_commandBuffer; }
        }

        private CommandBuffer m_commandBufferOverride;
        public CommandBuffer CommandBufferOverride
        {
            get { return m_commandBufferOverride; }
            set
            {
                m_commandBufferOverride = value;
                if(m_commandBufferOverride != null)
                {
                    RemoveCommandBuffer();
                }
                else
                {
                    CreateCommandBuffer();
                }
            }
        }


        [SerializeField]
        private CameraEvent m_cameraEvent = CameraEvent.BeforeImageEffects;
        public CameraEvent Event
        {
            get { return m_cameraEvent; }
            set
            {
                m_cameraEvent = value;

                if(m_commandBufferOverride == null)
                {
                    RemoveCommandBuffer();
                    CreateCommandBuffer();
                }
            }
        }

        private IRenderersCache m_renderersCache;
        private bool m_destroyRenderersCache;

        private IMeshesCache m_meshesCache;
        private bool m_destroyMeshesCache;

        public IRenderersCache RenderersCache
        {
            get { return m_renderersCache; }
            set 
            {
                DestroyRenderersCache();
                m_renderersCache = value; 
            }
        }

        public IMeshesCache MeshesCache
        {
            get { return m_meshesCache; }
            set 
            {
                DestroyMeshesCache();
                m_meshesCache = value; 
            }
        }


        private void Awake()
        {
            m_camera = GetComponent<Camera>();

            if (m_commandBufferOverride == null)
            {
                CreateCommandBuffer();
            }

            RefreshCommandBuffer();

            if (m_renderersCache != null)
            {
                m_renderersCache.Refreshed += OnRefresh;
            }

            if (m_meshesCache != null)
            {
                m_meshesCache.Refreshing += OnRefresh;
            }

            if (Created != null)
            {
                Created(this);
            }
        }


        private void OnDestroy()
        {
            if (m_renderersCache != null)
            {    
                m_renderersCache.Refreshed -= OnRefresh;
                DestroyRenderersCache();
            }

            if (m_meshesCache != null)
            {
                m_meshesCache.Refreshing -= OnRefresh;
                DestroyMeshesCache();
            }

            if (m_camera != null)
            {
                RemoveCommandBuffer();
            }

            if(Destroyed != null)
            {
                Destroyed(this);
            }
        }

        public void CreateRenderersCache()
        {
            DestroyRenderersCache();
            m_renderersCache = gameObject.AddComponent<RenderersCache>();
            m_destroyRenderersCache = true;
        }

        public void CreateMeshesCache()
        {
            DestroyMeshesCache();
            m_meshesCache = gameObject.AddComponent<MeshesCache>();
            m_destroyMeshesCache = true;
        }

        private void DestroyRenderersCache()
        {
            if (m_destroyRenderersCache && m_renderersCache != null)
            {
                m_renderersCache.Destroy();
                m_renderersCache = null;
            }
        }

        private void DestroyMeshesCache()
        {
            if (m_destroyMeshesCache && m_meshesCache != null)
            {
                m_meshesCache.Destroy();
                m_meshesCache = null;
            }
        }

        public void Destroy()
        {
            DestroyMeshesCache();
            DestroyRenderersCache();
            Destroy(this);
        }

        private void OnRefresh()
        {
            RefreshCommandBuffer();
        }

        private void CreateCommandBuffer()
        {
            if (m_commandBuffer != null || m_camera == null)
            {
                return;
            }
            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.name = "RTECameraCommandBuffer";
            m_camera.AddCommandBuffer(m_cameraEvent, m_commandBuffer);
        }

        private void RemoveCommandBuffer()
        {
            if (m_commandBuffer == null)
            {
                return;
            }
            m_camera.RemoveCommandBuffer(m_cameraEvent, m_commandBuffer);
            m_commandBuffer = null;
        }

        public void RefreshCommandBuffer()
        {
            if(Camera == null)
            {
                return;
            }

            CommandBuffer commandBuffer;
            if(m_commandBufferOverride == null)
            {
                if (m_commandBuffer == null)
                {
                    return;
                }

                m_commandBuffer.Clear();
                if (m_cameraEvent == CameraEvent.AfterImageEffects || m_cameraEvent == CameraEvent.AfterImageEffectsOpaque)
                {
                    m_commandBuffer.ClearRenderTarget(true, false, Color.black);
                }

                commandBuffer = m_commandBuffer;
            }
            else
            {
                commandBuffer = m_commandBufferOverride;
            }
            
            if(m_meshesCache != null)
            {
                IList<RenderMeshesBatch> batches = m_meshesCache.Batches;
                for (int i = 0; i < batches.Count; ++i)
                {
                    RenderMeshesBatch batch = batches[i];
                    if (batch.Material == null)
                    {
                        continue;
                    }

                    if (batch.Material.enableInstancing)
                    {
                        for (int j = 0; j < batch.Mesh.subMeshCount; ++j)
                        {
                            if (batch.Mesh != null)
                            {
                                commandBuffer.DrawMeshInstanced(batch.Mesh, j, batch.Material, -1, batch.Matrices, batch.Matrices.Length);
                            }
                        }
                    }
                    else
                    {
                        Matrix4x4[] matrices = batch.Matrices;
                        for (int m = 0; m < matrices.Length; ++m)
                        {
                            for (int j = 0; j < batch.Mesh.subMeshCount; ++j)
                            {
                                if (batch.Mesh != null)
                                {
                                    commandBuffer.DrawMesh(batch.Mesh, matrices[m], batch.Material, j, -1);
                                }
                            }
                        }
                    }
                }
            }


            if (m_renderersCache != null)
            {
                IList<Renderer> renderers = m_renderersCache.Renderers;
                for (int i = 0; i < renderers.Count; ++i)
                {
                    Renderer renderer = renderers[i];
                    Material[] materials = renderer.sharedMaterials;
                    for (int j = 0; j < materials.Length; ++j)
                    {
                        if(m_renderersCache.MaterialOverride != null)
                        {
                            commandBuffer.DrawRenderer(renderer, m_renderersCache.MaterialOverride, j, -1);
                        }
                        else
                        {
                            Material material = materials[j];
                            commandBuffer.DrawRenderer(renderer, material, j, -1);
                        }
                    }
                }
            }

            if (CommandBufferRefresh != null)
            {
                CommandBufferRefresh(this);
            }
        }
    }
}
