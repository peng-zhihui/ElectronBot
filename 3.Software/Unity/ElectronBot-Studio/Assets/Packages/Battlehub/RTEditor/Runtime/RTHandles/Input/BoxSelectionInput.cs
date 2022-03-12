using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class BoxSelectionInput : MonoBehaviour
    {
        private int MouseButton = 0;
        protected BoxSelection m_boxSelection;
        protected IRTE m_editor;

        private void Start()
        {
            if (m_boxSelection == null)
            {
                m_boxSelection = GetComponent<BoxSelection>();
            }
            m_editor = m_boxSelection.Editor;
        }

        private bool m_pointerPressed = false;
        private void LateUpdate()
        {
            if (!m_editor.Input.GetPointer(MouseButton))
            {
                if(m_pointerPressed)
                {
                    m_pointerPressed = false;
                    m_boxSelection.EndSelect();
                }
            }

            if(!m_boxSelection.enabled)
            {
                return;
            }

            if (!m_boxSelection.IsWindowActive)
            {
                return;
            }

            if (m_editor.Tools.ActiveTool != null && m_editor.Tools.ActiveTool != m_boxSelection)
            {
                return;
            }

            if (m_editor.Input.GetPointerDown(MouseButton))
            {
                m_pointerPressed = true;
                m_boxSelection.BeginSelect();
            }
           
        }
    }
}

