using Battlehub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityObject = UnityEngine.Object;
namespace Battlehub.RTCommon
{
    public delegate void ObjectEvent(ExposeToEditor obj);
    public delegate void ObjectEvent<T>(ExposeToEditor obj, T arg);
    public delegate void ObjectEvent<T, T2>(ExposeToEditor obj, T arg, T2 arg2);
    public delegate void ObjectParentChangedEvent(ExposeToEditor obj, ExposeToEditor oldValue, ExposeToEditor newValue);
   
    public interface IRuntimeObjects
    {
        event ObjectEvent Awaked;
        event ObjectEvent Started;
        event ObjectEvent Enabled;
        event ObjectEvent Disabled;
        event ObjectEvent Destroying;
        event ObjectEvent Destroyed;
        event ObjectEvent MarkAsDestroyedChanging;
        event ObjectEvent MarkAsDestroyedChanged;
        event ObjectEvent TransformChanged;
        event ObjectEvent NameChanged;
        event ObjectParentChangedEvent ParentChanged;
        event ObjectEvent<Component> ComponentAdded;
        event ObjectEvent<Component, bool> ReloadComponentEditor;

        IEnumerable<ExposeToEditor> Get(bool rootsOnly, bool useCache = true);
    }

    public class RuntimeObjects : MonoBehaviour, IRuntimeObjects
    {
        public event ObjectEvent Awaked;
        public event ObjectEvent Started;
        public event ObjectEvent Enabled;
        public event ObjectEvent Disabled;
        public event ObjectEvent Destroying;
        public event ObjectEvent Destroyed;
        public event ObjectEvent MarkAsDestroyedChanging;
        public event ObjectEvent MarkAsDestroyedChanged;
        public event ObjectEvent TransformChanged;
        public event ObjectEvent NameChanged;
        public event ObjectParentChangedEvent ParentChanged;
        public event ObjectEvent<Component> ComponentAdded;
        public event ObjectEvent<Component, bool> ReloadComponentEditor;

        private IRTE m_editor;

        private ExposeToEditor[] m_enabledObjects;
        private UnityObject[] m_selectedObjects;
        private HashSet<ExposeToEditor> m_editModeCache;
        private HashSet<ExposeToEditor> m_playModeCache;

        public IEnumerable<ExposeToEditor> Get(bool rootsOnly, bool useCache)
        {
            if(rootsOnly)
            {
                if (m_editor.IsPlaying)
                {
                    if(!useCache)
                    {
                        throw new System.InvalidOperationException("Operation is invalid in PlayeMode");
                    }
                    return m_playModeCache.Where(o => o.GetParent() == null);
                }
                else
                {
                    if (!useCache)
                    {
                        List<ExposeToEditor> objects = FindAll();
                        for (int i = 0; i < objects.Count; ++i)
                        {
                            objects[i].Init();
                        }
                        m_editModeCache = new HashSet<ExposeToEditor>(objects);
                    }
                    return m_editModeCache.Where(o => o.GetParent() == null);
                }
            }

            if (m_editor.IsPlaying)
            {
                return m_playModeCache;
            }

            return m_editModeCache;
        }

        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            if(m_editor.IsPlaying || m_editor.IsPlaymodeStateChanging)
            {
                Debug.LogError("Editor should be switched to edit mode");
                return;
            }

            List<ExposeToEditor> objects = FindAll();
            for(int i = 0; i < objects.Count; ++i)
            {
                objects[i].Init();
            }
            m_editModeCache = new HashSet<ExposeToEditor>(objects);
            m_playModeCache = null;

            OnIsOpenedChanged();
            m_editor.PlaymodeStateChanging += OnPlaymodeStateChanging;
            m_editor.IsOpenedChanged += OnIsOpenedChanged;
            m_editor.ActiveWindowChanged += OnActiveWindowChanged;

            ExposeToEditor._Awaked += OnAwaked;
            ExposeToEditor._Enabled += OnEnabled;
            ExposeToEditor._Started += OnStarted;
            ExposeToEditor._Disabled += OnDisabled;
            ExposeToEditor._Destroying += OnDestroying;
            ExposeToEditor._Destroyed += OnDestroyed;
            ExposeToEditor._MarkAsDestroyedChanging += OnMarkAsDestroyedChanging;
            ExposeToEditor._MarkAsDestroyedChanged += OnMarkAsDestroyedChanged;

            ExposeToEditor._TransformChanged += OnTransformChanged;
            ExposeToEditor._NameChanged += OnNameChanged;
            ExposeToEditor._ParentChanged += OnParentChanged;

