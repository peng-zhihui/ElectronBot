using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battlehub.RTCommon
{
    public interface IDragDropTarget
    {
        void BeginDrag(object[] dragObjects, PointerEventData eventData);
        void DragEnter(object[] dragObjects, PointerEventData eventData);
        void DragLeave(PointerEventData eventData);
        void Drag(object[] dragObjects, PointerEventData eventData);
        void Drop(object[] dragObjects, PointerEventData eventData);
    }

    [DefaultExecutionOrder(-50)]
    public class DragDropTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragDropTarget
    {
        public event DragDropEventHander BeginDragEvent;
        public event DragDropEventHander DragEnterEvent;
        public event DragDropEventHander DragLeaveEvent;
        public event DragDropEventHander DragEvent;
        public event DragDropEventHander DropEvent;

        [SerializeField]
        public GameObject m_dragDropTargetGO;

        private IDragDropTarget[] m_dragDropTargets = new IDragDropTarget[0];

        public virtual bool IsPointerOver
        {
            get;
            set;
        }

        private IRTE m_editor;
        public IRTE Editor
        {
            get { return m_editor; }
        }

        protected virtual void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            if (m_editor == null)
            {
                Debug.LogError("RTE is null");
                return;
            }

            if (m_dragDropTargetGO == null)
            {
                m_dragDropTargets = new[] { this };
            }
            else
            {
                m_dragDropTargets = m_dragDropTargetGO.GetComponents<Component>().OfType<IDragDropTarget>().ToArray();
                if(m_dragDropTargets.Length == 0)
                {
                    Debug.LogWarning("dragDropTargetGO does not contains components with IDragDropTarget interface implemented");
                    m_dragDropTargets = new[] { this };
                }
            }
            AwakeOverride();
        }

        protected virtual void OnDestroy()
        {
            m_dragDropTargets = null;
            OnDestroyOverride();
        }

        protected virtual void AwakeOverride()
        {
        }
        
        protected virtual void OnDestroyOverride()
        {

        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            IsPointerOver = true;
            OnPointerEnterOverride(eventData);
            if (m_editor.DragDrop.InProgress && m_editor.DragDrop.Source != (object)this)
            {
                DragEnter(m_editor.DragDrop.DragObjects, eventData);

                m_editor.DragDrop.BeginDrag += OnBeginDrag;
                m_editor.DragDrop.Drag += OnDrag;
                m_editor.DragDrop.Drop += OnDrop;
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            IsPointerOver = false;
            OnPointerExitOverride(eventData);
            m_editor.DragDrop.BeginDrag -= OnBeginDrag;
            m_editor.DragDrop.Drop -= OnDrop;
            m_editor.DragDrop.Drag -= OnDrag;
            if (m_editor.DragDrop.InProgress && m_editor.DragDrop.Source != (object)this)
            {
                DragLeave(eventData);
            }
        }

        protected virtual void OnPointerEnterOverride(PointerEventData eventData)
        {

        }

        protected virtual void OnPointerExitOverride(PointerEventData eventData)
        {

        }

        private void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (m_editor.DragDrop.InProgress)
            {
                for (int i = 0; i < m_dragDropTargets.Length; ++i)
                {
                    m_dragDropTargets[i].BeginDrag(m_editor.DragDrop.DragObjects, pointerEventData);
                }
            }
        }

        private void OnDrag(PointerEventData pointerEventData)
        {
            if(m_editor.DragDrop.InProgress)
            {
                for (int i = 0; i < m_dragDropTargets.Length; ++i)
                {
                    m_dragDropTargets[i].Drag(m_editor.DragDrop.DragObjects, pointerEventData);
                }
            }   
        }

        private void OnDrop(PointerEventData eventData)
        {
            m_editor.DragDrop.BeginDrag -= OnBeginDrag;
            m_editor.DragDrop.Drop -= OnDrop;
            m_editor.DragDrop.Drag -= OnDrag;
            if (m_editor.DragDrop.InProgress)
            {
                for (int i = 0; i < m_dragDropTargets.Length; ++i)
                {
                    m_dragDropTargets[i].Drop(m_editor.DragDrop.DragObjects, eventData);
                }
            }
        }

        public virtual void BeginDrag(object[] dragObjects, PointerEventData eventData)
        {
            if(BeginDragEvent != null)
            {
                BeginDragEvent(eventData);
            }
        }

        public virtual void DragEnter(object[] dragObjects, PointerEventData eventData)
        {    
            if(DragEnterEvent != null)
            {
                DragEnterEvent(eventData);
            }
        }

        public virtual void Drag(object[] dragObjects, PointerEventData eventData)
        {
            if (DragEvent != null)
            {
                DragEvent(eventData);
            }
        }

        public virtual void DragLeave(PointerEventData eventData)
        {
            if (DragLeaveEvent != null)
            {
                DragLeaveEvent(eventData);
            }
        }

        public virtual void Drop(object[] dragObjects, PointerEventData eventData)
        {
            if (DropEvent != null)
            {
                DropEvent(eventData);
            }
        }

    }
}

