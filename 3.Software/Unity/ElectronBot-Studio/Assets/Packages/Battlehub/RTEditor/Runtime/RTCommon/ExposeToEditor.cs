using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Battlehub.Utils;

namespace Battlehub.RTCommon
{
    public enum BoundsType
    {
        Any,
        Mesh,
        SkinnedMesh,
        Custom,
        None,
        Sprite,
        RectTransform,
    }


    public delegate void ExposeToEditorChangeEvent<T>(ExposeToEditor obj, T oldValue, T newValue);
    public delegate void ExposeToEditorEvent(ExposeToEditor obj);
    public delegate void ExposeToEditorEvent<T>(ExposeToEditor obj, T arg);
    public delegate void ExposeToEditorEvent<T, T2>(ExposeToEditor obj, T arg, T2 arg2);

    [System.Serializable]
    public class ExposeToEditorUnityEvent : UnityEvent<ExposeToEditor> { }
    [DisallowMultipleComponent]
    public class ExposeToEditor : MonoBehaviour
    {
        public const string HierarchyRootTag = "HierarchyRoot";

        public static event ExposeToEditorEvent _Awaked;
        public static event ExposeToEditorEvent _Destroying;
        public static event ExposeToEditorEvent _Destroyed;
        public static event ExposeToEditorEvent _MarkAsDestroyedChanging;
        public static event ExposeToEditorEvent _MarkAsDestroyedChanged;
        public static event ExposeToEditorEvent _NameChanged;
        public static event ExposeToEditorEvent _TransformChanged;
        public static event ExposeToEditorEvent _Started;
        public static event ExposeToEditorEvent _Enabled;
        public static event ExposeToEditorEvent _Disabled;
        public static event ExposeToEditorChangeEvent<ExposeToEditor> _ParentChanged;
        public static event ExposeToEditorEvent<Component> _ComponentAdded;
        public static event ExposeToEditorEvent<Component, bool> _ReloadComponentEditor;

        [SerializeField]
        [HideInInspector]
        private Collider[] m_colliders;
        public Collider[] Colliders
        {
            get { return m_colliders; }
            set { m_colliders = value; }
        }

        private SpriteRenderer m_spriteRenderer;
        public SpriteRenderer SpriteRenderer
        {
            get { return m_spriteRenderer; }
        }

        private MeshFilter m_filter;
        public MeshFilter MeshFilter
        {
            get { return m_filter; }
        }

        private SkinnedMeshRenderer m_skinned;
        public SkinnedMeshRenderer SkinnedMeshRenderer
        {
            get { return m_skinned; }
        }

        private static readonly Bounds m_none = new Bounds();
        public ExposeToEditorUnityEvent Selected;
        public ExposeToEditorUnityEvent Unselected;
        public GameObject BoundsObject;
        public BoundsType BoundsType;
        public Bounds CustomBounds;

        public bool CanTransform = true;
        public bool CanInspect = true;
        public bool CanDuplicate = true;
        public bool CanDelete = true;
        public bool CanRename = true;
        public bool CanCreatePrefab = true;
        public bool ShowSelectionGizmo = true;

        [HideInInspector, Obsolete]
        public bool CanSelect = true;

        [HideInInspector]
        public bool CanSnap = true;
        public bool AddColliders = true;

        [SerializeField, HideInInspector]
        private bool m_markAsDestroyed;
        public bool MarkAsDestroyed
        {
            get { return m_markAsDestroyed; }
            set
            {
                if (m_markAsDestroyed != value)
                {
                    SetMarkAsDestroyed(this, value);
                    if (_MarkAsDestroyedChanging != null)
                    {
                        _MarkAsDestroyedChanging(this);
                    }
                    gameObject.hideFlags = m_markAsDestroyed ? HideFlags.DontSave : HideFlags.None;
                    gameObject.SetActive(!m_markAsDestroyed);
                    if (_MarkAsDestroyedChanged != null)
                    {
                        _MarkAsDestroyedChanged(this);
                    }
                }
            }
        }

        private void SetMarkAsDestroyed(ExposeToEditor obj, bool value)
        {
            obj.m_markAsDestroyed = value;
            foreach (ExposeToEditor child in obj.GetChildren(true))
            {
                SetMarkAsDestroyed(child, value);
            }
        }

