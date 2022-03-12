using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class SceneGizmoInput : MonoBehaviour
    {
        private SceneGizmo m_sceneGizmo;
        private IRTE m_editor;

        private void Start()
        {
            if (m_sceneGizmo == null)
            {
                m_sceneGizmo = GetComponent<SceneGizmo>();
            }
            m_editor = m_sceneGizmo.Window.Editor;
        }

        private void Update()
        {
            if(m_editor.ActiveWindow != m_sceneGizmo.Window && m_sceneGizmo.Window.IsPointerOver)
            {
                return;
            }

            if (m_editor.Input.GetPointerUp(0))
            {
               m_sceneGizmo.Click();
            }
        }
    }

}
