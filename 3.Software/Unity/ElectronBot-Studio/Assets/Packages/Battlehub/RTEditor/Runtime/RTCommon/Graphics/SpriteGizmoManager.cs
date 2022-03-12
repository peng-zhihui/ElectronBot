using Battlehub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
    public interface ISpriteGizmoManager
    {
        void Register(Type type, Material material);
        Material Unregister(Type type);
        void Refresh();
    }

    [DefaultExecutionOrder(-1)]
    public class SpriteGizmoManager : MonoBehaviour, ISpriteGizmoManager
    {
        private readonly Dictionary<Type, string> m_builtIn = new Dictionary<Type, string>
            {
                {  typeof(Light), "BattlehubLightGizmo" },
                {  typeof(Camera), "BattlehubCameraGizmo" },
                {  typeof(AudioSource), "BattlehubAudioSourceGizmo" }
            };

        private Dictionary<Type, Tuple<Mesh, Material>> m_registered = new Dictionary<Type, Tuple<Mesh, Material>>();
        private Dictionary<Type, Tuple<Mesh, Material>> m_typeToMeshAndMaterial;
        private Type[] m_types;
        private IRTE m_editor;
        private IRTEGraphics m_graphics;
        private IMeshesCache m_meshesCache;
  
        [SerializeField]
        private float m_gizmoScale = 1;
        public float GizmoScale
        {
            get { return m_gizmoScale; }
            set { m_gizmoScale = value; }
        }

        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            if(m_editor == null)
            {
                Debug.LogError("RTE is null");
            }

            IOC.RegisterFallback<ISpriteGizmoManager>(this);
            AwakeOverride();
        }

        private void Start()
        {
            m_graphics = IOC.Resolve<IRTEGraphics>();
            m_meshesCache = m_graphics.CreateSharedMeshesCache(CameraEvent.BeforeImageEffects);
            m_meshesCache.RefreshMode = CacheRefreshMode.OnTransformChange;

            Refresh();
            StartOverride();
        }

        private void OnDestroy()
        {
            Cleanup();

            foreach (Type type in m_registered.Keys.ToArray())
            {
                Unregister(type);
            }

            m_graphics.DestroySharedMeshesCache(m_meshesCache);

            m_typeToMeshAndMaterial = null;
            m_types = null;

            OnDestroyOverride();

            IOC.UnregisterFallback<ISpriteGizmoManager>(this);
        }

        protected virtual void AwakeOverride()
        {

        }

        protected virtual void StartOverride()
        {

        }

        protected virtual void OnDestroyOverride()
        {

        }

        protected virtual Type[] GetTypes(Type[] types)
        {
            return types;
        }

        public void Register(Type type, Material material)
        {
            if(!material.enableInstancing)
            {
                Debug.LogWarning("material enableInstance == false");
                return;
            }

            m_registered[type] = new Tuple<Mesh, Material>(GraphicsUtility.CreateQuad(), material);
        }

        public Material Unregister(Type type)
        {
            Tuple<Mesh, Material> tuple;
            if (!m_registered.TryGetValue(type, out tuple))
            {
                return null;
            }

            if (tuple.Item1 != null)
            {
                Destroy(tuple.Item1);
            }

            return tuple.Item2;
        }

        public void Refresh()
        {
            Cleanup();
            Initialize();
        }

        protected virtual void GreateGizmo(GameObject go,  Component component, Type type)
        {
            Tuple<Mesh, Material> tuple;
            if (m_typeToMeshAndMaterial.TryGetValue(type, out tuple))
            {
                SpriteGizmo gizmo = go.GetComponent<SpriteGizmo>();
                if (!gizmo)
                {
                    gizmo = go.AddComponent<SpriteGizmo>();
                    gizmo.Component = component;
                    gizmo.ComponentDestroyed += OnComponentDestroyed;
                }

                gizmo.Mesh = tuple.Item1;
                m_meshesCache.Add(gizmo.Mesh, gizmo.transform);
                m_meshesCache.SetMaterial(tuple.Item1, tuple.Item2);
            }
        }

        protected virtual void DestroyGizmo(GameObject go)
        {
            SpriteGizmo gizmo = go.GetComponent<SpriteGizmo>();
            if (gizmo)
            {
                Destroy(gizmo);
                m_meshesCache.Remove(gizmo.Mesh, gizmo.transform);
            }
        }

        private void OnComponentDestroyed(SpriteGizmo gizmo)
        {
            gizmo.ComponentDestroyed -= OnComponentDestroyed;
            Destroy(gizmo);
            m_meshesCache.Remove(gizmo.Mesh, gizmo.transform);
            m_meshesCache.Refresh();
        }

        private void Initialize()
        {
            if (m_types != null)
            {
                Debug.LogWarning("Already initialized");
                return;
            }

            m_typeToMeshAndMaterial = new Dictionary<Type, Tuple<Mesh, Material>>();
            foreach(KeyValuePair<Type, Tuple<Mesh, Material>> kvp in m_registered)
            {
                if (kvp.Value != null)
                {
                    m_typeToMeshAndMaterial.Add(kvp.Key, kvp.Value);
                }   
            }

            foreach (KeyValuePair<Type, string> kvp in m_builtIn)
            {
                if(m_typeToMeshAndMaterial.ContainsKey(kvp.Key))
                {
                    continue;
                }

                Material material = Resources.Load<Material>(kvp.Value);
                if (material != null)
                {
                    m_typeToMeshAndMaterial.Add(kvp.Key, new Tuple<Mesh, Material>(GraphicsUtility.CreateQuad(), material));
                }
            }

            int index = 0;
            m_types = new Type[m_typeToMeshAndMaterial.Count];
            foreach (Type type in m_typeToMeshAndMaterial.Keys)
            {
                m_types[index] = type;
                index++;
            }

            m_types = GetTypes(m_types);
            OnIsOpenedChanged();
            m_editor.IsOpenedChanged += OnIsOpenedChanged;
            m_editor.BeforePlaymodeStateChange += OnBeforePlayModeStateChange;
            m_editor.PlaymodeStateChanged += OnPlayModeStateChanged;
        }

        private void Cleanup()
        {
            if(m_typeToMeshAndMaterial != null)
            {
                foreach(var kvp in m_typeToMeshAndMaterial)
                {
                    if(m_registered.ContainsKey(kvp.Key))
                    {
                        continue;
                    }

                    Mesh mesh = kvp.Value.Item1;
                    if(mesh != null)
                    {
                        Destroy(mesh);
                    }
                }
            }

            m_types = null;
            m_typeToMeshAndMaterial = null;
            if(m_editor != null)
            {
                m_editor.BeforePlaymodeStateChange -= OnBeforePlayModeStateChange;
                m_editor.IsOpenedChanged -= OnIsOpenedChanged;
                m_editor.PlaymodeStateChanged -= OnPlayModeStateChanged;
            }
            UnsubscribeAndDestroy();
        }

   

        private void UnsubscribeAndDestroy()
        {
            Unsubscribe();

            SpriteGizmo[] objs = Resources.FindObjectsOfTypeAll<SpriteGizmo>();
            for (int j = 0; j < objs.Length; ++j)
            {
                SpriteGizmo obj = objs[j];
                if (!obj.gameObject.IsPrefab())
                {
                    DestroyGizmo(obj.gameObject);
                }
            }
        }

        private void OnBeforePlayModeStateChange()
        {
            UnsubscribeAndDestroy();
        }

        private void OnPlayModeStateChanged()
        {
            SubscribeAndCreate();
        }

        private void OnIsOpenedChanged()
        {
            if (m_editor.IsOpened)
            {
                SubscribeAndCreate();
            }
            else
            {
                UnsubscribeAndDestroy();
            }
        }

        private void SubscribeAndCreate()
        {
            IEnumerable<ExposeToEditor> objects = m_editor.Object.Get(false);

            for (int i = 0; i < m_types.Length; ++i)
            {
                IEnumerable<ExposeToEditor> objectsOfType = objects;
                foreach (ExposeToEditor obj in objectsOfType)
                {
                    Component component = obj.GetComponent(m_types[i]);
                    if (component != null)
                    {
                        GreateGizmo(obj.gameObject, component, m_types[i]);
                    }
                }
            }

            m_meshesCache.Refresh();
            Subscribe();
        }

        private void Subscribe()
        {
            m_editor.Object.Awaked += OnAwaked;
            m_editor.Object.Destroyed += OnDestroyed;
            m_editor.Object.MarkAsDestroyedChanged += OnMarkAsDestroyedChanged;
            m_editor.Object.ComponentAdded += OnComponentAdded;
        }

        private void Unsubscribe()
        {
            if(m_editor != null && m_editor.Object != null)
            {
                m_editor.Object.Awaked -= OnAwaked;
                m_editor.Object.Destroyed -= OnDestroyed;
                m_editor.Object.MarkAsDestroyedChanged -= OnMarkAsDestroyedChanged;
                m_editor.Object.ComponentAdded -= OnComponentAdded;
            }
        }

        private void OnAwaked(ExposeToEditor obj)
        {
            bool refresh = false;
            for (int i = 0; i < m_types.Length; ++i)
            {
                Component component = obj.GetComponent(m_types[i]);
                if (component != null)
                {
                    GreateGizmo(obj.gameObject, component, m_types[i]);
                    refresh = true;
                }
            }

            if(refresh)
            {
                m_meshesCache.Refresh();
            }
        }

        private void OnComponentAdded(ExposeToEditor obj, Component component)
        {
            if (Array.IndexOf(m_types, component.GetType()) >= 0)
            {
                GreateGizmo(obj.gameObject, component, component.GetType());
                m_meshesCache.Refresh();
            }
        }

        private void OnDestroyed(ExposeToEditor obj)
        {
            bool refresh = false;
            for (int i = 0; i < m_types.Length; ++i)
            {
                Component component = obj.GetComponent(m_types[i]);
                if (component != null)
                {
                    DestroyGizmo(obj.gameObject);
                    refresh = true;
                }
            }

            if(refresh)
            {
                m_meshesCache.Refresh();
            }
        }

        private void OnMarkAsDestroyedChanged(ExposeToEditor obj)
        {
            bool refresh = false;
            if (obj.MarkAsDestroyed)
            {
                for (int i = 0; i < m_types.Length; ++i)
                {
                    Component component = obj.GetComponent(m_types[i]);
                    if (component != null)
                    {
                        DestroyGizmo(obj.gameObject);
                        refresh = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_types.Length; ++i)
                {
                    Component component = obj.GetComponent(m_types[i]);
                    if (component != null)
                    {
                        GreateGizmo(obj.gameObject, component, m_types[i]);
                        refresh = true;
                    }
                }
            }

            if (refresh)
            {
                m_meshesCache.Refresh();
            }
        }
    }
}
