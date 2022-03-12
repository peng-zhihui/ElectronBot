using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    public interface IUIRaycaster
    {   
        Camera eventCamera
        {
            get;
        }
        void Raycast(List<RaycastResult> results);
        void Raycast(PointerEventData eventData, List<RaycastResult> results);
    }

    public class RTEUIRaycaster : MonoBehaviour, IUIRaycaster
    {
        public Camera eventCamera
        {
            get { return m_raycaster.eventCamera; }
        }

        [SerializeField]
        private BaseRaycaster m_raycaster = null;
        private IInput m_input;
        private IRTE m_editor;
     
        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            m_input = m_editor.Input;
            if(m_raycaster == null)
            {
                m_raycaster = gameObject.GetComponent<BaseRaycaster>();
                if(m_raycaster == null)
                {
                    GraphicRaycaster raycaster = gameObject.AddComponent<GraphicRaycaster>();
                    raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
                    //raycaster.mask => ?
                    
                    m_raycaster = raycaster;
                }
            }
        }

        public void Raycast(List<RaycastResult> results)
        {
            PointerEventData eventData = new PointerEventData(m_editor.EventSystem);
            eventData.position = m_input.GetPointerXY(0);
            m_raycaster.Raycast(eventData, results);
        }

        public void Raycast(PointerEventData eventData, List<RaycastResult> results)
        {
            eventData.position = m_input.GetPointerXY(0);
            m_raycaster.Raycast(eventData, results);
        }
    }
}