        private BoundsType m_effectiveBoundsType;
        public BoundsType EffectiveBoundsType
        {
            get { return m_effectiveBoundsType; }
        }
        public Bounds Bounds
        {
            get
            {
                if (m_effectiveBoundsType == BoundsType.Any)
                {
                    if (m_filter != null && m_filter.sharedMesh != null)
                    {
                        return m_filter.sharedMesh.bounds;
                    }
                    else if (m_skinned != null && m_skinned.sharedMesh != null)
                    {
                        return m_skinned.sharedMesh.bounds;
                    }
                    else if (m_spriteRenderer != null)
                    {
                        return m_spriteRenderer.sprite.bounds;
                    }
                    else if(BoundsObject != null && BoundsObject.transform is RectTransform)
                    {
                        return BoundsObject.transform.CalculateRelativeRectTransformBounds();
                    }

                    return CustomBounds;
                }
                else if (m_effectiveBoundsType == BoundsType.Mesh)
                {
                    if (m_filter != null && m_filter.sharedMesh != null)
                    {
                        return m_filter.sharedMesh.bounds;
                    }
                    return m_none;
                }
                else if (m_effectiveBoundsType == BoundsType.SkinnedMesh)
                {
                    if (m_skinned != null && m_skinned.sharedMesh != null)
                    {
                        return m_skinned.sharedMesh.bounds;
                    }
                }
                else if (m_effectiveBoundsType == BoundsType.Sprite)
                {
                    if (m_spriteRenderer != null)
                    {
                        return m_spriteRenderer.sprite.bounds;
                    }
                }
                else if(m_effectiveBoundsType == BoundsType.RectTransform)
                {
                    if (BoundsObject != null && BoundsObject.transform is RectTransform)
                    {
                        return BoundsObject.transform.CalculateRelativeRectTransformBounds();
                    }
                }
                else if (m_effectiveBoundsType == BoundsType.Custom)
                {
                    return CustomBounds;
                }
                return m_none;
            }
        }

        public Vector3 LocalPosition
        {
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
        }

        public Vector3 LocalScale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        private Vector3 m_localEulerAngles;
        public Vector3 LocalEuler
        {
            get
            {
                if (transform.localRotation != Quaternion.Euler(m_localEulerAngles))
                {
                    m_localEulerAngles = transform.localEulerAngles;
                }
                return m_localEulerAngles;
            }
            set
            {
                if (m_localEulerAngles != value)
                {
                    m_localEulerAngles = value;
                    transform.localRotation = Quaternion.Euler(m_localEulerAngles);
                }
            }
        }

        public bool IsAwaked
        {
            get;
            private set;
        }

        private bool m_initialized;
        public void Init()
        {
            if (m_initialized)
            {
                return;
            }
            if (BoundsObject == null)
            {
                BoundsObject = gameObject;
            }
            m_initialized = true;
        }

        private void Awake()
        {
            Init();
            IsAwaked = true;

            m_effectiveBoundsType = BoundsType;
            m_filter = BoundsObject.GetComponent<MeshFilter>();
            m_skinned = BoundsObject.GetComponent<SkinnedMeshRenderer>();

            if (m_filter == null && m_skinned == null)
            {
                m_spriteRenderer = BoundsObject.GetComponent<SpriteRenderer>();
            }

            bool visible = (hideFlags & HideFlags.HideInHierarchy) == 0;
            if (visible)
            {
                if (transform.parent != null && transform.parent.GetComponent<ExposeToEditor>() == null && transform.parent.tag != HierarchyRootTag)
                {
                    //gameObject.hideFlags = HideFlags.HideInHierarchy;
                    visible = false;
                    Debug.LogWarning(gameObject.name + ": parent GameObject is not exposed to editor");
                }
            }

            if (visible)
            {
                if (_Awaked != null)
                {
                    _Awaked(this);
                }
            }
        }

        private void Start()
        {
            if ((hideFlags & HideFlags.HideInHierarchy) == 0)
            {
                if (_Started != null)
                {
                    _Started(this);
                }
            }
        }

        private void OnEnable()
        {
            if ((hideFlags & HideFlags.HideInHierarchy) == 0)
            {
                if (_Enabled != null)
                {
                    _Enabled(this);
                }
            }
        }

        private void OnDisable()
        {
            if ((hideFlags & HideFlags.HideInHierarchy) == 0)
            {
                if (_Disabled != null)
                {
                    _Disabled(this);
                }
            }
        }

