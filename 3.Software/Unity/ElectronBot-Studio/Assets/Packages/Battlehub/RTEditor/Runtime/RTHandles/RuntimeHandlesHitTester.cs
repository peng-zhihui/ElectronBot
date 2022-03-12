using Battlehub.RTCommon;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(-90)]
    public class RuntimeHandlesHitTester : MonoBehaviour
    {
        protected readonly List<BaseHandle> m_handles = new List<BaseHandle>();

        protected BaseHandle m_selectedHandle;
        protected RuntimeHandleAxis m_selectedAxis;
        protected RuntimeWindow m_window;
        protected IRTE m_editor;

        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            m_editor.ActiveWindowChanged += OnActiveWindowChanged;

            m_window = GetComponent<RuntimeWindow>();
            if(m_window == null)
            {
                Debug.LogError("Unable to find window");
            }
        }

        private void OnDestroy()
        {
            if(m_editor != null)
            {
                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
            }
        }

        public static void InitializeIfRequired(RuntimeWindow window, ref RuntimeHandlesHitTester hitTester)
        {
            hitTester = window.GetComponent<RuntimeHandlesHitTester>();
            if(!hitTester)
            {
                hitTester = window.gameObject.AddComponent<RuntimeHandlesHitTester>();
            }
        }

        private void OnActiveWindowChanged(RuntimeWindow deactivated)
        {
            if(m_window == m_editor.ActiveWindow)
            {
                HitTestAll();
            }
        }

        public virtual void Add(BaseHandle handle)
        {
            if(!m_handles.Contains(handle))
            {
                m_handles.Add(handle);
            }
        }

        public virtual void Remove(BaseHandle handle)
        {
            m_handles.Remove(handle);
        }

        public virtual RuntimeHandleAxis GetSelectedAxis(BaseHandle handle)
        {
            if(m_selectedHandle == null)
            {
                return RuntimeHandleAxis.None;
            }

            if(m_selectedHandle != handle)
            {
                return RuntimeHandleAxis.None;
            }

            return m_selectedAxis;
            
        }

        protected virtual void Update()
        {
            HitTestAll();
        }

        private void HitTestAll()
        {
            m_selectedHandle = null;
            m_selectedAxis = RuntimeHandleAxis.None;

            float minDistance = float.PositiveInfinity;
            for (int i = 0; i < m_handles.Count; ++i)
            {
                BaseHandle handle = m_handles[i];

                float distance;
                RuntimeHandleAxis selectedAxis = handle.HitTest(out distance);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    m_selectedAxis = selectedAxis;
                    m_selectedHandle = handle;
                }
            }
        }
    }
}


