using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    public enum RenderTextureUsage
    {
        UsePipelineSettings,
        Off,
        On
    }

    public class RuntimeCameraWindow : RuntimeWindow
    {
        public event Action CameraResized;

        [SerializeField]
        private RenderTextureUsage m_renderTextureUsage = RenderTextureUsage.UsePipelineSettings;
        public RenderTextureUsage RenderTextureUsage
        {
            get { return m_renderTextureUsage; }
            set { m_renderTextureUsage = value; }
        }

        [SerializeField]
        protected Camera m_camera;
        public override Camera Camera
        {
            get { return m_camera; }
            set
            {
                if (m_camera == value)
                {
                    return;
                }

                if (m_camera != null)
                {
                    ResetCullingMask();
                    UnregisterGraphicsCamera();
                }

                m_camera = value;

                if (m_camera != null)
                {
                    SetCullingMask();
                    if (WindowType == RuntimeWindowType.Scene)
                    {
                        RegisterGraphicsCamera();
                    }

                    RenderPipelineInfo.XRFix(Camera);

                    m_camera.depth = m_cameraDepth;
                }
            }
        }

        private int m_cameraDepth;
        public int CameraDepth
        {
            get { return m_cameraDepth; }
        }

        public virtual void SetCameraDepth(int depth)
        {
            m_cameraDepth = depth;
            if (m_camera != null)
            {
                m_camera.depth = m_cameraDepth;
            }
        }

        [SerializeField]
        private Pointer m_pointer;
        public override Pointer Pointer
        {
            get { return m_pointer; }
        }

        private Vector3 m_position;
        private Rect m_rect;
        private RectTransform m_rectTransform;

        protected override void AwakeOverride()
        {
            base.AwakeOverride();

            m_rectTransform = GetComponent<RectTransform>();

            if (Camera != null)
            {
                Image windowBackground = GetComponent<Image>();
                if (windowBackground != null)
                {
                    Color color = windowBackground.color;
                    color.a = 0;
                    windowBackground.color = color;
                }

                if (RenderTextureUsage == RenderTextureUsage.Off || RenderTextureUsage == RenderTextureUsage.UsePipelineSettings && !RenderPipelineInfo.UseRenderTextures)
                {
                    RenderTextureCamera renderTextureCamera = Camera.GetComponent<RenderTextureCamera>();
                    if (renderTextureCamera != null)
                    {
                        DestroyImmediate(renderTextureCamera);
                    }
                }

                RenderPipelineInfo.XRFix(Camera);
            }

            if (m_pointer == null)
            {
                m_pointer = gameObject.AddComponent<Pointer>();
            }

            if (m_camera != null)
            {
                SetCullingMask();
                if (WindowType == RuntimeWindowType.Scene)
                {
                    RegisterGraphicsCamera();
                }
            }
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
            if (m_camera != null)
            {
                ResetCullingMask();
                if (WindowType == RuntimeWindowType.Scene)
                {
                    UnregisterGraphicsCamera();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TryResize();
        }

        protected virtual void Update()
        {
            UpdateOverride();
        }

        protected override void UpdateOverride()
        {
            TryResize();
        }

        private void RegisterGraphicsCamera()
        {
            IRTEGraphics graphics = IOC.Resolve<IRTEGraphics>();
            if (graphics != null)
            {
                graphics.RegisterCamera(m_camera);
            }
        }

        private void UnregisterGraphicsCamera()
        {
            IRTEGraphics graphics = IOC.Resolve<IRTEGraphics>();
            if (graphics != null)
            {
                graphics.UnregisterCamera(m_camera);
            }
        }

        private void TryResize()
        {
            if (m_camera != null && m_rectTransform != null)
            {
                if (m_rectTransform.rect != m_rect || m_rectTransform.position != m_position)
                {
                    HandleResize();

                    m_rect = m_rectTransform.rect;
                    m_position = m_rectTransform.position;
                }
            }
        }

        public override void HandleResize()
        {
            if (m_camera == null)
            {
                return;
            }

            Canvas canvas = Canvas;
            if (m_rectTransform != null && canvas != null)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    Vector3[] corners = new Vector3[4];
                    m_rectTransform.GetWorldCorners(corners);
                    ResizeCamera(new Rect(corners[0], new Vector2(corners[2].x - corners[0].x, corners[1].y - corners[0].y)));
                }
                else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    if (canvas.worldCamera != Camera)
                    {
                        Vector3[] corners = new Vector3[4];
                        m_rectTransform.GetWorldCorners(corners);

                        corners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[0]);
                        corners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
                        corners[2] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[2]);
                        corners[3] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);

                        Vector2 size = new Vector2(corners[2].x - corners[0].x, corners[1].y - corners[0].y);
                        ResizeCamera(new Rect(corners[0], size));
                    }
                }
            }
        }

        protected virtual void ResizeCamera(Rect pixelRect)
        {
            m_camera.pixelRect = pixelRect;
            if (CameraResized != null)
            {
                CameraResized();
            }
        }

        protected virtual void SetCullingMask()
        {
            SetCullingMask(m_camera);
        }

        protected virtual void ResetCullingMask()
        {
            ResetCullingMask(m_camera);
        }

        protected virtual void SetCullingMask(Camera camera)
        {
            CameraLayerSettings settings = Editor.CameraLayerSettings;
            camera.cullingMask &= (settings.RaycastMask | 1 << settings.AllScenesLayer);
        }

        protected virtual void ResetCullingMask(Camera camera)
        {
            CameraLayerSettings settings = Editor.CameraLayerSettings;
            camera.cullingMask |= ~(settings.RaycastMask | 1 << settings.AllScenesLayer);
        }
    }
}