            ExposeToEditor._ComponentAdded += OnComponentAdded;
            ExposeToEditor._ReloadComponentEditor += OnReloadComponentEditor;
        }

        private void OnDestroy()
        {
            if(m_editor != null)
            {
                m_editor.PlaymodeStateChanging -= OnPlaymodeStateChanging;
                m_editor.IsOpenedChanged -= OnIsOpenedChanged;
                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
            }

            ExposeToEditor._Awaked -= OnAwaked;
            ExposeToEditor._Enabled -= OnEnabled;
            ExposeToEditor._Started -= OnStarted;
            ExposeToEditor._Disabled -= OnDisabled;
            ExposeToEditor._Destroying -= OnDestroying;
            ExposeToEditor._Destroyed -= OnDestroyed;
            ExposeToEditor._MarkAsDestroyedChanging -= OnMarkAsDestroyedChanging;
            ExposeToEditor._MarkAsDestroyedChanged -= OnMarkAsDestroyedChanged;

            ExposeToEditor._TransformChanged -= OnTransformChanged;
            ExposeToEditor._NameChanged -= OnNameChanged;
            ExposeToEditor._ParentChanged -= OnParentChanged;

            ExposeToEditor._ComponentAdded -= OnComponentAdded;
            ExposeToEditor._ReloadComponentEditor -= OnReloadComponentEditor;
        }