        private void OnDestroy()
        {
            if (!m_isPaused)
            {
                if ((hideFlags & HideFlags.HideInHierarchy) == 0)
                {
                    if (_Destroying != null)
                    {
                        _Destroying(this);
                    }
                }

                if ((hideFlags & HideFlags.HideInHierarchy) == 0)
                {
                    if (_Destroyed != null)
                    {
                        _Destroyed(this);
                    }
                }
            }
        }

        private bool m_isPaused;
        private void OnApplicationQuit()
        {
            m_isPaused = true;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (Application.isEditor)
            {
                return;
            }
            m_isPaused = !hasFocus;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            m_isPaused = pauseStatus;
        }

        private void Update()
        {
            if (_TransformChanged != null)
            {
                if (transform.hasChanged)
                {
                    transform.hasChanged = false;

                    if ((hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        if (_TransformChanged != null)
                        {
                            _TransformChanged(this);
                        }
                    }
                }
            }
        }

        private ExposeToEditor m_oldParent;

        private void OnBeforeTransformParentChanged()
        {
            m_oldParent = GetParent();
        }

        private void OnTransformParentChanged()
        {
            ExposeToEditor newParent = GetParent();

            if (m_oldParent != newParent)
            {
                if (_ParentChanged != null)
                {
                    _ParentChanged(this, m_oldParent, newParent);
                }
            }
            m_oldParent = null;
        }

        public void SetName(string name, bool ignoreHideFlags = false)
        {
            if (gameObject.name != name)
            {
                gameObject.name = name;
                if ((hideFlags & HideFlags.HideInHierarchy) == 0 || ignoreHideFlags)
                {
                    if (_NameChanged != null)
                    {
                        _NameChanged(this);
                    }
                }
            }
        }

        public Component AddComponent(Type type)
        {
            Component component = gameObject.AddComponent(type);
            if (_ComponentAdded != null)
            {
                _ComponentAdded(this, component);
            }
            return component;
        }

        public void ReloadComponentEditor(Component component, bool force)
        {
            if (_ReloadComponentEditor != null)
            {
                _ReloadComponentEditor(this, component, force);
            }
        }

        public ExposeToEditor NextSibling(List<GameObject> rootGameObjects)
        {
            int siblingIndex = transform.GetSiblingIndex();

            Transform parent = transform.parent;
            if (parent == null)
            {
                for (int i = siblingIndex + 1; i < rootGameObjects.Count; ++i)
                {
                    if (rootGameObjects[i] != null)
                    {
                        Transform child = rootGameObjects[i].transform;
                        ExposeToEditor sibling = child.GetComponent<ExposeToEditor>();
                        if (sibling != null && !sibling.MarkAsDestroyed)
                        {
                            return sibling;
                        }
                    }
                }
            }
            else
            {
                int childCount = parent.childCount;
                for (int i = siblingIndex + 1; i < childCount; ++i)
                {
                    Transform child = parent.GetChild(i);
                    ExposeToEditor sibling = child.GetComponent<ExposeToEditor>();
                    if (sibling != null && !sibling.MarkAsDestroyed)
                    {
                        return sibling;
                    }
                }
            }

            return null;
        }

        public ExposeToEditor GetParent()
        {
            if (transform.parent != null)
            {
                ExposeToEditor parent = transform.parent.GetComponent<ExposeToEditor>();
                if (parent != null && !parent.MarkAsDestroyed)
                {
                    return parent;
                }
            }
            return null;
        }

        public List<ExposeToEditor> GetChildren(bool includeDestroyed = false)
        {
            List<ExposeToEditor> children = new List<ExposeToEditor>();
            foreach (Transform childTransform in transform)
            {
                ExposeToEditor child = childTransform.GetComponent<ExposeToEditor>();
                if (child != null && (includeDestroyed || !child.MarkAsDestroyed))
                {
                    children.Add(child);
                }
            }
            return children;
        }

        public bool HasChildren(bool includeDestroyed = false)
        {
            foreach (Transform childTransform in transform)
            {
                ExposeToEditor child = childTransform.GetComponent<ExposeToEditor>();
                if (child != null && (includeDestroyed || !child.MarkAsDestroyed))
                {
                    return true;
                }
            }
            return false;
        }

        public Bounds CalculateBounds(float minBoundsSize = 0.1f)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            Vector3 scale = gameObject.transform.localScale;
            gameObject.transform.localScale = Vector3.one;

            if (renderers.Length == 0)
            {
                return new Bounds(transform.position, Vector2.one * minBoundsSize);
            }
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }

            gameObject.transform.localScale = scale;
            return bounds;
        }
    }
}

