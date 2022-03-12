using UnityEngine;

namespace Battlehub.RTCommon
{
    public class RuntimeUndoInput : MonoBehaviour
    {
        public KeyCode UndoKey = KeyCode.Z;
        public KeyCode RedoKey = KeyCode.Y;
        public KeyCode RuntimeModifierKey = KeyCode.LeftControl;
        public KeyCode EditorModifierKey = KeyCode.LeftShift;
        public KeyCode ModifierKey
        {
            get
            {
                #if UNITY_EDITOR
                return EditorModifierKey;
                #else
                return RuntimeModifierKey;
                #endif
            }
        }

        private static RuntimeUndoInput m_instance;
        public static bool IsInitialized
        {
            get { return m_instance != null; }
        }


        private IRTE m_rte;
        private void Awake()
        {
            m_rte = IOC.Resolve<IRTE>();
            if(m_rte == null)
            {
                Debug.LogError("m_rte is null");
            }
            m_instance = this;
        }

        private void OnDestroy()
        {
            if(m_instance == this)
            {
                m_instance = null;
            }
        }

        private void Update()
        {
            if (UndoAction())
            {
                m_rte.Undo.Undo();
            }
            else if (RedoAction())
            {
                m_rte.Undo.Redo();
            }
        }

        protected virtual bool UndoAction()
        {
            return m_rte.Input.GetKeyDown(UndoKey) && m_rte.Input.GetKey(ModifierKey);
        }

        protected virtual bool RedoAction()
        {
            return m_rte.Input.GetKeyDown(RedoKey) && m_rte.Input.GetKey(ModifierKey);
        }
    }
}
