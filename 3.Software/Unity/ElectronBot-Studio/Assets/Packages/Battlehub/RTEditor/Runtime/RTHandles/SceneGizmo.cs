using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.UI;
using Battlehub.Utils;
using Battlehub.RTCommon;
using TMPro;

namespace Battlehub.RTHandles
{
    [RequireComponent(typeof(Camera))]
    public class SceneGizmo : RTEComponent
    {
        public Button BtnProjection;
        public Transform Pivot;
        public Vector2 Size = new Vector2(96, 96);
        public Vector2 PivotPoint = new Vector2(1, 0);
        public Vector2 Anchor = new Vector2(1, 0);

        public Vector3 Up = Vector3.up;
        public RuntimeHandlesComponent Appearance;
        
        public UnityEvent OrientationChanging;
        public UnityEvent OrientationChanged;
        public UnityEvent ProjectionChanged;

        private float m_scale;
        private Rect m_cameraPixelRect;
        private float m_aspect;
        private Camera m_camera;
        
        private MaterialPropertyBlock[] m_propertyBlocks;
        private float m_xAlpha = 1.0f;
        private float m_yAlpha = 1.0f;
        private float m_zAlpha = 1.0f;
        private float m_animationDuration = 0.2f;

        private GUIStyle m_buttonStyle;
        private Rect m_buttonRect;

        private bool m_mouseOver;
        private Vector3 m_selectedAxis;
        private GameObject m_collidersGO;
        private BoxCollider m_colliderProj;
        private BoxCollider m_colliderUp;
        private BoxCollider m_colliderDown;
        private BoxCollider m_colliderForward;
        private BoxCollider m_colliderBackward;
        private BoxCollider m_colliderLeft;
        private BoxCollider m_colliderRight;
        private Collider[] m_colliders;

        private Vector3 m_position;
        private Quaternion m_rotation;
        private Vector3 m_gizmoPosition;
        private IAnimationInfo m_rotateAnimation;
        
        private float m_screenHeight;
        private float m_screenWidth;
        private bool m_projectionChanged;
        public bool IsOrthographic
        {
            get { return m_camera.orthographic; }
            set
            {
                m_projectionChanged = true;
                m_camera.orthographic = value;
                Window.Camera.orthographic = value;
              
                if (BtnProjection != null)
                {
                    Text txt = BtnProjection.GetComponentInChildren<Text>();
                    if (txt != null)
                    {
                        if (value)
                        {
                            txt.text = "Ortho";
                        }
                        else
                        {
                            txt.text = "Persp";
                        }
                    }
                    else
                    {
                        TextMeshProUGUI txtPro = BtnProjection.GetComponentInChildren<TextMeshProUGUI>();
                        if(txtPro != null)
                        {
                            if(value)
                            {
                                txtPro.text = "Ortho";
                            }
                            else
                            {
                                txtPro.text = "Persp";
                            }
                        }
                    }
                }

         
                if (ProjectionChanged != null)
                {
                    ProjectionChanged.Invoke();
                    InitColliders();
                }
            }
        }

        [SerializeField]
        private Color m_textColor = Color.white;

        public Color TextColor
        {
            get { return m_textColor; }
            set
            {
                m_textColor = value;
                SetTextColor();
            }
        }

        private void SetTextColor()
        {
            if (BtnProjection != null)
            {
                Text txt = BtnProjection.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.color = m_textColor;
                }

                TextMeshProUGUI txtPro = BtnProjection.GetComponentInChildren<TextMeshProUGUI>();
                if(txtPro != null)
                {
                    txtPro.color = m_textColor;
                }
            }

            DoSceneGizmo();
        }

        private Material m_material;
        private GameObject m_output;
        private RenderTexture m_renderTexture;
        private IRTECamera m_rteCamera;
        private RTECamera m_rteGizmoCamera;
        private IRenderPipelineCameraUtility m_cameraUtility;
        private bool m_disableCamera;

