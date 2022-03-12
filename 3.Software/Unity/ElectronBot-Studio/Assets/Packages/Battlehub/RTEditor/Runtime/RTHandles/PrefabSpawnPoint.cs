using Battlehub.RTCommon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.RTHandles
{
    public class PrefabSpawnPoint : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private GameObject m_prefab = null;

        [SerializeField]
        private Image m_preview = null;

        [SerializeField]
        private Text m_prefabName = null;

        [SerializeField]
        private Vector3 m_prefabScale = Vector3.one;
        protected Vector3 PrefabScale
        {
            get { return m_prefabScale; }
        }

        private Texture2D m_texture;
        private Sprite m_sprite;

        private GameObject m_prefabInstance;
        protected GameObject PrefabInstance
        {
            get { return m_prefabInstance; }
            set { m_prefabInstance = value; }
        }

        private HashSet<Transform> m_prefabInstanceTransforms;
        private Plane m_dragPlane;
                
        private IRTE m_editor;
        private RuntimeWindow m_scene;
        protected RuntimeWindow Scene
        {
            get { return m_scene; }
        }
        
        protected virtual void Start()
        {
            if(m_prefab == null)
            {
                Debug.LogWarning("m_prefab is not set");
                return;
            }

            m_editor = IOC.Resolve<IRTE>();
            m_scene = m_editor.GetWindow(RuntimeWindowType.Scene);

            IResourcePreviewUtility resourcePreview = IOC.Resolve<IResourcePreviewUtility>();
            m_texture = resourcePreview.CreatePreview(m_prefab);
            if (m_preview != null)
            {
                m_preview.sprite = Sprite.Create(m_texture, new Rect(0, 0, m_texture.width, m_texture.height), new Vector2(0.5f, 0.5f));
                m_preview.color = Color.white;
            }

            if(m_prefabName != null)
            {
                m_prefabName.text = m_prefab.name;
            }
        }

        protected virtual void OnDestroy()
        {
            if(m_texture != null)
            {
                Destroy(m_texture);
                m_texture = null;
            }
        }

        protected virtual Plane GetDragPlane(Camera camera, Pointer pointer, Vector3 scenePivot)
        {
            Vector3 up = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(camera.transform.up, Vector3.up)) > Mathf.Cos(Mathf.Deg2Rad))
            {
                up = Vector3.Cross(camera.transform.right, Vector3.up);
            }
            else
            {
                up = Vector3.up;
            }
            return new Plane(up, scenePivot);
        }

        protected virtual bool GetPointOnDragPlane(Camera camera, Pointer pointer, out Vector3 point, out Quaternion rotation)
        {
            Ray ray = pointer;
            float distance;
            if (m_dragPlane.Raycast(ray, out distance))
            {
                point = ray.GetPoint(distance);
                rotation = Quaternion.identity;
                return true;
            }
            point = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }

        protected virtual GameObject InstantiatePrefab(GameObject prefab, Vector3 point, Quaternion rotation)
        {
            GameObject instance = Instantiate(prefab, point, rotation);
            instance.transform.localScale = Vector3.Scale(instance.transform.localScale, PrefabScale);
            return instance;
        }

        protected virtual ExposeToEditor ExposeToEditor(GameObject prefabInstance)
        {
            ExposeToEditor exposeToEditor = prefabInstance.GetComponent<ExposeToEditor>();
            if (exposeToEditor == null)
            {
                exposeToEditor = prefabInstance.AddComponent<ExposeToEditor>();
            }
            return exposeToEditor;
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (m_prefab == null)
            {
                return;
            }

            IScenePivot scenePivot = m_scene.IOCContainer.Resolve<IScenePivot>();
            m_dragPlane = GetDragPlane(m_scene.Camera, m_scene.Pointer, scenePivot.SecondaryPivot);

            bool wasPrefabEnabled = m_prefab.activeSelf;
            m_prefab.SetActive(false);

            Vector3 point;
            Quaternion rotation;
            if (GetPointOnDragPlane(m_scene.Camera, m_scene.Pointer, out point, out rotation))
            {
                m_prefabInstance = InstantiatePrefab(m_prefab, point, rotation);
            }
            else
            {
                m_prefabInstance = InstantiatePrefab(m_prefab, Vector3.zero, Quaternion.identity);
            }

            m_prefabInstanceTransforms = new HashSet<Transform>(m_prefabInstance.GetComponentsInChildren<Transform>(true));
            m_prefab.SetActive(wasPrefabEnabled);

            ExposeToEditor exposeToEditor = ExposeToEditor(m_prefabInstance); 

            exposeToEditor.SetName(m_prefab.name);
            m_prefabInstance.SetActive(true);
        }

      

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector3 point;
            Quaternion rotation;
            if (GetPointOnDragPlane(m_scene.Camera, m_scene.Pointer, out point, out rotation))
            {
                if (m_prefabInstance != null)
                {
                    m_prefabInstance.transform.position = point;
                    m_prefabInstance.transform.rotation = rotation;
                    RaycastHit hit = Physics.RaycastAll(m_scene.Pointer).Where(h => !m_prefabInstanceTransforms.Contains(h.transform)).FirstOrDefault();
                    if (hit.transform != null)
                    {
                        m_prefabInstance.transform.position = hit.point;
                    }
                }
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (m_prefabInstance != null)
            {
                ExposeToEditor exposeToEditor = m_prefabInstance.GetComponent<ExposeToEditor>();
                m_editor.Undo.BeginRecord();
                m_editor.Undo.RegisterCreatedObjects(new[] { exposeToEditor });
                m_editor.Selection.activeObject = m_prefabInstance;
                m_editor.Undo.EndRecord();
            }

            m_prefabInstance = null;
            m_prefabInstanceTransforms = null;
        }
    }
}



