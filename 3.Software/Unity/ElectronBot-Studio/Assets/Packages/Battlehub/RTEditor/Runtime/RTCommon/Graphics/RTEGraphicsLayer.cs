using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
    public interface IRTEGraphicsLayer
    {
        IRTECamera Camera
        {
            get;
        }
    }

    [DefaultExecutionOrder(-55)]
    [RequireComponent(typeof(RuntimeWindow))]
    public class RTEGraphicsLayer : MonoBehaviour, IRTEGraphicsLayer
    {
        private Camera m_camera;
        private RenderTextureCamera m_renderTextureCamera;
        private IRTEGraphics m_graphics;
        private IRTECamera m_graphicsCamera;
        public IRTECamera Camera
        {
            get { return m_graphicsCamera; }
        }

        private RuntimeCameraWindow m_window;
        public RuntimeCameraWindow Window
        {
            get { return m_window; }
        }

        [SerializeField]
        private RectTransform m_output = null;

        private void Awake()
        {
            m_window = GetComponent<RuntimeCameraWindow>();
            m_window.IOCContainer.RegisterFallback<IRTEGraphicsLayer>(this);
            m_window.CameraResized += OnCameraResized;
            m_graphics = IOC.Resolve<IRTEGraphics>();

            if (m_window.Index >= m_window.Editor.CameraLayerSettings.MaxGraphicsLayers)
            {
                Debug.LogError("m_editorWindow.Index >= m_editorWindow.Editor.CameraLayerSettings.MaxGraphicsLayers");
            }

            PrepareGraphicsLayerCamera();
        }

        private void OnDestroy()
        {
            if (m_window != null)
            {
                m_window.IOCContainer.UnregisterFallback<IRTEGraphicsLayer>(this);
                m_window.CameraResized -= OnCameraResized;
            }

            if(m_graphicsCamera != null)
            {
                m_graphicsCamera.Destroy();
            }

            if (m_camera != null)
            {
                Destroy(m_camera.gameObject);
            }

            if (m_renderTextureCamera != null && m_renderTextureCamera.OverlayMaterial != null)
            {
                Destroy(m_renderTextureCamera.OverlayMaterial);
            }
        }

        //private void Start()
        //{
        //    if (m_window.Index >= m_window.Editor.CameraLayerSettings.MaxGraphicsLayers)
        //    {
        //        Debug.LogError("m_editorWindow.Index >= m_editorWindow.Editor.CameraLayerSettings.MaxGraphicsLayers");
        //    }

        //    PrepareGraphicsLayerCamera();
        //}

        private void OnEnable()
        {
            UpdateGraphicsLayerCamera();
        }

        private void LateUpdate()
        {
            UpdateGraphicsLayerCamera();
        }

        private void OnCameraResized()
        {
            UpdateGraphicsLayerCamera();            
        }

        private void PrepareGraphicsLayerCamera()
        {
            bool wasActive = m_window.Camera.gameObject.activeSelf;
            m_window.Camera.gameObject.SetActive(false);

            if (m_window.Editor.IsVR && m_window.Camera.stereoEnabled && m_window.Camera.stereoTargetEye == StereoTargetEyeMask.Both )
            {
                m_camera = Instantiate(m_window.Camera, m_window.Camera.transform.parent);
                m_camera.transform.SetSiblingIndex(m_window.Camera.transform.GetSiblingIndex() + 1);
            }
            else
            {
                m_camera = Instantiate(m_window.Camera, m_window.Camera.transform);
            }

            for (int i = m_camera.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(m_camera.transform.GetChild(i).gameObject);
            }

            Component[] components = m_camera.GetComponents<Component>();
            for (int i = 0; i < components.Length; ++i)
            {
                Component component = components[i];
                if (component is Transform)
                {
                    continue;
                }
                if (component is Camera)
                {
                    continue;
                }
                if(component is RenderTextureCamera)
                {
                    continue;
                }

                Destroy(component);
            }

            m_camera.transform.localPosition = Vector3.zero;
            m_camera.transform.localRotation = Quaternion.identity;
            m_camera.transform.localScale = Vector3.one;
            m_camera.name = "GraphicsLayerCamera";
            m_camera.depth = m_window.Camera.depth + 1;
            m_camera.cullingMask = 0;

            if (RenderPipelineInfo.Type == RPType.Standard)
            {
                m_graphicsCamera = m_graphics.CreateCamera(m_camera, CameraEvent.BeforeImageEffects, true, true);
            }
            else
            {
                m_graphicsCamera = m_graphics.CreateCamera(m_camera, CameraEvent.AfterImageEffectsOpaque, true, true);
            }
            
            m_renderTextureCamera = m_camera.GetComponent<RenderTextureCamera>();
            if (m_renderTextureCamera == null)
            {
                if (RenderPipelineInfo.Type == RPType.Standard)
                {
                    if (m_window.RenderTextureUsage == RenderTextureUsage.On || m_window.RenderTextureUsage == RenderTextureUsage.UsePipelineSettings && RenderPipelineInfo.UseRenderTextures)
                    {
                        CreateRenderTextureCamera();
                    }
                    else
                    {
                        m_camera.clearFlags = CameraClearFlags.Depth;
                    }
                }
                else
                {
                    RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
                    RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
                }
            }
            else
            {
                if (m_window.RenderTextureUsage == RenderTextureUsage.Off || m_window.RenderTextureUsage == RenderTextureUsage.UsePipelineSettings && !RenderPipelineInfo.UseRenderTextures)
                {
                    DestroyImmediate(m_renderTextureCamera);
                }
                else
                {
                    m_renderTextureCamera.OverlayMaterial = new Material(Shader.Find("Battlehub/RTCommon/RenderTextureOverlay"));
                    m_camera.clearFlags = CameraClearFlags.SolidColor;
                    m_camera.backgroundColor = new Color(0, 0, 0, 0);
                }
            }

            m_camera.allowHDR = false; //fix strange screen blinking bug...
            m_camera.projectionMatrix = m_window.Camera.projectionMatrix; //for ARCore

            m_window.Camera.gameObject.SetActive(wasActive);
            m_camera.gameObject.SetActive(true);
        }

        private void OnBeginFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
            if (m_window.RenderTextureUsage == RenderTextureUsage.Off || m_window.RenderTextureUsage == RenderTextureUsage.UsePipelineSettings && !RenderPipelineInfo.UseRenderTextures)
            {
                //Stack camera
                IRenderPipelineCameraUtility cameraUtility = IOC.Resolve<IRenderPipelineCameraUtility>();
                if (cameraUtility != null)
                {
                    cameraUtility.Stack(Window.Camera, m_camera);
                }
            }
        }

        private void OnEndFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
            if (m_window.RenderTextureUsage == RenderTextureUsage.On || m_window.RenderTextureUsage == RenderTextureUsage.UsePipelineSettings && RenderPipelineInfo.UseRenderTextures)
            {
                CreateRenderTextureCamera();
            }
        }

        private void CreateRenderTextureCamera()
        {
            bool wasActive = m_camera.gameObject.activeSelf;
            m_camera.gameObject.SetActive(false);
            m_renderTextureCamera = m_camera.gameObject.AddComponent<RenderTextureCamera>();

            if (m_output != null)
            {
                m_renderTextureCamera.OutputRoot = m_output;
            }
            else
            {
                IRTE rte = IOC.Resolve<IRTE>();
                RuntimeWindow sceneWindow = rte.GetWindow(RuntimeWindowType.Scene);
                m_renderTextureCamera.OutputRoot = (RectTransform)sceneWindow.transform;
            }
            m_renderTextureCamera.OverlayMaterial = new Material(Shader.Find("Battlehub/RTCommon/RenderTextureOverlay"));
            m_camera.clearFlags = CameraClearFlags.SolidColor;
            m_camera.backgroundColor = new Color(0, 0, 0, 0);
            m_camera.gameObject.SetActive(wasActive);
        }

        private void UpdateGraphicsLayerCamera()
        {
            if(m_camera == null)
            {
                return;
            }

            if (m_renderTextureCamera != null)
            {
                m_renderTextureCamera.TryResizeRenderTexture();
            }

            if (m_camera.depth != m_window.Camera.depth + 1)
            {
                m_camera.depth = m_window.Camera.depth + 1;
            }

            if (m_camera.fieldOfView != m_window.Camera.fieldOfView)
            {
                m_camera.fieldOfView = m_window.Camera.fieldOfView;
            }

            if (m_camera.orthographic != m_window.Camera.orthographic)
            {
                m_camera.orthographic = m_window.Camera.orthographic;
            }

            if (m_camera.orthographicSize != m_window.Camera.orthographicSize)
            {
                m_camera.orthographicSize = m_window.Camera.orthographicSize;
            }

            if (m_camera.rect != m_window.Camera.rect)
            {
                m_camera.rect = m_window.Camera.rect;
            }

            if (m_camera.enabled != m_window.Camera.enabled)
            {
                m_camera.enabled = m_window.Camera.enabled;
            }

            if (m_window.Camera.pixelWidth > 0 && m_window.Camera.pixelHeight > 0)
            {
                m_camera.projectionMatrix = m_window.Camera.projectionMatrix; //ARCore
            }
        }
    }
}



