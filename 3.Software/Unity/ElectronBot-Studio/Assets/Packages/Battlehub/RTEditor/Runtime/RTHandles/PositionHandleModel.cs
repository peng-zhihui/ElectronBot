using Battlehub.RTCommon;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class PositionHandleModel : BaseHandleModel
    {
        [SerializeField]
        private GameObject[] m_models = null;
        private Renderer[] m_renderers;
        private Renderer m_ssQuadRenderer;

        [SerializeField]
        private GameObject m_screenSpaceQuad = null;
        [SerializeField]
        private GameObject m_normalModeArrows = null;
        [SerializeField]
        private GameObject m_vertexSnappingModeArrows = null;      
        [SerializeField]
        private Transform[] m_armatures = null;
        [SerializeField]
        private Transform m_ssQuadArmature = null;

        [SerializeField]
        private int m_xMatIndex = 0;
        [SerializeField]
        private int m_yMatIndex = 1;
        [SerializeField]
        private int m_zMatIndex = 2;
        [SerializeField]
        private int m_xArrowMatIndex = 3;
        [SerializeField]
        private int m_yArrowMatIndex = 4;
        [SerializeField]
        private int m_zArrowMatIndex = 5;
        [SerializeField]
        private int m_xQMatIndex = 6;
        [SerializeField]
        private int m_yQMatIndex = 7;
        [SerializeField]
        private int m_zQMatIndex = 8;
        [SerializeField]
        private int m_xQuadMatIndex = 9;
        [SerializeField]
        private int m_yQuadMatIndex = 10;
        [SerializeField]
        private int m_zQuadMatIndex = 11;
 
        [SerializeField]
        private float m_quadTransparency = 0.5f;

        [SerializeField]
        private float m_radius = DefaultRadius;
        [SerializeField]
        private float m_length = DefaultLength;
        [SerializeField]
        private float m_arrowRadius = DefaultArrowRadius;
        [SerializeField]
        private float m_arrowLength = DefaultArrowLength;
        [SerializeField]
        private float m_quadLength = DefaultQuadLength;
        private float QuadLength
        {
            get { return Appearance.PositionHandleArrowOnly ? 0 : m_quadLength;}
        }
        [SerializeField]
        private bool m_isVertexSnapping;

        private readonly bool m_useColliders = true;

        private Material[] m_materials;
        private Material m_ssQuadMaterial;

        private Transform[] m_b0;
        private Transform[] m_b1x;
        private Transform[] m_b2x;
        private Transform[] m_b3x;
        private Transform[] m_bSx;        
        private Transform[] m_b1y;
        private Transform[] m_b2y;
        private Transform[] m_b3y;        
        private Transform[] m_bSy;
        private Transform[] m_b1z;
        private Transform[] m_b2z;
        private Transform[] m_b3z;
        private Transform[] m_bSz;

        private Transform m_b1ss;
        private Transform m_b2ss;
        private Transform m_b3ss;
        private Transform m_b4ss;

        private Vector3[] m_defaultArmaturesScale;
        private Vector3[] m_defaultB3XScale;
        private Vector3[] m_defaultB3YScale;
        private Vector3[] m_defaultB3ZScale;
        private Vector3[] m_defaultSigns = new[]
        {
            new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1,-1, 1), new Vector3(1,-1, 1),
            new Vector3(1, 1,-1), new Vector3(-1, 1,-1), new Vector3(-1,-1,-1), new Vector3(1,-1,-1),
            new Vector3(1, 1, 1)
        };

        private const float DefaultRadius = 0.05f;
        private const float DefaultLength = 1.0f;
        private const float DefaultArrowRadius = 0.1f;
        private const float DefaultArrowLength = 0.2f;
        private const float DefaultQuadLength = 0.2f;

        private BoxCollider m_xCollider;
        private BoxCollider m_yCollider;
        private BoxCollider m_zCollider;
        private BoxCollider m_xyCollider;
        private BoxCollider m_xzCollider;
        private BoxCollider m_yzCollider;
        private SphereCollider m_snappingCollider;
        private Collider[] m_colliders;

        public bool IsVertexSnapping
        {
            get { return m_isVertexSnapping; }
            set
            {
                if(m_isVertexSnapping == value)
                {
                    return;
                }
                m_isVertexSnapping = value;
                OnVertexSnappingModeChaged();
                SetColors();
                UpdateColliders();
            }
        }

        protected override void Awake()
        {
            base.Awake();
        
            m_defaultArmaturesScale = new Vector3[m_armatures.Length];
            m_defaultB3XScale = new Vector3[m_armatures.Length];
            m_defaultB3YScale = new Vector3[m_armatures.Length];
            m_defaultB3ZScale = new Vector3[m_armatures.Length];

            m_b1x = new Transform[m_armatures.Length];
            m_b1y = new Transform[m_armatures.Length];
            m_b1z = new Transform[m_armatures.Length];
            m_b2x = new Transform[m_armatures.Length];
            m_b2y = new Transform[m_armatures.Length];
            m_b2z = new Transform[m_armatures.Length];
            m_b3x = new Transform[m_armatures.Length];
            m_b3y = new Transform[m_armatures.Length];
            m_b3z = new Transform[m_armatures.Length];
            m_b0 = new Transform[m_armatures.Length];
            m_bSx = new Transform[m_armatures.Length];
            m_bSy = new Transform[m_armatures.Length];
            m_bSz = new Transform[m_armatures.Length];
            for (int i = 0; i < m_armatures.Length; ++i)
            {
                m_b1x[i] = m_armatures[i].GetChild(0);
                m_b1y[i] = m_armatures[i].GetChild(1);
                m_b1z[i] = m_armatures[i].GetChild(2);
                m_b2x[i] = m_armatures[i].GetChild(3);
                m_b2y[i] = m_armatures[i].GetChild(4);
                m_b2z[i] = m_armatures[i].GetChild(5);
                m_b3x[i] = m_armatures[i].GetChild(6);
                m_b3y[i] = m_armatures[i].GetChild(7);
                m_b3z[i] = m_armatures[i].GetChild(8);
                m_b0[i] = m_armatures[i].GetChild(9);
                m_bSx[i] = m_armatures[i].GetChild(10);
                m_bSy[i] = m_armatures[i].GetChild(11);
                m_bSz[i] = m_armatures[i].GetChild(12);

                m_defaultArmaturesScale[i] = m_armatures[i].localScale;
                m_defaultB3XScale[i] = transform.TransformVector(m_b3x[i].localScale);
                m_defaultB3YScale[i] = transform.TransformVector(m_b3y[i].localScale);
                m_defaultB3ZScale[i] = transform.TransformVector(m_b3z[i].localScale);
            }

            m_b1ss = m_ssQuadArmature.GetChild(1);
            m_b2ss = m_ssQuadArmature.GetChild(2);
            m_b3ss = m_ssQuadArmature.GetChild(3);
            m_b4ss = m_ssQuadArmature.GetChild(4);

            m_materials = m_models[0].GetComponent<Renderer>().materials;
            m_ssQuadRenderer = m_screenSpaceQuad.GetComponent<Renderer>();
            m_ssQuadRenderer.forceRenderingOff = true;
            m_ssQuadMaterial = m_ssQuadRenderer.sharedMaterial;
            SetDefaultColors();

            m_renderers = new Renderer[m_models.Length];
            for (int i = 0; i < m_models.Length; ++i)
            {
                Renderer renderer = m_models[i].GetComponent<Renderer>();
                renderer.sharedMaterials = m_materials;
                renderer.forceRenderingOff = true;
                m_renderers[i] = renderer;
            }

            OnVertexSnappingModeChaged();
            if(m_useColliders)
            {
                GameObject colliders = new GameObject("Colliders");
                colliders.transform.SetParent(transform, false);
                //colliders.transform.localScale = Vector3.one;

                colliders.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;

                m_xCollider = colliders.AddComponent<BoxCollider>();
                m_yCollider = colliders.AddComponent<BoxCollider>();
                m_zCollider = colliders.AddComponent<BoxCollider>();
                m_xzCollider = colliders.AddComponent<BoxCollider>();
                m_xyCollider = colliders.AddComponent<BoxCollider>();
                m_yzCollider = colliders.AddComponent<BoxCollider>();
                m_snappingCollider = colliders.AddComponent<SphereCollider>();

                m_colliders = new Collider[] { m_xCollider, m_yCollider, m_zCollider, m_xzCollider, m_xyCollider, m_yzCollider };

                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    m_colliders[i].isTrigger = true;
                }

                m_snappingCollider.isTrigger = true;

                m_xCollider.transform.gameObject.SetActive(false);
            }
        }

        protected override void Start()
        {
            base.Start();
            SetColors();
            UpdateColliders();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(IsWindowActive)
            {
                m_prevRotation = transform.rotation;
                m_prevPosition = transform.position;
                m_prevCameraPosition = Window.Camera.transform.position;
                int index = SetCameraPosition(m_prevCameraPosition);
                if(index >= 0)
                {
                    UpdateColliders(index);
                }   
            }
        }

        protected override void OnWindowActivating()
        {
            base.OnWindowActivating();
            m_prevRotation = transform.rotation;
            m_prevPosition = transform.position;
            m_prevCameraPosition = Window.Camera.transform.position;
            int index = SetCameraPosition(m_prevCameraPosition);
            if (index >= 0)
            {
                UpdateColliders(index);
            }

            if(m_useColliders)
            {
                m_xCollider.transform.gameObject.SetActive(true);
            }   
        }

        protected override void OnWindowDeactivating()
        {
            base.OnWindowDeactivating();

            if (m_xCollider != null)
            {
                m_xCollider.transform.gameObject.SetActive(false);
            }
        }

        public override void SetLock(LockObject lockObj)
        {
            base.SetLock(lockObj);
            OnVertexSnappingModeChaged();
            SetColors();
            UpdateColliders();
        }

        public override void Select(RuntimeHandleAxis axis)
        {
            base.Select(axis);
            OnVertexSnappingModeChaged();
            SetColors();
            UpdateColliders();
        }

        private void OnVertexSnappingModeChaged()
        {
            m_normalModeArrows.SetActive(!m_isVertexSnapping);
            m_vertexSnappingModeArrows.SetActive(m_isVertexSnapping && !m_lockObj.IsPositionLocked);

            if(m_vertexSnappingModeArrows.activeSelf)
            {
                for (int i = 0; i < m_renderers.Length; ++i)
                {
                    m_renderers[i].forceRenderingOff = true;
                }
                m_renderers[m_renderers.Length - 1].forceRenderingOff = false;
            }
            else
            {
                if(m_xCollider != null)
                {
                    m_prevCameraPosition = Window.Camera.transform.position;
                    int index = SetCameraPosition(m_prevCameraPosition, true);
                    if (index >= 0)
                    {
                        UpdateColliders(index);
                        m_prevIndex = index;
                    }
                }
            }

            m_ssQuadRenderer.forceRenderingOff = !m_vertexSnappingModeArrows.activeSelf;

            PushUpdatesToGraphicLayer();
        }

        private void SetDefaultColors()
        {
            if (m_lockObj.PositionX)
            {
                m_materials[m_xMatIndex].color = Colors.DisabledColor;
                m_materials[m_xArrowMatIndex].color = Colors.DisabledColor;

                if(Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_xMatIndex].SetFloat("_ZWrite", 0);
                    m_materials[m_xArrowMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_xMatIndex].color = Colors.XColor;
                m_materials[m_xArrowMatIndex].color = Colors.XColor;
                m_materials[m_xMatIndex].SetFloat("_ZWrite", 1);
                m_materials[m_xArrowMatIndex].SetFloat("_ZWrite", 1);
            }

            if (m_lockObj.PositionY)
            {
                m_materials[m_yMatIndex].color = Colors.DisabledColor;
                m_materials[m_yArrowMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_yMatIndex].SetFloat("_ZWrite", 0);
                    m_materials[m_yArrowMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_yMatIndex].color = Colors.YColor;
                m_materials[m_yArrowMatIndex].color = Colors.YColor;
                m_materials[m_yMatIndex].SetFloat("_ZWrite", 1);
                m_materials[m_yArrowMatIndex].SetFloat("_ZWrite", 1);
            }

            if (m_lockObj.PositionZ)
            {
                m_materials[m_zMatIndex].color = Colors.DisabledColor;
                m_materials[m_zArrowMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_zMatIndex].SetFloat("_ZWrite", 0);
                    m_materials[m_zArrowMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_zMatIndex].color = Colors.ZColor;
                m_materials[m_zArrowMatIndex].color = Colors.ZColor;
                m_materials[m_zMatIndex].SetFloat("_ZWrite", 1);
                m_materials[m_zArrowMatIndex].SetFloat("_ZWrite", 1);
            }

            if (m_lockObj.PositionY || m_lockObj.PositionZ)
            {
                m_materials[m_xQMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_xQMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_xQMatIndex].color = Colors.XColor;
                m_materials[m_xQMatIndex].SetFloat("_ZWrite", 1);
            }
            
            if(m_lockObj.PositionX || m_lockObj.PositionZ)
            {
                m_materials[m_yQMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_yQMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_yQMatIndex].color = Colors.YColor;
                m_materials[m_yQMatIndex].SetFloat("_ZWrite", 1);
            }
            
            if (m_lockObj.PositionX || m_lockObj.PositionY)
            {
                m_materials[m_zQMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_zQMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_zQMatIndex].color = Colors.ZColor;
                m_materials[m_zQMatIndex].SetFloat("_ZWrite", 1);
            }
                
            Color xQuadColor = m_lockObj.PositionY || m_lockObj.PositionZ ? Colors.DisabledColor : Colors.XColor; xQuadColor.a = Mathf.Min(m_quadTransparency, xQuadColor.a);
            m_materials[m_xQuadMatIndex].color =  xQuadColor;

            Color yQuadColor = m_lockObj.PositionX || m_lockObj.PositionZ ? Colors.DisabledColor : Colors.YColor; yQuadColor.a = Mathf.Min(m_quadTransparency, yQuadColor.a);
            m_materials[m_yQuadMatIndex].color = yQuadColor;

            Color zQuadColor = m_lockObj.PositionX || m_lockObj.PositionY ? Colors.DisabledColor : Colors.ZColor; zQuadColor.a = Mathf.Min(m_quadTransparency, zQuadColor.a);
            m_materials[m_zQuadMatIndex].color = zQuadColor;

            m_ssQuadMaterial.color = Colors.AltColor;
        }

        private void UpdateColliders()
        {
            m_xCollider.enabled = !m_lockObj.PositionX;
            m_yCollider.enabled = !m_lockObj.PositionY;
            m_zCollider.enabled = !m_lockObj.PositionZ;

            m_xyCollider.enabled = !m_lockObj.PositionX && !m_lockObj.PositionY;
            m_xzCollider.enabled = !m_lockObj.PositionX && !m_lockObj.PositionZ;
            m_yzCollider.enabled = !m_lockObj.PositionY && !m_lockObj.PositionZ;
        }

        private void SetColors()
        {
            SetDefaultColors();
            switch (m_selectedAxis)
            {
                case RuntimeHandleAxis.XY:
                    if (!m_lockObj.PositionX && !m_lockObj.PositionY)
                    {
                        m_materials[m_xArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_xMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zQMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zQuadMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.YZ:
                    if (!m_lockObj.PositionY && !m_lockObj.PositionZ)
                    {
                        m_materials[m_yArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zMatIndex].color = Colors.SelectionColor;
                        m_materials[m_xQMatIndex].color = Colors.SelectionColor;
                        m_materials[m_xQuadMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.XZ:
                    if (!m_lockObj.PositionX && !m_lockObj.PositionZ)
                    {
                        m_materials[m_xArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_xMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yQMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yQuadMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.X:
                    if (!m_lockObj.PositionX)
                    {
                        m_materials[m_xArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_xMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Y:
                    if (!m_lockObj.PositionY)
                    {
                        m_materials[m_yArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Z:
                    if (!m_lockObj.PositionZ)
                    {
                        m_materials[m_zArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Snap:
                    m_ssQuadMaterial.color = Colors.SelectionColor;
                    break;
                case RuntimeHandleAxis.Screen:
                    break;
            }
        }

        public override void UpdateModel()
        {
            float quadLength = Mathf.Abs(QuadLength);
            m_radius = Mathf.Max(0.01f, m_radius);

            Vector3 right = transform.rotation * Vector3.right * transform.localScale.x;
            Vector3 up = transform.rotation * Vector3.up * transform.localScale.y;
            Vector3 forward = transform.rotation * Vector3.forward * transform.localScale.z;
            Vector3 p = transform.position;

            float radius = m_radius * ModelScale;
            float length = m_length * ModelScale;
            float arrowRadius = m_arrowRadius * ModelScale;
            float arrowLength = m_arrowLength * ModelScale;

            quadLength = quadLength * ModelScale;

            float scale = radius / DefaultRadius;
            float arrowScale = arrowLength / DefaultArrowLength / scale;
            float arrowRadiusScale = arrowRadius / DefaultArrowRadius / scale;

            for (int i = 0; i < m_models.Length; ++i)
            {
                m_armatures[i].localScale = m_defaultArmaturesScale[i] * scale;
                m_ssQuadArmature.localScale = Vector3.one * scale;

                m_b3x[i].position = transform.TransformPoint(Vector3.right * length);
                m_b3y[i].position = transform.TransformPoint(Vector3.up * length);
                m_b3z[i].position = transform.TransformPoint(Vector3.forward * length);

                m_b2x[i].position = transform.TransformPoint(Vector3.right * (length - arrowLength));
                m_b2y[i].position = transform.TransformPoint(Vector3.up * (length - arrowLength));
                m_b2z[i].position = transform.TransformPoint(Vector3.forward * (length - arrowLength));

                m_b3x[i].localScale = Vector3.right * arrowScale +
                    new Vector3(0, 1, 1) * arrowRadiusScale;
                m_b3y[i].localScale = Vector3.forward * arrowScale +
                    new Vector3(1, 1, 0) * arrowRadiusScale;
                m_b3z[i].localScale = Vector3.up * arrowScale +
                    new Vector3(1, 0, 1) * arrowRadiusScale;

                m_b1x[i].position = transform.TransformPoint(m_defaultSigns[i].x * Vector3.right * quadLength);
                m_b1y[i].position = transform.TransformPoint(m_defaultSigns[i].y * Vector3.up * quadLength);
                m_b1z[i].position = transform.TransformPoint(m_defaultSigns[i].z * Vector3.forward * quadLength);

                m_bSx[i].position = p + (m_b1y[i].position - p) + (m_b1z[i].position - p);
                m_bSy[i].position = p + (m_b1x[i].position - p) + (m_b1z[i].position - p);
                m_bSz[i].position = p + (m_b1x[i].position - p) + (m_b1y[i].position - p);
            }

            m_b1ss.position = p + transform.rotation * new Vector3(1, 1, 0) * quadLength * transform.localScale.x * 0.5f;
            m_b2ss.position = p + transform.rotation * new Vector3(-1, -1, 0) * quadLength * transform.localScale.x * 0.5f;
            m_b3ss.position = p + transform.rotation * new Vector3(-1, 1, 0) * quadLength * transform.localScale.x * 0.5f;
            m_b4ss.position = p + transform.rotation * new Vector3(1, -1, 0) * quadLength * transform.localScale.x * 0.5f;

            int index = SetCameraPosition(Window.Camera.transform.position);
            if (index >= 0)
            {
                UpdateColliders(index);
            }

            base.UpdateModel();
        }

        private void UpdateColliders(int i)
        {
            if (!m_useColliders)
            {
                return;
            }

            float size = 2 * m_arrowRadius * SelectionMargin;
            float size2 = 2 * m_radius;

            Transform root = m_xCollider.transform.parent;

            Vector3 b3x, b1x;
            b3x = m_b3x[i].position;
            b3x = root.InverseTransformPoint(b3x);
            b1x = m_b1x[i].position;
            b1x = root.InverseTransformPoint(b1x);
            m_xCollider.size = new Vector3(b3x.x - Mathf.Clamp(b1x.x, 0, b1x.x), size, size);
            m_xCollider.center = new Vector3(Mathf.Clamp(b1x.x, 0, b1x.x) + m_xCollider.size.x / 2, b3x.y, b3x.z);

            Vector3 b3y, b1y;
            b3y = m_b3y[i].position;
            b3y = root.InverseTransformPoint(b3y);
            b1y = m_b1y[i].position;
            b1y = root.InverseTransformPoint(b1y);
            m_yCollider.size = new Vector3(size, b3y.y - Mathf.Clamp(b1y.y, 0, b1y.y), size);
            m_yCollider.center = new Vector3(b3y.x, Mathf.Clamp(b1y.y, 0, b1y.y) + m_yCollider.size.y / 2, b3y.z);

            Vector3 b3z, b1z;
            b3z = m_b3z[i].position;
            b3z = root.InverseTransformPoint(b3z);
            b1z = m_b1z[i].position;
            b1z = root.InverseTransformPoint(b1z);
            m_zCollider.size = new Vector3(size, size, b3z.z - Mathf.Clamp(b1z.z, 0, b1z.z));
            m_zCollider.center = new Vector3(b3z.x, b3z.y, Mathf.Clamp(b1z.z, 0, b1z.z) + m_zCollider.size.z / 2);

            if(ModelScale > 0)
            {
                b1x /= ModelScale;
                b1y /= ModelScale;
                b1z /= ModelScale;

                m_xyCollider.size = new Vector3(Mathf.Abs(b1x.x * SelectionMargin), Mathf.Abs(b1y.y * SelectionMargin), size2);
                m_xyCollider.center = new Vector3(b1x.x * SelectionMargin / 2, b1y.y * SelectionMargin / 2, 0);

                m_xzCollider.size = new Vector3(Mathf.Abs(b1x.x * SelectionMargin), size2, Mathf.Abs(b1z.z * SelectionMargin));
                m_xzCollider.center = new Vector3(b1x.x * SelectionMargin / 2, 0, b1z.z * SelectionMargin / 2);

                m_yzCollider.size = new Vector3(size2, Mathf.Abs(b1y.y * SelectionMargin), Mathf.Abs(b1z.z * SelectionMargin));
                m_yzCollider.center = new Vector3(0, b1y.y * SelectionMargin / 2, b1z.z * SelectionMargin / 2);

                m_snappingCollider.radius = Mathf.Abs(b1x.x * SelectionMargin) / ModelScale;
            }
            else
            {
                m_xyCollider.size = Vector3.zero;
                m_xyCollider.center = Vector3.zero;
                m_xzCollider.size = Vector3.zero;
                m_xzCollider.center = Vector3.zero;
                m_yzCollider.size = Vector3.zero;
                m_yzCollider.center = Vector3.zero;
                m_snappingCollider.radius = 0;
            }
        }

        public override RuntimeHandleAxis HitTest(Ray ray, out float distance)
        {
            if (!m_useColliders)
            {
                distance = float.PositiveInfinity;
                return RuntimeHandleAxis.None;
            }

            Collider collider = null;
            float minDistance = float.MaxValue;

            if(m_isVertexSnapping)
            {
                RaycastHit hit;
                if(m_snappingCollider.Raycast(ray, out hit, Window.Camera.farClipPlane))
                {
                    collider = hit.collider;
                    minDistance = hit.distance;
                }
            }
            else
            {
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    RaycastHit hit;
                    if (m_colliders[i].Raycast(ray, out hit, Window.Camera.farClipPlane))
                    {
                        if(m_lockObj.PositionX)
                        {
                            if(hit.collider == m_xCollider || hit.collider == m_xyCollider || hit.collider == m_xzCollider)
                            {
                                continue;
                            }
                            
                        }
                        if(m_lockObj.PositionY)
                        {
                            if (hit.collider == m_yCollider || hit.collider == m_yzCollider || hit.collider == m_xyCollider)
                            {
                                continue;
                            }
                        }
                        if(m_lockObj.PositionZ)
                        {
                            if (hit.collider == m_zCollider || hit.collider == m_yzCollider || hit.collider == m_xzCollider)
                            {
                                continue;
                            }
                        }

                        if (hit.distance < minDistance)
                        {
                            collider = hit.collider;
                            minDistance = hit.distance;
                        }
                    }
                }
            }

            distance = minDistance;
            if (collider == m_xCollider)
            {
                return RuntimeHandleAxis.X;
            }

            if (collider == m_yCollider)
            {
                return RuntimeHandleAxis.Y;
            }

            if (collider == m_zCollider)
            {
                return RuntimeHandleAxis.Z;
            }

            if (collider == m_xyCollider)
            {
                return RuntimeHandleAxis.XY;
            }

            if (collider == m_xzCollider)
            {
                return RuntimeHandleAxis.XZ;
            }

            if(collider == m_yzCollider)
            {
                return RuntimeHandleAxis.YZ;
            }

            if(collider == m_snappingCollider)
            {
                return RuntimeHandleAxis.Snap;
            }

            distance = float.PositiveInfinity;
            return RuntimeHandleAxis.None;
        }

        private float m_prevRadius;
        private float m_prevLength;
        private float m_prevArrowRadius;
        private float m_prevArrowLength;
        private float m_prevQuadLength;

        private Vector3 m_prevCameraPosition = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private Vector3 m_prevPosition;
        private Quaternion m_prevRotation;
        private int m_prevIndex = -1;

        public int SetCameraPosition(Vector3 pos, bool force = false)
        {
            Vector3 toCam = (pos - transform.position).normalized;
            toCam = transform.InverseTransformDirection(toCam);

            int index = -1;
            if (toCam.x >= 0)
            {
                if (toCam.y >= 0)
                {
                    if (toCam.z >= 0)
                    {
                        index = 0;
                    }
                    else
                    {
                        index = 4;
                    }
                }
                else
                {
                    if (toCam.z >= 0)
                    {
                        index = 3;
                    }
                    else
                    {
                        index = 7;
                    }
                }
            }
            else
            {
                if (toCam.y >= 0)
                {
                    if (toCam.z >= 0)
                    {
                        index = 1;
                    }
                    else
                    {
                        index = 5;
                    }
                }
                else
                {
                    if (toCam.z >= 0)
                    {
                        index = 2;
                    }
                    else
                    {
                        index = 6;
                    }
                }
            }

            index = (index + (transform.localScale.z < 0 ? 4 : 0)) % 8;

            if (m_lockObj != null && (m_lockObj.PositionX || m_lockObj.PositionY || m_lockObj.PositionZ))
            {
                index = 0;
            }

            if(m_prevIndex == index && !force)
            {
                return -1;
            }
            
            if(m_prevIndex >= 0)
            {
                m_models[m_prevIndex].SetActive(false);

                Renderer renderer = m_renderers[m_prevIndex];
                renderer.forceRenderingOff = true;
            }

            if(index >= 0)
            {
                m_models[index].SetActive(true);

                Renderer renderer = m_renderers[index];
                renderer.forceRenderingOff = false;
            }

            PushUpdatesToGraphicLayer();

            m_prevIndex = index;
            return index;
        }

        protected override void Update()
        {
            base.Update();
                   
            if(m_prevCameraPosition != Window.Camera.transform.position || m_prevPosition != transform.position || m_prevRotation != transform.rotation)
            {
                m_prevRotation = transform.rotation;
                m_prevPosition = transform.position;
                m_prevCameraPosition = Window.Camera.transform.position;
                int index = SetCameraPosition(m_prevCameraPosition);
                if (index >= 0)
                {
                    UpdateColliders(index);
                }
            }

            if (m_prevRadius != m_radius || m_prevLength != m_length || m_prevArrowRadius != m_arrowRadius || m_prevArrowLength != m_arrowLength || m_prevQuadLength != QuadLength)
            {
                m_prevRadius = m_radius;
                m_prevLength = m_length;
                m_prevArrowRadius = m_arrowRadius;
                m_prevArrowLength = m_arrowLength;
                m_prevQuadLength = QuadLength;

                UpdateModel();
            }
        }


        
        
    }
}


