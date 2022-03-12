using Battlehub.RTCommon;
using Battlehub.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class OutlineManager : MonoBehaviour, IOutlineManager
    {
        private IRTE m_editor;
        private RuntimeWindow m_sceneWindow;
        private OutlineEffect m_outlineEffect;

        public Camera Camera
        {
            private get;
            set;
        }

        private IRuntimeSelection m_selectionOverride;
        public IRuntimeSelection Selection
        {
            get
            {
                if (m_selectionOverride != null)
                {
                    return m_selectionOverride;
                }

                return m_editor.Selection;
            }
            set
            {
                if (m_selectionOverride != value)
                {
                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged -= OnSelectionChanged;
                    }

                    m_selectionOverride = value;
                    if (m_selectionOverride == m_editor.Selection)
                    {
                        m_selectionOverride = null;
                    }

                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged += OnSelectionChanged;
                    }
                }
            }
        }

        private void Start()
        {
            if (RenderPipelineInfo.Type != RPType.Standard)
            {
                //Debug.Log("OutlineManager is not supported");
                Destroy(this);
                return;
            }

            if (Camera == null)
            {
                Camera = GetComponent<Camera>();
            }

            m_outlineEffect =  Camera.gameObject.AddComponent<OutlineEffect>();
            
            m_editor = IOC.Resolve<IRTE>();

            TryToAddRenderers(m_editor.Selection);
            m_editor.Selection.SelectionChanged += OnRuntimeEditorSelectionChanged;
            m_editor.Object.Enabled += OnObjectEnabled;
            m_editor.Object.Disabled += OnObjectDisabled;

            RTEComponent rteComponent = GetComponentInParent<RTEComponent>();
            if(rteComponent != null)
            {
                m_sceneWindow = rteComponent.Window;
                m_sceneWindow.IOCContainer.RegisterFallback<IOutlineManager>(this);
            }
        }

        private void OnDestroy()
        {
            if(m_sceneWindow != null)
            {
                m_sceneWindow.IOCContainer.UnregisterFallback<IOutlineManager>(this);
            }

            if(m_editor != null)
            {
                if(m_editor.Selection != null)
                {
                    m_editor.Selection.SelectionChanged -= OnRuntimeEditorSelectionChanged;
                }
                
                if(m_editor.Object != null)
                {
                    m_editor.Object.Enabled -= OnObjectEnabled;
                    m_editor.Object.Disabled -= OnObjectDisabled;
                }
            }

            if(m_selectionOverride != null)
            {
                m_selectionOverride.SelectionChanged -= OnSelectionChanged;
            }

            if(m_outlineEffect != null)
            {
                Destroy(m_outlineEffect);
            }
        }

        private void OnObjectEnabled(ExposeToEditor obj)
        {
            if (m_selectionOverride != null)
            {
                OnSelectionChanged(m_selectionOverride.objects);
            }
            else
            {
                OnRuntimeEditorSelectionChanged(m_editor.Selection.objects);
            }
        }

        private void OnObjectDisabled(ExposeToEditor obj)
        {
            if (m_selectionOverride != null)
            {
                OnSelectionChanged(m_selectionOverride.objects);
            }
            else
            {
                OnRuntimeEditorSelectionChanged(m_editor.Selection.objects);
            }
        }

        private void OnRuntimeEditorSelectionChanged(Object[] unselectedObject)
        {
            OnSelectionChanged(m_editor.Selection, unselectedObject);
        }

        private void OnSelectionChanged(Object[] unselectedObjects)
        {
            OnSelectionChanged(m_selectionOverride, unselectedObjects);
        }

        private void OnSelectionChanged(IRuntimeSelection selection, Object[] unselectedObjects)
        {
            TryToRemoveRenderers(unselectedObjects);
            TryToAddRenderers(selection);
        }

        private void TryToRemoveRenderers(Object[] unselectedObjects)
        {
            if (unselectedObjects != null)
            {
                Renderer[] renderers = unselectedObjects.Select(go => go as GameObject).Where(go => go != null).SelectMany(go => go.GetComponentsInChildren<Renderer>(true)).ToArray();
                m_outlineEffect.RemoveRenderers(renderers);

                ICustomOutlinePrepass[] customRenderers = unselectedObjects.Select(go => go as GameObject).Where(go => go != null).SelectMany(go => go.GetComponentsInChildren<ICustomOutlinePrepass>(true)).ToArray();
                m_outlineEffect.RemoveRenderers(customRenderers);
            }
        }

        private void TryToAddRenderers(IRuntimeSelection selection)
        {
            if (selection.gameObjects != null)
            {
                IList<Renderer> renderers = GetRenderers(selection.gameObjects);
                m_outlineEffect.AddRenderers(renderers.ToArray());

                IList<ICustomOutlinePrepass> customRenderers = GetCustomRenderers(selection.gameObjects);
                m_outlineEffect.AddRenderers(customRenderers.ToArray());
            }
        }

        private IList<GameObject> FilterSelection(IList<GameObject> gameObjects)
        {
            IList<GameObject> result = new List<GameObject>();

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];
                if (go == null || go.IsPrefab() || (go.hideFlags & HideFlags.HideInHierarchy) != 0)
                {
                    continue;
                }

                ExposeToEditor exposed = go.GetComponent<ExposeToEditor>();
                if (exposed == null || exposed.ShowSelectionGizmo)
                {
                    result.Add(go);
                }
            }
            return result;
        }

        private IList<Renderer> GetRenderers(IList<GameObject> gameObjects)
        {
            List<Renderer> result = new List<Renderer>();

            gameObjects = FilterSelection(gameObjects);

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];

                foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
                {
                    if (renderer.gameObject.activeInHierarchy && (renderer.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        result.Add(renderer);
                    }
                }
            }

            return result;
        }

        private IList<ICustomOutlinePrepass> GetCustomRenderers(IList<GameObject> gameObjects)
        {
            List<ICustomOutlinePrepass> result = new List<ICustomOutlinePrepass>();

            gameObjects = FilterSelection(gameObjects);

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];

                foreach (ICustomOutlinePrepass customRenderer in go.GetComponentsInChildren<ICustomOutlinePrepass>())
                {
                    Renderer renderer = customRenderer.GetRenderer();
                    if (renderer.gameObject.activeInHierarchy && (renderer.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        result.Add(customRenderer);
                    }
                }
            }

            return result;
        }


        public bool ContainsRenderer(Renderer renderer)
        {
            return m_outlineEffect.ContainsRenderer(renderer);
        }

        public void AddRenderers(Renderer[] renderers)
        {
            m_outlineEffect.AddRenderers(renderers);
        }

        public void RemoveRenderers(Renderer[] renderers)
        {
            m_outlineEffect.RemoveRenderers(renderers);
        }

        public void RecreateCommandBuffer()
        {
            m_outlineEffect.RecreateCommandBuffer();
        }
    }
}