        protected override void Awake()
        {
            base.Awake();
        
            RuntimeHandlesComponent.InitializeIfRequired(ref Appearance);

            if (Pivot == null)
            {
                Pivot = transform;
            }

            m_collidersGO = new GameObject();
            m_collidersGO.transform.SetParent(transform, false);
            m_collidersGO.transform.position = GetGizmoPosition();
            m_collidersGO.transform.rotation = Quaternion.identity;
            m_collidersGO.name = "Colliders";

            m_colliderProj = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderUp = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderDown = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderLeft = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderRight = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderForward = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderBackward = m_collidersGO.AddComponent<BoxCollider>();

            m_colliders = new[] { m_colliderProj, m_colliderUp, m_colliderDown, m_colliderRight, m_colliderLeft, m_colliderForward, m_colliderBackward };
            DisableColliders();

            m_camera = GetComponent<Camera>();
            m_rteGizmoCamera = m_camera.gameObject.AddComponent<RTECamera>();
            m_rteGizmoCamera.Event = CameraEvent.BeforeImageEffects;
            m_rteGizmoCamera.CommandBufferRefresh += OnCommandBufferRefresh;

            m_propertyBlocks = new[] { new MaterialPropertyBlock(), new MaterialPropertyBlock(), new MaterialPropertyBlock() };
    
            m_cameraUtility = IOC.Resolve<IRenderPipelineCameraUtility>();
            if (m_cameraUtility != null)
            {
                m_cameraUtility.SetBackgroundColor(m_camera, new Color(0, 0, 0, 0));
                m_cameraUtility.EnablePostProcessing(m_camera, false);
                m_cameraUtility.PostProcessingEnabled += OnPostProcessingEnabled;
            }

            m_material = new Material(Shader.Find("Battlehub/RTHandles/RawImage"));
            
            m_output = new GameObject("SceneGizmoOutput");
            m_output.gameObject.SetActive(false);
            m_output.transform.SetParent(Window.Camera.transform, false);
            m_output.transform.localPosition = Vector3.forward * m_camera.nearClipPlane;
            m_output.AddComponent<MeshFilter>().sharedMesh = Appearance.CreateRawImageMesh();
            m_output.AddComponent<MeshRenderer>().sharedMaterial = m_material;
            
            m_camera.clearFlags = CameraClearFlags.SolidColor;
            m_camera.backgroundColor = new Color(0, 0, 0, 0);
            m_camera.cullingMask = 0;
            m_camera.orthographic = Window.Camera.orthographic;
            m_camera.rect = new Rect(0, 0, 1, 1);
            m_camera.stereoTargetEye = StereoTargetEyeMask.None;

            m_screenHeight = Screen.height;
            m_screenWidth = Screen.width;

            UpdateLayout();
            InitColliders();
            UpdateAlpha(ref m_xAlpha, Vector3.right, 1);
            UpdateAlpha(ref m_yAlpha, Vector3.up, 1);
            UpdateAlpha(ref m_zAlpha, Vector3.forward, 1);
            if (Run.Instance == null)
            {
                GameObject runGO = new GameObject();
                runGO.name = "Run";
                runGO.AddComponent<Run>();
            }

            if (BtnProjection != null)
            {
                BtnProjection.onClick.AddListener(OnBtnModeClick);
                SetTextColor();
            }

            if (!GetComponent<SceneGizmoInput>())
            {
                gameObject.AddComponent<SceneGizmoInput>();
            }
        }

        private void OnPostProcessingEnabled(Camera camera, bool enabled)
        {
            if(camera == Window.Camera)
            {
                UpdateLayout();
                DoSceneGizmo();
            }
        }

