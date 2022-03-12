using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class RuntimeSelectionInput : RuntimeSelectionInputBase
    {
#if UNITY_EDITOR
        public KeyCode m_modifierKey = KeyCode.LeftShift;
#else
        public KeyCode m_modifierKey = KeyCode.LeftControl;
#endif
        protected KeyCode ModifierKey
        {
            get { return m_modifierKey; }
        }

        public KeyCode SelectAllKey = KeyCode.A;

        protected virtual bool MultiselectAction()
        {
            IInput input = m_component.Editor.Input;
            return input.GetKey(ModifierKey);
        }

        protected virtual bool SelectAllAction()
        {
            return Input.GetKeyDown(SelectAllKey) && Input.GetKey(ModifierKey);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if(SelectAllAction())
            {
                SelectAll();
            }
        }

        protected override void OnSelectGO()
        {
            m_component.SelectGO(MultiselectAction(), true);
        }
       
    }

}

