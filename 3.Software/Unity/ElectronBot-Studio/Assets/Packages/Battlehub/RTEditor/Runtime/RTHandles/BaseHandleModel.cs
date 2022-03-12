using Battlehub.RTCommon;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(5)]
    public class BaseHandleModel : RTEComponent
    {
        private RuntimeHandlesComponent m_appearance;
        public RuntimeHandlesComponent Appearance
        {
            get { return m_appearance; }
            set { m_appearance = value; }
        }

        public RTHColors Colors
        {
            get { return m_appearance.Colors; }
        }

        private float m_modelScale = 1.0f;
        public float ModelScale
        {
            get { return m_modelScale;  }
            set
            {
                if(m_modelScale != value)
                {
                    m_modelScale = value;
                    if(enabled && gameObject.activeSelf)
                    {
                        UpdateModel();
                    }
                }   
            }
        }

        private float m_selectionMargin = 1.0f;
        public float SelectionMargin
        {
            get { return m_selectionMargin; }
            set
            {
                if(m_selectionMargin != value)
                {
                    m_selectionMargin = value;
                    if(enabled && gameObject.activeSelf)
                    {
                        UpdateModel();
                    }
                }
            }
        }

        protected RuntimeHandleAxis m_selectedAxis = RuntimeHandleAxis.None;
        protected LockObject m_lockObj = new LockObject();

        protected override void Awake()
        {
            base.Awake();
            SetLayer(transform, Window.Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index);
        }
    

        protected virtual void OnEnable()
        {
            UpdateModel();
        }

        protected virtual void OnDisable()
        {
            IRTECamera rteCamera = GetRTECamera();

            if (rteCamera != null)
            {
                rteCamera.RenderersCache.Remove(GetRenderers());
                rteCamera.RenderersCache.Refresh();
            }
        }

        protected virtual void Update()
        {
            
        }

        public virtual void UpdateModel()
        {
            PushUpdatesToGraphicLayer();
        }

        private IRTECamera GetRTECamera()
        {
            if(Window == null || Window.Camera == null)
            {
                return null;
            }

            IRTECamera rteCamera;
            IRTEGraphicsLayer graphicsLayer = Window.IOCContainer.Resolve<IRTEGraphicsLayer>();
            if (graphicsLayer != null)
            {
                rteCamera = graphicsLayer.Camera;
            }
            else
            {
                IRTEGraphics graphics = IOC.Resolve<IRTEGraphics>();
                rteCamera = graphics.GetOrCreateCamera(Window.Camera, CameraEvent.AfterImageEffectsOpaque);
            }

            return rteCamera;
        }

        public void PushUpdatesToGraphicLayer()
        {
            IRTECamera rteCamera = GetRTECamera();

            if (rteCamera != null && gameObject.activeInHierarchy && rteCamera.RenderersCache != null)
            {
                Renderer[] renderers = GetRenderers();
                rteCamera.RenderersCache.Remove(renderers);
                rteCamera.RenderersCache.Add(renderers, false, true);
                rteCamera.RenderersCache.Refresh();
            }
        }

        protected virtual Renderer[] GetRenderers()
        {
            return gameObject.GetComponentsInChildren<Renderer>(true);
        }

        private void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                SetLayer(child, layer);
            }
        }

        public virtual void SetLock(LockObject lockObj)
        {
            if (lockObj == null)
            {
                lockObj = new LockObject();
            }
            m_lockObj = lockObj;
        }

        public virtual void Select(RuntimeHandleAxis axis)
        {
            m_selectedAxis = axis;
        }

        public virtual void SetScale(Vector3 scale)
        {

        }

        public virtual RuntimeHandleAxis HitTest(Ray ray, out float distance)
        {
            distance = float.PositiveInfinity;
            return RuntimeHandleAxis.None;
        }

    }
}
