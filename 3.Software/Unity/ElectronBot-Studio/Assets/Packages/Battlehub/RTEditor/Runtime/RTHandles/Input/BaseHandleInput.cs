using UnityEngine;
using Battlehub.RTCommon;
namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(-60)]
    public class BaseHandleInput : MonoBehaviour
    {
        [SerializeField]
        protected BaseHandle m_handle;
        public virtual BaseHandle Handle
        {
            get { return m_handle; }
            set { m_handle = value; }
        }

        protected IRTE m_editor;

        private void OnEnable()
        {
            if (m_handle == null)
            {
                m_handle = GetComponent<BaseHandle>();
            }

            m_editor = m_handle.Editor;

            if(m_editor != null)
            {
                if (BeginDragAction())
                {
                    m_handle.BeginDrag();
                }
            }
        }

        protected virtual void Start()
        {
            if(m_editor == null)
            {
                m_editor = m_handle.Editor;
            }
        }

        protected virtual void Update()
        {
            if(m_handle == null)
            {
                Destroy(this);
                return;
            }

            if(!m_handle.enabled)
            {
                return;
            }

            if (BeginDragAction())
            {
                m_handle.BeginDrag();
            }
            else if (EndDragAction())
            {
                m_handle.EndDrag();
            }

            if(m_handle != null && m_handle.IsDragging)
            {
                m_handle.UnitSnapping = UnitSnappingAction();
            }
        }

        protected virtual bool BeginDragAction()
        {
            return m_editor.Input.GetPointerDown(0);
        }

        protected virtual bool EndDragAction()
        {
            return m_editor.Input.GetPointerUp(0);
        }

        protected virtual bool UnitSnappingAction()
        {
            return m_editor.Input.GetKey(KeyCode.LeftShift) || m_editor.Tools.UnitSnapping;
        }

    }
}