        protected override void Start()
        {
            if(IsOrthographic != Window.Camera.orthographic)
            {
                IsOrthographic = Window.Camera.orthographic;
            }

            Init();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
                
            if (BtnProjection != null)
            {
                BtnProjection.onClick.RemoveListener(OnBtnModeClick);
            }
            if (Editor != null && Editor.Tools != null && Editor.Tools.ActiveTool == this)
            {
                if(Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
            }

            if(m_rteGizmoCamera != null)
            {
                m_rteGizmoCamera.CommandBufferRefresh -= OnCommandBufferRefresh;
                Destroy(m_rteGizmoCamera);
            }

            if(m_material != null)
            {
                Destroy(m_material);
            }

            if(m_renderTexture != null)
            {
                Destroy(m_renderTexture);
            }

            if(m_output != null)
            {
                Destroy(m_output);
            }

            if (m_cameraUtility != null)
            {
                m_cameraUtility.PostProcessingEnabled -= OnPostProcessingEnabled;
            }
        }

        protected virtual void OnEnable()
        {
            if(IsStarted)
            {
                Init();
            }
        }

        private void Init()
        {
            Camera camera = Window.Camera;
            IRTEGraphicsLayer graphicsLayer = Window.IOCContainer.Resolve<IRTEGraphicsLayer>();
            if (graphicsLayer != null && graphicsLayer.Camera != null)
            {
                camera = graphicsLayer.Camera.Camera;
            }

            if (camera == null)
            {
                return;
            }

            IRTEGraphics graphics = IOC.Resolve<IRTEGraphics>();
            m_rteCamera = graphics.CreateCamera(camera, CameraEvent.AfterImageEffects, false, true);
            m_rteCamera.RenderersCache.Add(m_output.GetComponent<Renderer>());
            m_rteCamera.RefreshCommandBuffer();

            DoSceneGizmo();

            if (BtnProjection != null)
            {
                BtnProjection.gameObject.SetActive(true);
            }
        }

        protected virtual void OnDisable()
        {
            if(m_rteCamera != null)
            {
                m_rteCamera.Destroy();
                m_rteCamera = null;
            }
            
            if (BtnProjection != null)
            {
                BtnProjection.gameObject.SetActive(false);
            }
        }

        private void OnBtnModeClick()
        {
            IsOrthographic = !Window.Camera.orthographic;
        }

        protected override void OnWindowDeactivated()
        {
            base.OnWindowDeactivated();
            if(Editor != null && Editor.Tools != null && Editor.Tools.ActiveTool == this)
            {
                Editor.Tools.ActiveTool = null;
            }
        }

        private void Update()
        {
            bool changed = Sync();
            float delta = Time.deltaTime / m_animationDuration;
            bool updateAlpha = UpdateAlpha(ref m_xAlpha, Vector3.right, delta);
            updateAlpha |= UpdateAlpha(ref m_yAlpha, Vector3.up, delta);
            updateAlpha |= UpdateAlpha(ref m_zAlpha, Vector3.forward, delta);

            if (changed || updateAlpha || m_mouseOver || m_projectionChanged)
            {
                if (updateAlpha)
                {
                    DisableColliders();
                    EnableColliders();
                }

                m_projectionChanged = false;
                DoSceneGizmo();
            }
            else
            {
                if(!m_disableCamera)
                {
                    m_disableCamera = true;
                }
                else
                {
                    if(RenderPipelineInfo.Type != RPType.HDRP)
                    {
                        m_camera.enabled = false;
                        m_disableCamera = false;
                    }
                }
            }
            
            if (Editor.Tools.IsViewing)
            {
                m_selectedAxis = Vector3.zero;
                return;
            }

            if(Editor.Tools.ActiveTool != null && Editor.Tools.ActiveTool != this)
            {
                m_selectedAxis = Vector3.zero;
                return;
            }

            bool isMouseOverButton = false;
            bool pointerOverSceneGizmo = m_camera.pixelRect.Contains(ScreenPointToViewPoint(Window.Pointer.ScreenPoint));

            if (pointerOverSceneGizmo && Editor.ActiveWindow == Window && Window.IsPointerOver)
            {
                if (!m_mouseOver || updateAlpha)
                {
                    InitColliders();
                    EnableColliders();
                }

                Collider collider = HitTest();
                if (collider == null || m_rotateAnimation != null && m_rotateAnimation.InProgress)
                {
                    m_selectedAxis = Vector3.zero;
                }
                else if (collider == m_colliderProj)
                {
                    m_selectedAxis = Vector3.one;
                }
                else if (collider == m_colliderUp)
                {
                    m_selectedAxis = Vector3.up;
                }
                else if (collider == m_colliderDown)
                {
                    m_selectedAxis = Vector3.down;
                }
                else if (collider == m_colliderForward)
                {
                    m_selectedAxis = Vector3.forward;
                }
                else if (collider == m_colliderBackward)
                {
                    m_selectedAxis = Vector3.back;
                }
                else if (collider == m_colliderRight)
                {
                    m_selectedAxis = Vector3.right;
                }
                else if (collider == m_colliderLeft)
                {
                    m_selectedAxis = Vector3.left;
                }

                if (m_selectedAxis != Vector3.zero || isMouseOverButton)
                {
                    Editor.Tools.ActiveTool = this;
                }
                else
                {
                    if(Editor.Tools.ActiveTool == this)
                    {
                        Editor.Tools.ActiveTool = null;
                    }
                }

                m_mouseOver = true;
            }
            else
            {
                if (m_mouseOver)
                {
                    DisableColliders();

                    if(Editor.Tools.ActiveTool == this)
                    {
                        Editor.Tools.ActiveTool = null;
                    }   
                }
                m_selectedAxis = Vector3.zero;
                m_mouseOver = false;
            }
        }

        public void DoSceneGizmo()
        {
            if(m_camera != null)
            {
                m_camera.enabled = true;
                m_rteGizmoCamera.RefreshCommandBuffer();
            }    
        }

        private void OnCommandBufferRefresh(IRTECamera rteCamera)
        {
            Appearance.DoSceneGizmo(rteCamera.CommandBuffer, m_propertyBlocks, m_camera, GetGizmoPosition(), Quaternion.identity, m_selectedAxis, Appearance.SceneGizmoScale, m_textColor, m_xAlpha, m_yAlpha, m_zAlpha);
        }

        public void Click()
        {
            if (m_selectedAxis != Vector3.zero)
            {
                if (m_selectedAxis == Vector3.one)
                {
                    IsOrthographic = !IsOrthographic;
                    DoSceneGizmo();
                }
                else
                {
                    ChangeOrientation(-m_selectedAxis);
                }
            }
        }

        public void ChangeOrientation(Vector3 axis)
        {
            if (m_rotateAnimation == null || !m_rotateAnimation.InProgress)
            {
                if (OrientationChanging != null)
                {
                    OrientationChanging.Invoke();
                }
            }

            if (m_rotateAnimation != null)
            {
                m_rotateAnimation.Abort();
            }

            Vector3 pivot = Pivot.transform.position;
            Vector3 radiusVector = Vector3.back * (Window.Camera.transform.position - pivot).magnitude;
            Quaternion targetRotation = Quaternion.LookRotation(axis, Up);
            m_rotateAnimation = new QuaternionAnimationInfo(Window.Camera.transform.rotation, targetRotation, 0.4f, QuaternionAnimationInfo.EaseOutCubic,
                (target, value, t, completed) =>
                {
                    Window.Camera.transform.position = pivot + value * radiusVector;
                    Window.Camera.transform.rotation = value;

                    if (completed)
                    {
                        DisableColliders();
                        EnableColliders();

                        if (OrientationChanged != null)
                        {
                            OrientationChanged.Invoke();
                        }
                    }
                });

            Run.Instance.Animation(m_rotateAnimation);
        }

        private bool Sync()
        {
            bool changed = false;
         
            if (m_screenHeight != Screen.height || m_screenWidth != Screen.width || m_cameraPixelRect != Window.Camera.pixelRect || m_scale != Appearance.SceneGizmoScale)
            {
                UpdateLayout();
                changed = true;
            }

            if (m_aspect != m_camera.aspect)
            {
                m_aspect = m_camera.aspect;
                changed = true;
            }

            if(m_camera.depth != Window.Camera.depth + 1)
            {
                m_camera.depth = Window.Camera.depth + 1;
                changed = true;
            }

            Quaternion rotation = Window.Camera.transform.rotation;
            if(rotation != m_camera.transform.rotation)
            {
                m_camera.transform.rotation = rotation;
                changed = true;
            }

            return changed;
        }

        private void EnableColliders()
        {
            m_colliderProj.enabled = true;
            if (m_zAlpha == 1)
            {
                m_colliderForward.enabled = true;
                m_colliderBackward.enabled = true;
            }
            if (m_yAlpha == 1)
            {
                m_colliderUp.enabled = true;
                m_colliderDown.enabled = true;
            }
            if (m_xAlpha == 1)
            {
                m_colliderRight.enabled = true;
                m_colliderLeft.enabled = true;
            }
        }

        private void DisableColliders()
        {
            for (int i = 0; i < m_colliders.Length; ++i)
            {
                m_colliders[i].enabled = false;
            }
        }

        private Vector2 ScreenPointToViewPoint(Vector2 screenPoint)
        {
            Vector2 size = Size * Appearance.SceneGizmoScale;

            Rect pixelRect = Window.Camera.pixelRect;
            float offsetX = pixelRect.width * Anchor.x - size.x * PivotPoint.x;
            float offsetY = pixelRect.height - (pixelRect.height * Anchor.y + (size.y - size.y * PivotPoint.y));

            screenPoint.x -= offsetX + pixelRect.x;
            screenPoint.y -= offsetY + pixelRect.y;

            return screenPoint;     
        }

        private Collider HitTest()
        {
            Ray ray = m_camera.ScreenPointToRay(ScreenPointToViewPoint(Window.Pointer.ScreenPoint));

            float minDistance = float.MaxValue;
            Collider result = null;
            for(int i = 0; i < m_colliders.Length; ++i)
            {
                Collider collider = m_colliders[i];
                RaycastHit hitInfo;
                if (collider.Raycast(ray, out hitInfo, m_gizmoPosition.magnitude * 5))
                {
                    if(hitInfo.distance < minDistance)
                    {
                        minDistance = hitInfo.distance;
                        result = hitInfo.collider;
                    }
                }
            }

            return result;
        }

        private Vector3 GetGizmoPosition()
        {
            return transform.TransformPoint(Vector3.forward * 5);
        }
        
        private void InitColliders()
        {
            m_gizmoPosition = GetGizmoPosition();
            float sScale = RuntimeHandlesComponent.GetScreenScale(m_gizmoPosition, m_camera) * Appearance.SceneGizmoScale;

            m_collidersGO.transform.rotation = Quaternion.identity;
            m_collidersGO.transform.position = GetGizmoPosition();

            const float size = 0.15f;
            m_colliderProj.size = new Vector3(size * 1.5f, size * 1.5f, size * 1.5f) * sScale;

            m_colliderUp.size = new Vector3(size, size * 2, size) * sScale;
            m_colliderUp.center = new Vector3(0.0f, size + size / 2, 0.0f) * sScale;

            m_colliderDown.size = new Vector3(size, size * 2, size) * sScale;
            m_colliderDown.center = new Vector3(0.0f, -(size + size / 2), 0.0f) * sScale;

            m_colliderForward.size = new Vector3(size, size, size * 2) * sScale;
            m_colliderForward.center = new Vector3(0.0f,  0.0f, size + size / 2) * sScale;

            m_colliderBackward.size = new Vector3(size, size, size * 2) * sScale;
            m_colliderBackward.center = new Vector3(0.0f, 0.0f, -(size + size / 2)) * sScale;

            m_colliderRight.size = new Vector3(size * 2, size, size) * sScale;
            m_colliderRight.center = new Vector3(size + size / 2, 0.0f, 0.0f) * sScale;

            m_colliderLeft.size = new Vector3(size * 2, size, size) * sScale;
            m_colliderLeft.center = new Vector3(-(size + size / 2), 0.0f, 0.0f) * sScale;
        }

        private bool UpdateAlpha(ref float alpha, Vector3 axis, float delta)
        {
            bool hide = Math.Abs(Vector3.Dot(Window.Camera.transform.forward, axis)) > 0.9;
            if (hide)
            {
                if (alpha > 0.0f)
                {
                    
                    alpha -= delta;
                    if (alpha < 0.0f)
                    {
                        alpha = 0.0f;
                    }
                    return true;
                }
            }
            else
            {
                if (alpha < 1.0f)
                {
                    alpha += delta;
                    if (alpha > 1.0f)
                    {
                        alpha = 1.0f;
                    }
                    return true;
                }
            }

            return false;
        }

        public void UpdateLayout()
        {
            m_screenHeight = Screen.height;
            m_screenWidth = Screen.width;
            m_cameraPixelRect = Window.Camera.pixelRect;
            m_scale = Appearance.SceneGizmoScale;

            if (m_camera == null)
            {
                return;
            }

            m_aspect = m_camera.aspect;

            if (Window.Camera != null)
            {
                bool initColliders = false;

                if (m_camera.pixelRect.height == 0 || m_camera.pixelRect.width == 0)
                {
                    return;
                }
                else
                {
                    if (!enabled)
                    {
                        initColliders = true;
                    }
                }
                m_camera.depth = Window.Camera.depth + 1;
                m_aspect = m_camera.aspect;

                if (initColliders)
                {
                    InitColliders();
                }
            }

            Vector2 pivotPoint = PivotPoint;
            Vector2 anchor = Anchor;
            if (RenderPipelineInfo.Type == RPType.Standard || m_cameraUtility != null && m_cameraUtility.IsPostProcessingEnabled(Window.Camera))
            {
                pivotPoint.y = 1 - pivotPoint.y;
                anchor.y = 1 - anchor.y;
            }

            if (m_renderTexture != null)
            {
                Destroy(m_renderTexture);
            }

            Vector2 size = Size * Appearance.SceneGizmoScale;

            RenderTexture oldTexture = m_renderTexture;

            m_renderTexture = new RenderTexture((int)size.x, (int)size.y, 24, RenderTextureFormat.ARGB32);
            m_renderTexture.filterMode = FilterMode.Point;
            m_renderTexture.antiAliasing = Mathf.Max(1, RenderPipelineInfo.MSAASampleCount);

            m_material.SetTexture("_MainTex", m_renderTexture);
            m_material.SetFloat("_Width", size.x);
            m_material.SetFloat("_Height", size.y);
            m_material.SetVector("_PivotAndAnchor", new Vector4(pivotPoint.x, pivotPoint.y, anchor.x, anchor.y));

            m_camera.targetTexture = m_renderTexture;

            if (oldTexture != null)
            {
                oldTexture.Release();
                Destroy(oldTexture);
            }
        }
    }
}

