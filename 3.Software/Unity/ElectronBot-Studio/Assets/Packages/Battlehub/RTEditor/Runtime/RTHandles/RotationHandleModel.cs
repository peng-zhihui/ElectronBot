using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class RotationHandleModel : BaseHandleModel
    {
        private const float DefaultMinorRadius = 0.0075f;
        private const float DefaultMajorRadius = 1.0f;
        private const float DefaultOuterRadius = 1.11f;
        [SerializeField]
        private float m_minorRadius = DefaultMinorRadius;
        [SerializeField]
        private float m_majorRadius = DefaultMajorRadius;
        [SerializeField]
        private float m_outerRadius = DefaultOuterRadius;

        [SerializeField]
        private MeshFilter m_xyz = null;
        [SerializeField]
        private MeshFilter m_innerCircle = null;
        [SerializeField]
        private MeshFilter m_outerCircle = null;
        [SerializeField]
        private MeshFilter m_innerCircleVR = null;
        [SerializeField]
        private MeshFilter m_outerCircleVR = null;

        private MeshFilter m_inner;
        private MeshFilter m_outer;

        private Mesh m_xyzMesh;
        private Mesh m_innerCircleMesh;
        private Mesh m_outerCircleMesh;

        [SerializeField]
        private int m_xMatIndex = 0;
        [SerializeField]
        private int m_yMatIndex = 1;
        [SerializeField]
        private int m_zMatIndex = 2;
        [SerializeField]
        private int m_innerCircleBorderMatIndex = 0;
        [SerializeField]
        private int m_innerCircleFillMatIndex = 1;

        private Material[] m_xyzMaterials;
        private Material[] m_innerCircleMaterials;
        private Material m_outerCircleMaterial;

        private readonly bool m_useColliders = true;
        [SerializeField]
        private Mesh m_axisColliderMesh = null;
        [SerializeField]
        private Mesh m_ssMesh = null;

        private MeshCollider m_xCollider;
        private MeshCollider m_yCollider;
        private MeshCollider m_zCollider;
        private MeshCollider m_innerCollider;
        private MeshCollider m_outerCollider;
        private Collider[] m_colliders;

        protected override void Awake()
        {
            base.Awake();
        
            m_xyzMesh = m_xyz.sharedMesh;

            if(Editor.IsVR)
            {
                Destroy(m_innerCircle.gameObject);
                Destroy(m_outerCircle.gameObject);

                m_inner = m_innerCircleVR;
                m_outer = m_outerCircleVR;
            }
            else
            {
                Destroy(m_innerCircleVR.gameObject);
                Destroy(m_outerCircleVR.gameObject);

                m_inner = m_innerCircle;
                m_outer = m_outerCircle;
            }

            m_innerCircleMesh = m_inner.sharedMesh;
            m_outerCircleMesh = m_outer.sharedMesh;

            Renderer renderer = m_xyz.GetComponent<Renderer>();
            renderer.sharedMaterials = renderer.materials;
            m_xyzMaterials = renderer.sharedMaterials;

            renderer = m_inner.GetComponent<Renderer>();
            renderer.sharedMaterials = renderer.materials;
            m_innerCircleMaterials = renderer.sharedMaterials;

            renderer = m_outer.GetComponent<Renderer>();
            renderer.sharedMaterials = renderer.materials;
            m_outerCircleMaterial = renderer.sharedMaterial;

            Mesh mesh = m_xyz.mesh;
            m_xyz.sharedMesh = mesh;

            mesh = m_inner.mesh;
            m_inner.sharedMesh = mesh;

            mesh = m_outer.mesh;
            m_outer.sharedMesh = mesh;

            if (m_useColliders)
            {
                GameObject colliders = new GameObject("Colliders");
                colliders.transform.SetParent(transform, false);
                colliders.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;
                //m_graphicsBlockingCollider = colliders.AddComponent<SphereCollider>();
                //m_graphicsBlockingCollider.isTrigger = true;

                GameObject xCollider = new GameObject("XAxis");
                xCollider.transform.SetParent(colliders.transform, false);
                xCollider.transform.localRotation = Quaternion.Euler(0, 90, 0);
                m_xCollider = xCollider.AddComponent<MeshCollider>();
                
                GameObject yCollider = new GameObject("YAxis");
                yCollider.transform.SetParent(colliders.transform, false);
                yCollider.transform.localRotation = Quaternion.Euler(90, 0, 0);
                m_yCollider = yCollider.AddComponent<MeshCollider>();
                
                GameObject zCollider = new GameObject("ZAxis");
                zCollider.transform.SetParent(colliders.transform, false);
                m_zCollider = zCollider.AddComponent<MeshCollider>();

                GameObject innerCollider = new GameObject("InnerAxis");
                innerCollider.transform.SetParent(colliders.transform, false);
                m_innerCollider = innerCollider.AddComponent<MeshCollider>();

                GameObject outerCollider = new GameObject("OuterCollider");
                outerCollider.transform.SetParent(colliders.transform, false);
                m_outerCollider = outerCollider.AddComponent<MeshCollider>();

                m_colliders = new[] { m_xCollider, m_yCollider, m_zCollider, m_innerCollider, m_outerCollider };
                foreach(Collider collider in m_colliders)
                {
                    collider.gameObject.SetActive(false);
                    collider.gameObject.layer = Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            SetColors();
        }

        public override void Select(RuntimeHandleAxis axis)
        {
            base.Select(axis);
            SetColors();
        }

        public override void SetLock(LockObject lockObj)
        {
            base.SetLock(lockObj);
            SetColors();
        }

        private void SetDefaultColors()
        {
            if (m_lockObj.RotationX)
            {
                m_xyzMaterials[m_xMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_xyzMaterials[m_xMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_xyzMaterials[m_xMatIndex].color = Colors.XColor;
                m_xyzMaterials[m_xMatIndex].SetFloat("_ZWrite", 1);
            }

            if (m_lockObj.RotationY)
            {
                m_xyzMaterials[m_yMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_xyzMaterials[m_yMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_xyzMaterials[m_yMatIndex].color = Colors.YColor;
                m_xyzMaterials[m_yMatIndex].SetFloat("_ZWrite", 1);
            }

            if (m_lockObj.RotationZ)
            {
                m_xyzMaterials[m_zMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_xyzMaterials[m_zMatIndex].SetFloat("_ZWrite", 0);
                }
            }
            else
            {
                m_xyzMaterials[m_zMatIndex].color = Colors.ZColor;
                m_xyzMaterials[m_zMatIndex].SetFloat("_ZWrite", 1);
            }

            if(m_lockObj.RotationScreen)
            {
                m_outerCircleMaterial.color = Colors.DisabledColor;
            }
            else
            {
                m_outerCircleMaterial.color = Colors.AltColor;
            }
            m_outerCircleMaterial.SetInt("_ZTest", 2);

            if (m_lockObj.RotationFree)
            {
                
                m_innerCircleMaterials[m_innerCircleBorderMatIndex].color = Colors.DisabledColor;
                if (Mathf.Approximately(Colors.DisabledColor.a, 0))
                {
                    m_innerCircleMaterials[m_innerCircleBorderMatIndex].SetFloat("_ZWrite", 0);
                    
                    if (m_lockObj.RotationX && m_lockObj.RotationY || m_lockObj.RotationY && m_lockObj.RotationZ || m_lockObj.RotationX && m_lockObj.RotationZ)
                    {
                        m_innerCircle.gameObject.SetActive(false);
                        Renderer renderer = m_innerCircle.GetComponent<Renderer>();
                        if(renderer != null)
                        {
                            renderer.forceRenderingOff = true;
                        }
                    }
                    else
                    {
                        m_innerCircle.gameObject.SetActive(true);
                        Renderer renderer = m_innerCircle.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.forceRenderingOff = false;
                        }
                    }
                }
            }
            else
            {
                m_innerCircleMaterials[m_innerCircleBorderMatIndex].color = Colors.AltColor2;
                m_innerCircleMaterials[m_innerCircleBorderMatIndex].SetFloat("_ZWrite", 1);
                m_inner.gameObject.SetActive(true);
            }

            m_innerCircleMaterials[m_innerCircleFillMatIndex].color = new Color(0, 0, 0, 0);
        }

        private void SetColors()
        {
            SetDefaultColors();
            switch (m_selectedAxis)
            {
                case RuntimeHandleAxis.X:
                    if (!m_lockObj.RotationX)
                    {
                        m_xyzMaterials[m_xMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Y:
                    if (!m_lockObj.RotationY)
                    {
                        m_xyzMaterials[m_yMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Z:
                    if (!m_lockObj.RotationZ)
                    {
                        m_xyzMaterials[m_zMatIndex].color = Colors.SelectionColor;
                    }
                    break;
                case RuntimeHandleAxis.Free:
                    if(!m_lockObj.RotationFree)
                    {
                        m_innerCircleMaterials[m_innerCircleFillMatIndex].color = Colors.SelectionAltColor;
                    }
                    break;
                case RuntimeHandleAxis.Screen:
                    if(!m_lockObj.RotationScreen)
                    {
                        m_outerCircleMaterial.color = Colors.SelectionColor;
                        m_outerCircleMaterial.SetInt("_ZTest", 0);
                    }
                    break;
            }
        }

        private void UpdateXYZ(Mesh mesh, float majorRadius, float minorRadius)
        {
            m_xyz.transform.localScale = Vector3.one * majorRadius;
            minorRadius /= Mathf.Max(0.01f, majorRadius);

            Vector3[] verts = m_xyzMesh.vertices;
            for(int s = 0; s < m_xyzMesh.subMeshCount; ++s)
            {
                int[] tris =  mesh.GetTriangles(s);
                for(int t = 0; t < tris.Length; ++t)
                {
                    int tri = tris[t];
                    Vector3 v = verts[tri];
                    Vector3 c = v;
                   
                    if(s == 0)
                    {
                        c.x = 0;
                    }
                    else if(s == 1)
                    {
                        c.y = 0;
                    }
                    else if(s == 2)
                    {
                        c.z = 0;
                    }

                    c.Normalize();
                    verts[tri] = c + (v - c).normalized * minorRadius;
                }
            }
            mesh.vertices = verts;
        }

        private void UpdateCircle(Mesh mesh, Mesh originalMesh, Transform circleTransform, float majorRadius, float minorRadius)
        {
            circleTransform.localScale = transform.localScale.z < 0 ? new Vector3(1, 1, -1) * majorRadius : Vector3.one * majorRadius;
            minorRadius /= Mathf.Max(0.01f, majorRadius);

            Vector3[] verts = originalMesh.vertices;
            int[] tris = mesh.GetTriangles(0);
            for (int t = 0; t < tris.Length; ++t)
            {
                int tri = tris[t];
                Vector3 v = verts[tri];
                Vector3 c = v;
                c.z = 0;

                c.Normalize();

                verts[tri] = c + (v - c).normalized * minorRadius;
            }

            if(mesh.subMeshCount > 1)
            {
                tris = mesh.GetTriangles(1);
                for (int t = 0; t < tris.Length; ++t)
                {
                    int tri = tris[t];
                    Vector3 v = verts[tri];
                    v.Normalize();

                    verts[tri] = v * (1 - minorRadius);
                }
            }
            
            mesh.vertices = verts;
        }


        private void UpdateColliders()
        {
            if (!m_useColliders)
            {
                return;
            }

            float majorRadius = m_majorRadius * ModelScale;//  * SelectionMargin;
            float minorRadius = m_minorRadius * SelectionMargin * 10;// * ModelScale * SelectionMargin;
            float outerRadius = m_outerRadius * ModelScale;//  * SelectionMargin;

            Mesh axisMesh = Instantiate(m_axisColliderMesh);
            UpdateCircle(axisMesh, m_axisColliderMesh, m_xCollider.transform, majorRadius, minorRadius);
            m_xCollider.sharedMesh = null;
            m_xCollider.sharedMesh = axisMesh;
            UpdateCircle(axisMesh, m_axisColliderMesh, m_yCollider.transform, majorRadius, minorRadius);
            m_yCollider.sharedMesh = null;
            m_yCollider.sharedMesh = axisMesh;
            UpdateCircle(axisMesh, m_axisColliderMesh, m_zCollider.transform, majorRadius, minorRadius);
            m_zCollider.sharedMesh = null;
            m_zCollider.sharedMesh = axisMesh;

            Mesh innerMesh = Instantiate(m_ssMesh);
            UpdateCircle(innerMesh, m_ssMesh, m_innerCollider.transform, majorRadius, minorRadius);
            m_innerCollider.sharedMesh = null;
            m_innerCollider.sharedMesh = innerMesh;

            Mesh outerMesh = Instantiate(m_axisColliderMesh);
            UpdateCircle(outerMesh, m_ssMesh, m_outerCollider.transform, outerRadius, minorRadius);
            m_outerCollider.sharedMesh = null;
            m_outerCollider.sharedMesh = outerMesh;

            //m_graphicsBlockingCollider.radius = outerRadius;
        }

        public override RuntimeHandleAxis HitTest(Ray ray, out float distance)
        {
            if (!m_useColliders)
            {
                distance = float.PositiveInfinity;
                return RuntimeHandleAxis.None;
            }

            Collider hitCollider = null;
            float minDistance = float.MaxValue;

            Camera camera = Window.Camera;// Editor.ActiveWindow.Camera;
            if (Editor.IsVR)
            {
                m_innerCollider.transform.LookAt(m_innerCollider.transform.position - (camera.transform.position - m_innerCollider.transform.position), Vector3.up);
                m_outerCollider.transform.LookAt(m_outerCollider.transform.position - (camera.transform.position - m_outerCollider.transform.position), Vector3.up);
            }
            else
            {
                m_innerCollider.transform.LookAt(m_innerCollider.transform.position + camera.transform.rotation * Vector3.forward,
                    camera.transform.rotation * Vector3.up);
                m_outerCollider.transform.LookAt(m_outerCollider.transform.position + camera.transform.rotation * Vector3.forward,
                    camera.transform.rotation * Vector3.up);
            }

            for (int i = 0; i < m_colliders.Length; ++i)
            {
                Collider collider = m_colliders[i];
                if (collider == m_innerCollider)
                {
                    if(m_lockObj.RotationFree)
                    {
                        continue;
                    }
                }
                else if(collider == m_xCollider)
                {
                    if(m_lockObj.RotationX)
                    {
                        continue;
                    }
                }
                else if(collider == m_yCollider)
                {
                    if(m_lockObj.RotationY)
                    {
                        continue;
                    }
                }
                else if(collider == m_zCollider)
                {
                    if(m_lockObj.RotationZ)
                    {
                        continue;
                    }
                }
                else if(collider == m_outerCollider)
                {
                    if(m_lockObj.RotationScreen)
                    {
                        continue;
                    }
                }

                m_colliders[i].gameObject.SetActive(true);
                RaycastHit hit;
                if (m_colliders[i].Raycast(ray, out hit, camera.farClipPlane))
                {
                    if (hit.distance < minDistance)
                    {
                        hitCollider = hit.collider;
                        minDistance = hit.distance;
                    }
                }

                m_colliders[i].gameObject.SetActive(false);
            }

            distance = minDistance;
            if (hitCollider == m_xCollider)
            {
                return RuntimeHandleAxis.X;
            }

            if (hitCollider == m_yCollider)
            {
                return RuntimeHandleAxis.Y;
            }

            if (hitCollider == m_zCollider)
            {
                return RuntimeHandleAxis.Z;
            }

            if (hitCollider == m_innerCollider)
            {
                return RuntimeHandleAxis.Free;
            }

            if(hitCollider == m_outerCollider)
            {
                return RuntimeHandleAxis.Screen;
            }

            distance = float.PositiveInfinity;
            return RuntimeHandleAxis.None;
        }

        private float m_prevMinorRadius = DefaultMinorRadius;
        private float m_prevMajorRadius = DefaultMajorRadius;
        private float m_prevOuterRadius = DefaultOuterRadius;
        protected override void Update()
        {
            base.Update();
            if(m_prevMinorRadius != m_minorRadius || m_prevMajorRadius != m_majorRadius || m_prevOuterRadius != m_outerRadius)
            {
                m_prevMinorRadius = m_minorRadius;
                m_prevMajorRadius = m_majorRadius;
                m_prevOuterRadius = m_outerRadius;
                UpdateModel();       
            }
        }

        public override void UpdateModel()
        {
            float majorRadius = m_majorRadius * ModelScale;
            float minorRadius = m_minorRadius * ModelScale;
            float outerRadius = m_outerRadius * ModelScale;
            UpdateXYZ(m_xyz.sharedMesh, majorRadius, minorRadius);
            UpdateCircle(m_inner.sharedMesh, m_innerCircleMesh, m_inner.transform, majorRadius, minorRadius);
            UpdateCircle(m_outer.sharedMesh, m_outerCircleMesh, m_outer.transform, outerRadius, minorRadius);
            UpdateColliders();

            base.UpdateModel();
        }
    }

}
