
using UnityEngine;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(-65)]
    public class PositionHandleInput : BaseHandleInput
    {
        public KeyCode VertexSnappingKey = KeyCode.V;
        public KeyCode VertexSnappingToggleKey = KeyCode.LeftShift;
        public KeyCode SnapToGroundKey = KeyCode.None;

        protected PositionHandle PositionHandle
        {
            get { return (PositionHandle)m_handle; }
        }

        protected override void Update()
        {
            base.Update();

            if(m_handle == null)
            {
                return;
            }

            if (!m_handle.enabled)
            {
                return;
            }

            if (m_editor.Tools.IsViewing)
            {
                return;
            }

            if(!m_handle.IsWindowActive || !m_handle.Window.IsPointerOver)
            {
                return;
            }

            PositionHandle.SnapToGround = SnapToGroundAction();

            if (BeginVertexSnappingAction())
            {
                PositionHandle.IsInVertexSnappingMode = true;
                if(VertexSnappingToggleAction())
                {
                    m_editor.Tools.IsSnapping = !m_editor.Tools.IsSnapping;
                }
            }
            else if (EndVertexSnappingAction())
            {
                PositionHandle.IsInVertexSnappingMode = false;
            }
        }

        protected virtual bool BeginVertexSnappingAction()
        {
            return m_editor.Input.GetKeyDown(VertexSnappingKey);
        }

        protected virtual bool EndVertexSnappingAction()
        {
            return m_editor.Input.GetKeyUp(VertexSnappingKey);
        }

        protected virtual bool VertexSnappingToggleAction()
        {
            return m_editor.Input.GetKey(VertexSnappingToggleKey);
        }

        protected virtual bool SnapToGroundAction()
        {
            return m_editor.Input.GetKey(SnapToGroundKey);
        }
    }

}

