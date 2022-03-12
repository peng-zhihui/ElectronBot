using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class ScaleHandleModel : BaseHandleModel
    {
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
        private int m_xyzMatIndex = 6;
        [SerializeField]
        private Transform m_armature = null;
        [SerializeField]
        private Transform m_model = null;

        private Transform m_b1x;
        private Transform m_b2x;
        private Transform m_b3x;
        private Transform m_b1y;
        private Transform m_b2y;
        private Transform m_b3y;
        private Transform m_b1z;
        private Transform m_b2z;
        private Transform m_b3z;
        private Transform m_b0;

        [SerializeField]
        private float m_radius = DefaultRadius;
        [SerializeField]
        private float m_length = DefaultLength;
        [SerializeField]
        private float m_arrowRadius = DefaultArrowRadius;

        private const float DefaultRadius = 0.05f;
        private const float DefaultLength = 1.0f;
        private const float DefaultArrowRadius = 0.1f;

        private Material[] m_materials;

        private Vector3 m_scale = Vector3.one;

        private readonly bool m_useColliders = true;
        private BoxCollider m_xCollider;
        private BoxCollider m_yCollider;
        private BoxCollider m_zCollider;
        private BoxCollider m_xyzCollider;
        private Collider[] m_colliders;

        protected override void Awake()
        {
            base.Awake();
        
            m_b1x = m_armature.GetChild(0);
            m_b1y = m_armature.GetChild(1);
            m_b1z = m_armature.GetChild(2);
            m_b2x = m_armature.GetChild(3);
            m_b2y = m_armature.GetChild(4);
            m_b2z = m_armature.GetChild(5);
            m_b3x = m_armature.GetChild(6);
            m_b3y = m_armature.GetChild(7);
            m_b3z = m_armature.GetChild(8);
            m_b0 = m_armature.GetChild(9);

            Renderer renderer = m_model.GetComponent<Renderer>();
            m_materials = renderer.materials;
            renderer.sharedMaterials = m_materials;

            if (m_useColliders)
            {
                GameObject colliders = new GameObject("Colliders");
                colliders.transform.SetParent(transform, false);
                colliders.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;

                m_xyzCollider = colliders.AddComponent<BoxCollider>();
                m_xCollider = colliders.AddComponent<BoxCollider>();
                m_yCollider = colliders.AddComponent<BoxCollider>();
                m_zCollider = colliders.AddComponent<BoxCollider>();

                m_colliders = new Collider[] { m_xyzCollider, m_xCollider, m_yCollider, m_zCollider };
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    m_colliders[i].isTrigger = true;
                }

                m_xCollider.gameObject.SetActive(false);
            }
        }

        protected override void Start()
        {
            base.Start();
            SetColors();
        }

        protected override void OnWindowActivating()
        {
            base.OnWindowActivating();
            if (m_useColliders)
            {
                m_xCollider.gameObject.SetActive(true);
            }
        }

        protected override void OnWindowDeactivating()
        {
            base.OnWindowDeactivating();
            if(m_xCollider != null)
            {
                m_xCollider.gameObject.SetActive(false);
            }
        }

        public override void SetLock(LockObject lockObj)
        {
            base.SetLock(lockObj);
            SetColors();
        }

        public override void Select(RuntimeHandleAxis axis)
        {
            base.Select(axis);
            SetColors();
        }

        private void SetDefaultColors()
        {
            if (m_lockObj.ScaleX)
            {
                m_materials[m_xMatIndex].color = Colors.DisabledColor;
                m_materials[m_xArrowMatIndex].color = Colors.DisabledColor;

                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
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

            if (m_lockObj.ScaleY)
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

            if (m_lockObj.ScaleZ)
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

            if(m_lockObj.IsScaleLocked)
            {
                m_materials[m_xyzMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_materials[m_xyzMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_materials[m_xyzMatIndex].color = Colors.AltColor;
                m_materials[m_xyzMatIndex].SetFloat("_ZWrite", 1);
            }
        }

        private void SetColors()
        {
            SetDefaultColors();
            switch (m_selectedAxis)
            {
                case RuntimeHandleAxis.X:
                    if (!m_lockObj.ScaleX)
                    {
                        m_materials[m_xArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_xMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Y:
                    if (!m_lockObj.ScaleY)
                    {
                        m_materials[m_yArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_yMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Z:
                    if (!m_lockObj.ScaleZ)
                    {
                        m_materials[m_zArrowMatIndex].color = Colors.SelectionColor;
                        m_materials[m_zMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Free:
                    m_materials[m_xyzMatIndex].color = Colors.SelectionColor;
                    break;
            }
        }

        public override void SetScale(Vector3 scale)
        {
            base.SetScale(scale);
            m_scale = scale;
            if(enabled)
            {
                UpdateModel();
            }
        }

        public override void UpdateModel()
        {
            m_radius = Mathf.Max(0.001f, m_radius);

            float radius = m_radius * ModelScale;
            float arrowRadius = m_arrowRadius * ModelScale;
            float length = m_length * ModelScale;

            float scale = radius / DefaultRadius;
            float arrowScale = arrowRadius / DefaultArrowRadius;

            m_b0.localScale = Vector3.one * arrowScale;// * 2;
            m_b3z.localScale = m_b3y.localScale = m_b3x.localScale = Vector3.one * arrowScale;

            m_b1x.position = transform.TransformPoint(Vector3.right * arrowRadius);
            m_b1y.position = transform.TransformPoint(Vector3.up * arrowRadius);
            m_b1z.position = transform.TransformPoint(Vector3.forward * arrowRadius);

            m_b2x.position = transform.TransformPoint(Vector3.right * (length * m_scale.x - arrowRadius));
            m_b2y.position = transform.TransformPoint(Vector3.up * (length * m_scale.y - arrowRadius));
            m_b2z.position = transform.TransformPoint(Vector3.forward * (length * m_scale.z - arrowRadius));

            m_b2x.localScale = m_b1x.localScale = new Vector3(1, scale, scale);
            m_b2y.localScale = m_b1y.localScale = new Vector3(scale, scale, 1);
            m_b2z.localScale = m_b1z.localScale = new Vector3(scale, 1, scale);

            m_b3x.position = transform.TransformPoint(Vector3.right * length * m_scale.x);
            m_b3y.position = transform.TransformPoint(Vector3.up * length * m_scale.y);
            m_b3z.position = transform.TransformPoint(Vector3.forward * length * m_scale.z);

            UpdateColliders();

            base.UpdateModel();
        }

        private void UpdateColliders()
        {
            if (!m_useColliders)
            {
                return;
            }

            if(m_scale.x <= 0 || m_scale.y <= 0 || m_scale.z <= 0)
            {
                return;
            }

            float size = 2 * m_arrowRadius * SelectionMargin;

            Transform root = m_xCollider.transform.parent;

            Vector3 b3x = root.InverseTransformPoint(m_b3x.position);
            Vector3 b1x = root.InverseTransformPoint(m_b1x.position);
            m_xCollider.size = new Vector3(b3x.x - b1x.x, size, size);
            m_xCollider.center = new Vector3(b1x.x + m_xCollider.size.x / 2, b3x.y, b3x.z);

            Vector3 b3y = root.InverseTransformPoint(m_b3y.position);
            Vector3 b1y = root.InverseTransformPoint(m_b1y.position);
            m_yCollider.size = new Vector3(size, b3y.y - b1y.y, size);
            m_yCollider.center = new Vector3(b3y.x, b1y.y + m_yCollider.size.y / 2, b3y.z);

            Vector3 b3z = root.InverseTransformPoint(m_b3z.position);
            Vector3 b1z = root.InverseTransformPoint(m_b1z.position);
            m_zCollider.size = new Vector3(size, size, b3z.z - b1z.z);
            m_zCollider.center = new Vector3(b3z.x, b3z.y, b1z.z + m_zCollider.size.z / 2);

            m_xyzCollider.size = new Vector3(size * 2, size * 2, size * 2);
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

            for (int i = 0; i < m_colliders.Length; ++i)
            {
                RaycastHit hit;
                if (m_colliders[i].Raycast(ray, out hit, Window.Camera.farClipPlane))
                {
                    if(m_lockObj.ScaleX && m_lockObj.ScaleY && m_lockObj.ScaleZ)
                    {
                        if(hit.collider == m_xyzCollider)
                        {
                            continue;
                        }
                    }
                    if (m_lockObj.ScaleX)
                    {
                        if (hit.collider == m_xCollider)
                        {
                            continue;
                        }
                    }
                    if (m_lockObj.ScaleY)
                    {
                        if (hit.collider == m_yCollider)
                        {
                            continue;
                        }
                    }
                    if (m_lockObj.ScaleZ)
                    {
                        if (hit.collider == m_zCollider)
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

            if (collider == m_xyzCollider)
            {
                return RuntimeHandleAxis.Free;
            }

            distance = float.PositiveInfinity;
            return RuntimeHandleAxis.None;
        }

        private float m_prevRadius;
        private float m_prevLength;
        private float m_prevArrowRadius;
        protected override void Update()
        {
            if (m_prevRadius != m_radius || m_prevLength != m_length || m_prevArrowRadius != m_arrowRadius)
            {
                m_prevRadius = m_radius;
                m_prevLength = m_length;
                m_prevArrowRadius = m_arrowRadius;
                UpdateModel();
            }

            base.Update();

        }
    }
}
