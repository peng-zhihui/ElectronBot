using UnityEngine;

using Battlehub.RTCommon;
namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(2)]
    public class ScaleHandle : BaseHandle
    {
        public bool AbsoluteGrid = false;
        public float GridSize = 0.1f;
        public Vector3 MinScale = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private Vector3 m_prevPoint;
        private Matrix4x4 m_matrix;
        private Matrix4x4 m_inverse;

        private Vector3 m_roundedScale;
        private Vector3 m_scale;
        private Vector3[] m_refScales;
        private float m_screenScale;
 
        public override bool SnapToGrid
        {
            get { return AbsoluteGrid; }
            set { AbsoluteGrid = value; }
        }

        public override float SizeOfGrid
        {
            get { return GridSize; }
            set { GridSize = value; }
        }

        public override RuntimeTool Tool
        {
            get { return RuntimeTool.Scale; }
        }

        protected override float CurrentGridUnitSize
        {
            get { return SizeOfGrid; }
        }

        protected override void Awake()
        {
            base.Awake();
        
            m_scale = Vector3.one;
            m_roundedScale = m_scale;
        }

        protected override bool OnBeginDrag()
        {
            if(!base.OnBeginDrag())
            {
                return false;
            }

            if(SelectedAxis == RuntimeHandleAxis.Free)
            {
                DragPlane = GetDragPlane(Vector3.zero);
            }
            else if(SelectedAxis == RuntimeHandleAxis.None)
            {
                return false;
            }

            m_refScales = new Vector3[ActiveTargets.Length];
            for (int i = 0; i < m_refScales.Length; ++i)
            {
                Quaternion rotation = PivotRotation == RuntimePivotRotation.Global ? ActiveTargets[i].rotation : Quaternion.identity;
                m_refScales[i] = rotation * ActiveTargets[i].localScale;
            }

            Vector3 axis = Vector3.zero;
            switch (SelectedAxis)
            {
                case RuntimeHandleAxis.X:
                    axis = Vector3.right;
                    break;
                case RuntimeHandleAxis.Y:
                    axis = Vector3.up;
                    break;
                case RuntimeHandleAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            DragPlane = GetDragPlane(axis);
            bool result = GetPointOnDragPlane(Window.Pointer, out m_prevPoint);
            if(!result)
            {
                SelectedAxis = RuntimeHandleAxis.None;
            }
            return result;
        }

        protected override void OnDrag()
        {
            base.OnDrag();

            Vector3 point;
            if (GetPointOnDragPlane(Window.Pointer, out point))
            {
                Vector3 offset = m_inverse.MultiplyVector((point - m_prevPoint) / m_screenScale);
                float mag = offset.magnitude;
                if (SelectedAxis == RuntimeHandleAxis.X)
                {
                    offset.y = offset.z = 0.0f;

                    if (LockObject == null || !LockObject.ScaleX)
                    {
                        m_scale.x += Mathf.Sign(offset.x) * mag;
                    }
                }
                else if (SelectedAxis == RuntimeHandleAxis.Y)
                {
                    offset.x = offset.z = 0.0f;
                    if(LockObject == null || !LockObject.ScaleY)
                    {
                        m_scale.y += Mathf.Sign(offset.y) * mag;
                    }
                }
                else if(SelectedAxis == RuntimeHandleAxis.Z)
                {
                    offset.x = offset.y = 0.0f;
                    if(LockObject == null || !LockObject.ScaleZ)
                    {
                        m_scale.z += Mathf.Sign(offset.z) * mag;
                    }
                }
                if(SelectedAxis == RuntimeHandleAxis.Free)
                {
                    float sign = Mathf.Sign(offset.x + offset.y);

                    if(LockObject != null)
                    {
                        if (!LockObject.ScaleX)
                        {
                            m_scale.x += sign * mag;
                        }

                        if (!LockObject.ScaleY)
                        {
                            m_scale.y += sign * mag;
                        }

                        if (!LockObject.ScaleZ)
                        {
                            m_scale.z += sign * mag;
                        }
                    }
                    else
                    {
                        m_scale.x += sign * mag;
                        m_scale.y += sign * mag;
                        m_scale.z += sign * mag;
                    }
                }

                if(SnapToGrid)
                {
                    for (int i = 0; i < m_refScales.Length; ++i)
                    {
                        Quaternion rotation = PivotRotation == RuntimePivotRotation.Global ? Targets[i].rotation : Quaternion.identity;

                        float gridSize = EffectiveGridUnitSize * 2;

                        m_roundedScale = Vector3.Scale(m_refScales[i], m_scale);
                        if (EffectiveGridUnitSize > 0.01)
                        {
                            m_roundedScale.x = Mathf.RoundToInt(m_roundedScale.x / gridSize) * gridSize;
                            m_roundedScale.y = Mathf.RoundToInt(m_roundedScale.y / gridSize) * gridSize;
                            m_roundedScale.z = Mathf.RoundToInt(m_roundedScale.z / gridSize) * gridSize;
                        }

                        Vector3 scale = Quaternion.Inverse(rotation) * m_roundedScale;
                        scale.x = Mathf.Max(MinScale.x, scale.x);
                        scale.y = Mathf.Max(MinScale.y, scale.y);
                        scale.z = Mathf.Max(MinScale.z, scale.z);
                        ActiveTargets[i].localScale = scale;
                    }

                    if (Model != null)
                    {
                        Model.SetScale(m_scale);
                    }
                }
                else
                {
                    m_roundedScale = m_scale;

                    if (EffectiveGridUnitSize > 0.01)
                    {
                        m_roundedScale.x = Mathf.RoundToInt(m_roundedScale.x / EffectiveGridUnitSize) * EffectiveGridUnitSize;
                        m_roundedScale.y = Mathf.RoundToInt(m_roundedScale.y / EffectiveGridUnitSize) * EffectiveGridUnitSize;
                        m_roundedScale.z = Mathf.RoundToInt(m_roundedScale.z / EffectiveGridUnitSize) * EffectiveGridUnitSize;
                    }

                    if (Model != null)
                    {
                        Model.SetScale(m_roundedScale);
                    }

                    for (int i = 0; i < m_refScales.Length; ++i)
                    {
                        Quaternion rotation = PivotRotation == RuntimePivotRotation.Global ? Targets[i].rotation : Quaternion.identity;

                        Vector3 scale = Quaternion.Inverse(rotation) * Vector3.Scale(m_refScales[i], m_roundedScale);
                        
                        scale.x = Mathf.Max(MinScale.x, scale.x);
                        scale.y = Mathf.Max(MinScale.y, scale.y);
                        scale.z = Mathf.Max(MinScale.z, scale.z);
                        ActiveTargets[i].localScale = scale;
                    }
                }
               
                m_prevPoint = point;
            }
        }

        protected override void OnDrop()
        {
            base.OnDrop();

            m_scale = Vector3.one;
            m_roundedScale = m_scale;
            if(Model != null)
            {
                Model.SetScale(m_roundedScale);
            }
        }

        private RTHDrawingSettings m_settings = new RTHDrawingSettings();
        protected override void RefreshCommandBuffer(IRTECamera camera)
        {
            m_settings.Position = Target.position;
            m_settings.Rotation = Rotation;
            m_settings.Scale = m_roundedScale;
            m_settings.SelectedAxis = SelectedAxis;
            m_settings.LockObject = LockObject;

            Appearance.DoScaleHandle(camera.CommandBuffer, camera.Camera, m_settings);
        }
        public override RuntimeHandleAxis HitTest(out float distance)
        {
            m_screenScale = RuntimeHandlesComponent.GetScreenScale(transform.position, Window.Camera) * Appearance.HandleScale;
            m_matrix = Matrix4x4.TRS(transform.position, Rotation, Appearance.InvertZAxis ? new Vector3(1, 1, -1) : Vector3.one);
            m_inverse = m_matrix.inverse;

            if (Model != null)
            {
                return Model.HitTest(Window.Pointer, out distance);
            }

            return Appearance.HitTestScaleHandle(Window.Camera, Window.Pointer, m_settings, out distance);
        }
    }
}