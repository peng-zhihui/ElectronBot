using UnityEngine;
using Battlehub.RTCommon;
namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(-80)]
    public class RuntimeToolsInput : MonoBehaviour
    {
        public KeyCode ViewKey = KeyCode.Q;
        public KeyCode MoveKey = KeyCode.W;
        public KeyCode RotateKey = KeyCode.E;
        public KeyCode ScaleKey = KeyCode.R;
        public KeyCode RectToolKey = KeyCode.T;
        public KeyCode PivotRotationKey = KeyCode.X;
        public KeyCode PivotModeKey = KeyCode.Z;

        private IRTE m_editor;
        
        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();

        }

        private void Start()
        {
            m_editor.Tools.Current = RuntimeTool.Move;
        }


        private void Update()
        {
            if(m_editor.Tools.ActiveTool != null || m_editor.IsInputFieldActive)
            {
                return;
            }

            bool isGameViewActive = m_editor.ActiveWindow != null && m_editor.ActiveWindow.WindowType == RuntimeWindowType.Game;
            bool isLocked = m_editor.Tools.IsViewing || isGameViewActive;
            if (!isLocked)
            {
                if (ViewAction())
                {
                    m_editor.Tools.Current = RuntimeTool.View;
                }
                else if (MoveAction())
                {
                    m_editor.Tools.Current = RuntimeTool.Move;
                }
                else if (RotateAction())
                {
                    m_editor.Tools.Current = RuntimeTool.Rotate;
                }
                else if (ScaleAction())
                {
                    m_editor.Tools.Current = RuntimeTool.Scale;
                }
                else if(RectToolAction())
                {
                    m_editor.Tools.Current = RuntimeTool.Rect;
                }

                if (PivotRotationAction())
                {
                    if (m_editor.Tools.PivotRotation == RuntimePivotRotation.Local)
                    {
                        m_editor.Tools.PivotRotation = RuntimePivotRotation.Global;
                    }
                    else
                    {
                        m_editor.Tools.PivotRotation = RuntimePivotRotation.Local;
                    }
                }
                if (PivotModeAction())
                {
                    if (m_editor.Tools.PivotMode == RuntimePivotMode.Center)
                    {
                        m_editor.Tools.PivotMode = RuntimePivotMode.Pivot;
                    }
                    else
                    {
                        m_editor.Tools.PivotMode = RuntimePivotMode.Center;
                    }
                }
            }
        }

        protected virtual bool ViewAction()
        {
            return m_editor.Input.GetKeyDown(ViewKey);
        }

        protected virtual bool MoveAction()
        {
            return m_editor.Input.GetKeyDown(MoveKey);
        }

        protected virtual bool RotateAction()
        {
            return m_editor.Input.GetKeyDown(RotateKey);
        }

        protected virtual bool ScaleAction()
        {
            return m_editor.Input.GetKeyDown(ScaleKey);
        }

        protected virtual bool RectToolAction()
        {
            return m_editor.Input.GetKeyDown(RectToolKey);
        }

        protected virtual bool PivotRotationAction()
        {
            return m_editor.Input.GetKeyDown(PivotRotationKey);
        }

        protected virtual bool PivotModeAction()
        {
            return m_editor.Input.GetKeyDown(PivotModeKey) &&
                        !(m_editor.Input.GetKey(KeyCode.LeftControl) || m_editor.Input.GetKey(KeyCode.LeftShift));
        }
    }
}

