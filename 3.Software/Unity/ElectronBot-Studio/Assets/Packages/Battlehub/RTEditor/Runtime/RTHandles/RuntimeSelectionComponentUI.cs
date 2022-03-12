using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Battlehub.RTHandles
{
    public class RuntimeSelectionComponentUI : Selectable
    {
        [SerializeField]
        private Image m_background = null;

        public event EventHandler Selected;
        public event EventHandler Unselected;

        private bool m_isSelected;
        public bool IsSelected
        {
            get { return m_isSelected; }
            private set
            {
                if(m_isSelected != value)
                {
                    m_isSelected = value;
                    if(m_isSelected)
                    {
                        if (Selected != null)
                        {
                            Selected(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if(Unselected != null)
                        {
                            Unselected(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if(m_background == null)
            {
                Image image = gameObject.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0);
            }
        }

        
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            IsSelected = true;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            IsSelected = false;
        }
    }
}
