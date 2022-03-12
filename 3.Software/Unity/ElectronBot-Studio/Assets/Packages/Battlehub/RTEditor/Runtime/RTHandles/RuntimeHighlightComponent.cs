using Battlehub.RTCommon;
using UnityEngine;
using System.Linq;
namespace Battlehub.RTHandles
{
    public class RuntimeHighlightComponent : MonoBehaviour
    {
        private IRTE m_editor;
        private IRenderersCache m_cache;
        private bool m_updateRenderers = true;
        private bool m_animatingCamera = false;
        
        private Renderer[] m_renderers;
        private IRuntimeSelectionComponent m_selectionComponent;
        private RuntimeWindow m_activeWindow;
        private Ray m_prevRay;

        private Color32[] m_texPixels;
        private Vector2Int m_texSize;

        private void Start()
        {
            IOC.Register("HighlightRenderers", m_cache = gameObject.AddComponent<RenderersCache>());

            m_editor = IOC.Resolve<IRTE>();

            m_activeWindow = m_editor.ActiveWindow;
            if(m_activeWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Scene)
            {
                m_selectionComponent = m_activeWindow.IOCContainer.Resolve<IRuntimeSelectionComponent>();
            }

            m_editor.Object.Enabled += OnObjectEnabled;
            m_editor.Object.Disabled += OnObjectDisabled;
            m_editor.Object.ComponentAdded += OnComponentAdded;
            
            m_editor.Selection.SelectionChanged += OnSelectionChanged;
            m_editor.ActiveWindowChanged += OnActiveWindowChanged;
        }

        private void OnDestroy()
        {
            IOC.Unregister("HighlightRenderers", m_cache);

            if (m_editor != null)
            {
                if (m_editor.Object != null)
                {
                    m_editor.Object.Enabled -= OnObjectEnabled;
                    m_editor.Object.Disabled -= OnObjectDisabled;
                    m_editor.Object.ComponentAdded -= OnComponentAdded;
                }

                if(m_editor.Selection != null)
                {
                    m_editor.Selection.SelectionChanged -= OnSelectionChanged;
                }

                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
            }
        }

        private void Update()
        {
            if(m_activeWindow == null || m_selectionComponent == null)
            {
                return;
            }

            Ray ray = new Ray(m_activeWindow.Camera.transform.position, m_activeWindow.Camera.transform.forward);
            if (m_prevRay.origin == ray.origin && m_prevRay.direction == ray.direction && !m_editor.Tools.IsViewing)
            {
                if(m_animatingCamera)
                {
                    m_updateRenderers = true;
                }

                m_animatingCamera = false;
            }
            else
            {
                m_prevRay = ray;
                m_animatingCamera = true;
                return;
            }

            if (m_updateRenderers)
            {
                m_updateRenderers = false;
                m_renderers = m_editor.Object.Get(true).SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray();
                m_texPixels = m_selectionComponent.BoxSelection.BeginPick(out m_texSize, m_renderers);
            }
            
            m_cache.Clear();

            BaseHandle handle = null;
            switch (m_editor.Tools.Current)
            {
                case RuntimeTool.Move:
                    handle = m_selectionComponent.PositionHandle;
                    break;
                case RuntimeTool.Rotate:
                    handle = m_selectionComponent.RotationHandle;
                    break;
                case RuntimeTool.Scale:
                    handle = m_selectionComponent.ScaleHandle;
                    break;
                case RuntimeTool.Rect:
                    handle = m_selectionComponent.RectTool;
                    break;
                case RuntimeTool.Custom:
                    handle = m_selectionComponent.CustomHandle;
                    break;
            }

            if(IsPointerOver(handle) || m_editor.Tools.ActiveTool == m_selectionComponent.BoxSelection && m_selectionComponent.BoxSelection != null)
            {
                return;
            }

            Renderer[] renderers = m_selectionComponent.BoxSelection.EndPick(m_texPixels, m_texSize, m_renderers);
            m_cache.Add(renderers, true, true);
        }

        private bool IsPointerOver(BaseHandle handle)
        {
            return handle != null && handle.SelectedAxis != RuntimeHandleAxis.None;
        }

        private void OnObjectEnabled(ExposeToEditor obj)
        {
            m_updateRenderers = true;
        }

        private void OnObjectDisabled(ExposeToEditor obj)
        {
            m_updateRenderers = true;
        }

        private void OnComponentAdded(ExposeToEditor obj, Component arg)
        {
            m_updateRenderers = true;
        }

        private void OnSelectionChanged(Object[] unselectedObjects)
        {
            m_updateRenderers = true;
        }

        private void OnActiveWindowChanged(RuntimeWindow window)
        {
            if(m_editor.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Scene)
            {
                m_updateRenderers = true;
                m_activeWindow = m_editor.ActiveWindow;
                m_selectionComponent = m_activeWindow.IOCContainer.Resolve<IRuntimeSelectionComponent>();
            }    
            else
            {
                m_activeWindow = null;
                m_selectionComponent = null;
                m_renderers = null;
            }
        }
    }
}