        private void OnIsOpenedChanged()
        {
            if (m_editor.IsOpened)
            {
                if(!m_editor.IsApplicationPaused)
                {
                    foreach (ExposeToEditor obj in m_editModeCache)
                    {
                        TryToAddColliders(obj);
                        obj.SendMessage("OnRuntimeEditorOpened", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            else
            {
                if(!m_editor.IsApplicationPaused)
                {
                    foreach (ExposeToEditor obj in m_editModeCache.ToArray())
                    {
                        if (obj != null)
                        {
                            TryToDestroyColliders(obj);
                            obj.SendMessage("OnRuntimeEditorClosed", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }

        private void OnActiveWindowChanged(RuntimeWindow deactivatedWindow)
        {
            if (!m_editor.IsPlaying)
            {
                return;
            }

            if (m_editor.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Game)
            {
                foreach (ExposeToEditor playObj in m_playModeCache)
                {
                    playObj.SendMessage("OnRuntimeActivate", SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                foreach (ExposeToEditor playObj in m_playModeCache)
                {
                    playObj.SendMessage("OnRuntimeDeactivate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        private void OnPlaymodeStateChanging()
        {
            if (m_editor.IsPlaying) 
            {
                m_playModeCache = new HashSet<ExposeToEditor>();
                m_enabledObjects = m_editModeCache.Where(eo => eo != null && eo.gameObject.activeSelf && !eo.MarkAsDestroyed).ToArray();
                m_selectedObjects = m_editor.Selection.objects;

                HashSet<GameObject> selectionHS = new HashSet<GameObject>(m_editor.Selection.gameObjects != null ? m_editor.Selection.gameObjects : new GameObject[0]);
                List<GameObject> playmodeSelection = new List<GameObject>();

                GameObject fakeRoot = new GameObject("FakeRoot");
                fakeRoot.SetActive(false);
                foreach (ExposeToEditor editorObj in m_editModeCache.OrderBy(eo => eo.transform.GetSiblingIndex()))
                {
                    if (editorObj.GetParent() != null)
                    {
                        continue;
                    }

                    editorObj.gameObject.SetActive(false);
                    editorObj.transform.SetParent(fakeRoot.transform);
                }

                GameObject fakeRootPlayMode = Instantiate(fakeRoot);
                for(int i = 0; i < fakeRootPlayMode.transform.childCount; ++i)
                {
                    ExposeToEditor playModeObj = fakeRootPlayMode.transform.GetChild(i).GetComponent<ExposeToEditor>();
                    ExposeToEditor editorObj = fakeRoot.transform.GetChild(i).GetComponent<ExposeToEditor>();

                    playModeObj.SetName(editorObj.name);
                    playModeObj.Init();
                    m_playModeCache.Add(playModeObj);

                    ExposeToEditor[] editorObjAndChildren = editorObj.GetComponentsInChildren<ExposeToEditor>(true);
                    ExposeToEditor[] playModeObjAndChildren = playModeObj.GetComponentsInChildren<ExposeToEditor>(true);
                    for (int j = 0; j < editorObjAndChildren.Length; j++)
                    {
                        if (selectionHS.Contains(editorObjAndChildren[j].gameObject))
                        {
                            playmodeSelection.Add(playModeObjAndChildren[j].gameObject);
                        }
                    }
                }

                
                fakeRoot.transform.DetachChildren();
                fakeRootPlayMode.transform.DetachChildren();
                Destroy(fakeRoot);
                Destroy(fakeRootPlayMode);

                foreach(ExposeToEditor playModeObj in m_playModeCache.ToArray())
                {
                    playModeObj.gameObject.SetActive(true);
                }

                bool isEnabled = m_editor.Undo.Enabled;
                m_editor.Undo.Enabled = false;
                m_editor.Selection.objects = playmodeSelection.ToArray();
                m_editor.Undo.Enabled = isEnabled;
                m_editor.Undo.Store();
            }
            else 
            {
                foreach (ExposeToEditor playObj in m_playModeCache)
                {
                    if(playObj != null)
                    {
                        playObj.SendMessage("OnRuntimeDestroy", SendMessageOptions.DontRequireReceiver);
                        DestroyImmediate(playObj.gameObject);
                    }
                }
                
                for (int i = 0; i < m_enabledObjects.Length; ++i)
                {
                    ExposeToEditor editorObj = m_enabledObjects[i];
                    if (editorObj != null)
                    {
                        editorObj.gameObject.SetActive(true);
                    }
                }

                bool isEnabled = m_editor.Undo.Enabled;
                m_editor.Undo.Enabled = false;
                m_editor.Selection.objects = m_selectedObjects;
                m_editor.Undo.Enabled = isEnabled;
                m_editor.Undo.Restore();

                m_playModeCache = null;
                m_enabledObjects = null;
                m_selectedObjects = null;
            }
        }

        private static bool HasValidState(ExposeToEditor exposeToEditor)
        {
            return exposeToEditor != null &&
                !exposeToEditor.MarkAsDestroyed &&
                (exposeToEditor.hideFlags & HideFlags.HideInHierarchy) == 0 && exposeToEditor.IsAwaked;
        }

        private static List<ExposeToEditor> FindAll()
        {
            if (SceneManager.GetActiveScene().isLoaded)
            {
                return FindAllUsingSceneManagement();
            }
            List<ExposeToEditor> result = new List<ExposeToEditor>();
            ExposeToEditor[] objects = Resources.FindObjectsOfTypeAll<ExposeToEditor>();
            for (int i = 0; i < objects.Length; ++i)
            {
                ExposeToEditor obj = objects[i];
                if (obj == null)
                {
                    continue;
                }

                if(!HasValidState(obj))
                {
                    continue;
                }

                if (!obj.gameObject.IsPrefab())
                {
                    result.Add(obj);
                }
            }

            return result;
        }

        private static List<ExposeToEditor> FindAllUsingSceneManagement()
        {
            List<ExposeToEditor> result = new List<ExposeToEditor>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < rootGameObjects.Length; ++i)
            {
                ExposeToEditor[] exposedObjects = rootGameObjects[i].GetComponentsInChildren<ExposeToEditor>(true);
                for (int j = 0; j < exposedObjects.Length; ++j)
                {
                    ExposeToEditor obj = exposedObjects[j];
                    if (HasValidState(obj))
                    {
                        result.Add(obj);
                    }
                }
            }
            return result;
        }

        private void TryToAddColliders(ExposeToEditor obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj.Colliders == null || obj.Colliders.Length == 0)
            {
                List<Collider> colliders = new List<Collider>();
                Rigidbody rigidBody = obj.BoundsObject.GetComponentInParent<Rigidbody>();

                bool isRigidBody = rigidBody != null;
                if (obj.EffectiveBoundsType == BoundsType.Any)
                {
                    if (obj.MeshFilter != null)
                    {
                        if (obj.AddColliders && !isRigidBody)
                        {
                            MeshCollider collider = obj.BoundsObject.AddComponent<MeshCollider>();
                            collider.convex = isRigidBody;
                            collider.sharedMesh = obj.MeshFilter.sharedMesh;
                            colliders.Add(collider);
                        }
                    }
                    else if (obj.SkinnedMeshRenderer != null)
                    {
                        if (obj.AddColliders && !isRigidBody)
                        {
                            MeshCollider collider = obj.BoundsObject.AddComponent<MeshCollider>();
                            collider.convex = isRigidBody;
                            collider.sharedMesh = obj.SkinnedMeshRenderer.sharedMesh;
                            colliders.Add(collider);
                        }
                    }
                    else if (obj.SpriteRenderer != null)
                    {
                        if (obj.AddColliders && !isRigidBody)
                        {
                            BoxCollider collider = obj.BoundsObject.AddComponent<BoxCollider>();
                            collider.size = obj.SpriteRenderer.sprite.bounds.size;
                            colliders.Add(collider);
                        }
                    }
                    
                }
                else if (obj.EffectiveBoundsType == BoundsType.Mesh)
                {
                    if (obj.MeshFilter != null)
                    {
                        if (obj.AddColliders && !isRigidBody)
                        {
                            MeshCollider collider = obj.BoundsObject.AddComponent<MeshCollider>();
                            collider.convex = isRigidBody;
                            collider.sharedMesh = obj.MeshFilter.sharedMesh;
                            colliders.Add(collider);
                        }
                    }
                }
                else if (obj.EffectiveBoundsType == BoundsType.SkinnedMesh)
                {
                    if (obj.SkinnedMeshRenderer != null)
                    {
                        if (obj.AddColliders && !isRigidBody)
                        {
                            MeshCollider collider = obj.BoundsObject.AddComponent<MeshCollider>();
                            collider.convex = isRigidBody;
                            collider.sharedMesh = obj.SkinnedMeshRenderer.sharedMesh;
                            colliders.Add(collider);
                        }
                    }
                }
                else if (obj.EffectiveBoundsType == BoundsType.Sprite)
                {
                    if (obj.SpriteRenderer != null)
                    {
                        if (obj.AddColliders && !isRigidBody)
                        {
                            BoxCollider collider = obj.BoundsObject.AddComponent<BoxCollider>();
                            collider.size = obj.SpriteRenderer.sprite.bounds.size;
                            colliders.Add(collider);
                        }
                    }
                }
                else if (obj.EffectiveBoundsType == BoundsType.Custom)
                {
                    if (obj.AddColliders && !isRigidBody)
                    {
                        Mesh box = GraphicsUtility.CreateCube(Color.black, obj.CustomBounds.center, 1, obj.CustomBounds.extents.x * 2, obj.CustomBounds.extents.y * 2, obj.CustomBounds.extents.z * 2);

                        MeshCollider collider = obj.BoundsObject.AddComponent<MeshCollider>();
                        collider.convex = isRigidBody;

                        collider.sharedMesh = box;
                        colliders.Add(collider);
                    }
                }

                obj.Colliders = colliders.ToArray();
            }
        }

        private void TryToDestroyColliders(ExposeToEditor obj)
        {
            if (obj != null && obj.Colliders != null)
            {
                for (int i = 0; i < obj.Colliders.Length; ++i)
                {
                    Collider collider = obj.Colliders[i];
                    if (collider != null)
                    {
                        Destroy(collider);
                    }
                }
                obj.Colliders = null;
            }
        }

        private void OnAwaked(ExposeToEditor obj)
        {
            if(m_editor.IsPlaying || m_editor.IsPlaymodeStateChanging)
            {
                obj.SendMessage("RuntimeAwake", SendMessageOptions.DontRequireReceiver);

                if (!m_playModeCache.Contains(obj))
                {
                    m_playModeCache.Add(obj);
                }    
            }
            else
            {
                obj.SendMessage("EditorAwake", SendMessageOptions.DontRequireReceiver);

                if (!m_editModeCache.Contains(obj))
                {
                    m_editModeCache.Add(obj);
                    if (m_editor.IsOpened)
                    {
                        TryToAddColliders(obj);
                    }
                    else
                    {
                        TryToDestroyColliders(obj);
                    }
                }                
            }

            if (Awaked != null)
            {
                Awaked(obj);
            }
        }

        private void OnDestroying(ExposeToEditor obj)
        {
            if (Destroying != null)
            {
                Destroying(obj);
            }
        }

        private void OnDestroyed(ExposeToEditor obj)
        {
            if (m_editor.IsPlaying)
            {
                obj.SendMessage("OnRuntimeDestroy", SendMessageOptions.DontRequireReceiver);
                m_playModeCache.Remove(obj);
            }
            else 
            {
                obj.SendMessage("OnEditorDestroy", SendMessageOptions.DontRequireReceiver);
                if (m_editModeCache.Contains(obj))
                {
                    m_editModeCache.Remove(obj);
                    TryToDestroyColliders(obj);
                }
            }

            if(m_editor.Selection.IsSelected(obj.gameObject))
            {
                m_editor.Selection.objects = m_editor.Selection.objects.Where(o => o != obj.gameObject).ToArray();
            }

            if (Destroyed != null)
            {
                Destroyed(obj);
            }
        }

        private void OnMarkAsDestroyedChanging(ExposeToEditor obj)
        {
            if (m_editor.IsPlaying)
            {
                if(obj.HasChildren(true))
                {
                    ExposeToEditor[] children = obj.GetComponentsInChildren<ExposeToEditor>(true);
                    if(obj.MarkAsDestroyed)
                    {
                        for (int i = 0; i < children.Length; ++i)
                        {
                            ExposeToEditor child = children[i];
                            m_playModeCache.Remove(child);
                            SendMessageTo(child.gameObject, "OnMarkAsDestroyed");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < children.Length; ++i)
                        {
                            ExposeToEditor child = children[i];
                            m_playModeCache.Add(child);
                            SendMessageTo(child.gameObject, "OnMarkAsRestored");
                        }
                    }
                }
                else
                {
                    if (obj.MarkAsDestroyed)
                    {
                        m_playModeCache.Remove(obj);
                        SendMessageTo(obj.gameObject, "OnMarkAsDestroyed");
                    }
                    else
                    {
                        m_playModeCache.Add(obj);
                        SendMessageTo(obj.gameObject, "OnMarkAsRestored");
                    }
                }
            }
            else
            {
                if (obj.HasChildren(true))
                {
                    ExposeToEditor[] children = obj.GetComponentsInChildren<ExposeToEditor>(true);
                    if (obj.MarkAsDestroyed)
                    {
                        for (int i = 0; i < children.Length; ++i)
                        {
                            ExposeToEditor child = children[i];
                            m_editModeCache.Remove(child);
                            SendMessageTo(child.gameObject, "OnMarkAsDestroyed");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < children.Length; ++i)
                        {
                            ExposeToEditor child = children[i];
                            m_editModeCache.Add(child);
                            SendMessageTo(child.gameObject, "OnMarkAsRestored");
                        }
                    }
                }
                else
                {
                    if (obj.MarkAsDestroyed)
                    {
                        
                        m_editModeCache.Remove(obj);
                        SendMessageTo(obj.gameObject, "OnMarkAsDestroyed");
                    }
                    else
                    {
                        m_editModeCache.Add(obj);
                        SendMessageTo(obj.gameObject, "OnMarkAsRestored");
                    }
                }
            }

            if(MarkAsDestroyedChanging != null)
            {
                MarkAsDestroyedChanging(obj);
            }
        }

        public void SendMessageTo(GameObject gameobject, string methodName, params object[] parameters)
        {
            MonoBehaviour[] components = gameobject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour m in components)
            {
                InvokeIfExists(m, methodName, parameters);
            }
        }

        private void InvokeIfExists(object objectToCheck, string methodName, params object[] parameters)
        {
            Type type = objectToCheck.GetType();
            
            MethodInfo methodInfo = type.GetMethod(methodName);
            if (methodInfo != null)
            {
                methodInfo.Invoke(objectToCheck, parameters);
            }
        }

        private void OnMarkAsDestroyedChanged(ExposeToEditor obj)
        {
            if (MarkAsDestroyedChanged != null)
            {
                MarkAsDestroyedChanged(obj);
            }
        }

        private void OnEnabled(ExposeToEditor obj)
        {
            if (Enabled != null)
            {
                Enabled(obj);
            }
        }

        private void OnStarted(ExposeToEditor obj)
        {
            if (m_editor.IsPlaying)
            {
                obj.SendMessage("RuntimeStart", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                obj.SendMessage("EditorStart", SendMessageOptions.DontRequireReceiver);
            }

            if (Started != null)
            {
                Started(obj);
            }
        }

        private void OnDisabled(ExposeToEditor obj)
        {
            if (Disabled != null)
            {
                Disabled(obj);
            }
        }

        private void OnTransformChanged(ExposeToEditor obj)
        {
            if (TransformChanged != null)
            {
                TransformChanged(obj);
            }
        }

        private void OnNameChanged(ExposeToEditor obj)
        {
            if (NameChanged != null)
            {
                NameChanged(obj);
            }
        }

        private void OnParentChanged(ExposeToEditor obj, ExposeToEditor oldValue, ExposeToEditor newValue)
        {
            if (ParentChanged != null)
            {
                ParentChanged(obj, oldValue, newValue);
            }
        }

        private void OnComponentAdded(ExposeToEditor obj, Component component)
        {
            if(ComponentAdded != null)
            {
                ComponentAdded(obj, component);
            }
        }

        private void OnReloadComponentEditor(ExposeToEditor obj, Component component, bool force)
        {
            if(ReloadComponentEditor != null)
            {
                ReloadComponentEditor(obj, component, force);
            }
        }
    }
}

