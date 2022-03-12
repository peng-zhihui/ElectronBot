using System;
using UnityEngine;

namespace Battlehub.RTCommon
{
    public class RTEComponent : MonoBehaviour
    {
        private IRTE m_editor;
        public IRTE Editor
        {
            get { return m_editor; }
        }

        [SerializeField]
        private RuntimeWindow m_window;
        public virtual RuntimeWindow Window
        {
            get { return m_window; }
            set
            {
                if(m_window != value)
                {
                    if (m_isStarted)
                    {
                        throw new System.NotSupportedException("window change is not supported");
                    }

                    m_editor = IOC.Resolve<IRTE>();
                    m_window = value;
                }
            }
        }

        public bool IsWindowActive
        {
            get { return Window == m_editor.ActiveWindow; }
        }

        private bool m_isStarted;
        protected bool IsStarted
        {
            get { return m_isStarted; }
        }

        protected virtual void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();

            if(Window == null)
            {
                Window = GetDefaultWindow();
                if (Window == null)
                {
                    Debug.LogError("m_window == null");
                    enabled = false;
                    return;
                }
            }
#pragma warning disable CS0618
            AwakeOverride();
#pragma warning restore CS0618
        }

        protected virtual void Start()
        {
            if (IsWindowActive)
            {
                OnWindowActivating();
                OnWindowActivated();
            }
            m_editor.ActiveWindowChanging += OnActiveWindowChanging;
            m_editor.ActiveWindowChanged += OnActiveWindowChanged;
            m_isStarted = true;
        }

        protected virtual RuntimeWindow GetDefaultWindow()
        {
           return m_editor.GetWindow(RuntimeWindowType.Scene);
        }

        protected virtual void OnDestroy()
        {
            if(m_editor != null)
            {
                m_editor.ActiveWindowChanging -= OnActiveWindowChanging;
                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
            }

#pragma warning disable CS0618
            OnDestroyOverride();
#pragma warning restore CS0618
        }

        protected virtual void OnActiveWindowChanging(RuntimeWindow activatedWindow)
        {
            if(activatedWindow == Window)
            {
                OnWindowActivating();
            }
            else
            {
                OnWindowDeactivating();
            }
        }

        protected virtual void OnActiveWindowChanged(RuntimeWindow deactivatedWindow)
        {
            if (m_editor.ActiveWindow == Window)
            {
                OnWindowActivated();
            }
            else
            {
                OnWindowDeactivated();
            }
        }

        protected virtual void OnWindowActivating()
        {

        }

        protected virtual void OnWindowDeactivating()
        {

        }

        protected virtual void OnWindowActivated()
        {
            
        }

        protected virtual void OnWindowDeactivated()
        {

        }


        [Obsolete("Override Awake method instead")]
        protected virtual void AwakeOverride()
        {

        }

        [Obsolete("Override OnDestroy method instead")]
        protected virtual void OnDestroyOverride()
        {

        }

    }
}

