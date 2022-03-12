using UnityEngine;

using Battlehub.RTCommon;
using UnityEngine.EventSystems;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(3)]
    public class RotationHandle : BaseHandle
    {
        public float GridSize = 15.0f;
        public float XSpeed = 1.0f;
        public float YSpeed = 1.0f;

        private float m_deltaX;
        private float m_deltaY;
        private Vector2 m_prevPointer;
  
        public override RuntimeTool Tool
        {
            get { return RuntimeTool.Rotate; }
        }

        private Quaternion m_targetInverse = Quaternion.identity;
        private Matrix4x4 m_targetInverseMatrix;
        private Vector3 m_startingRotationAxis = Vector3.zero;
        private Quaternion m_targetRotation = Quaternion.identity;
        private Quaternion m_startingRotation = Quaternion.identity;
        private Quaternion StartingRotation
        {
            get { return PivotRotation == RuntimePivotRotation.Global ? m_startingRotation : Quaternion.identity; }
        }

        private Quaternion m_startinRotationInv = Quaternion.identity;
        private Quaternion StartingRotationInv
        {
            get { return PivotRotation == RuntimePivotRotation.Global ? m_startinRotationInv : Quaternion.identity; }
        }

        private Quaternion TargetRotation
        {
            get { return PivotRotation == RuntimePivotRotation.Global ? Target.rotation : ActiveRealTargets[0].rotation; }
        }

        protected override float CurrentGridUnitSize
        {
            get { return GridSize; }
        }

        protected override void Start()
        {
            base.Start();
            OnPivotRotationChanged();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnPivotRotationChanged();
            Editor.Tools.PivotRotationChanged += OnPivotRotationChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Editor.Tools.PivotRotationChanged -= OnPivotRotationChanged;
        }

        protected override void UpdateOverride()
        {
            base.UpdateOverride();

            if (Editor.Tools.IsViewing)
            {
                SelectedAxis = RuntimeHandleAxis.None;
                return;
            }
            if (!IsWindowActive || !Window.IsPointerOver)
            {
                return;
            }
            if (!IsDragging)
            {
                UpdateMatricesAndRotations(false);
            }
        }

        protected override void OnPivotRotationChanged()
        {
            UpdateMatricesAndRotations(true);
            base.OnPivotRotationChanged();
        }

        private void UpdateMatricesAndRotations(bool forceUpdate)
        {
            if (Target == null)
            {
                return;
            }

            if (HightlightOnHover)
            {
                m_targetInverseMatrix = Matrix4x4.TRS(Target.position, TargetRotation * StartingRotationInv, Vector3.one).inverse;
            }

            if (forceUpdate || m_targetRotation != TargetRotation)
            {
                m_startingRotation = TargetRotation;
                m_startinRotationInv = Quaternion.Inverse(m_startingRotation);
                m_targetRotation = TargetRotation;
            }
        }

        private bool ForceScreenRotationMode()
        {
            if (SelectedAxis == RuntimeHandleAxis.Free)
            {
                return false;
            }

            if (SelectedAxis == RuntimeHandleAxis.X)
            {
                return Mathf.Abs(Vector3.Dot(Window.Camera.transform.forward, (Target.rotation * StartingRotationInv) * Vector3.right)) > 0.8;
            }
            else if (SelectedAxis == RuntimeHandleAxis.Y)
            {
                return Mathf.Abs(Vector3.Dot(Window.Camera.transform.forward, (Target.rotation * StartingRotationInv) * Vector3.up)) > 0.8;
            }
            else if (SelectedAxis == RuntimeHandleAxis.Z)
            {
                return Mathf.Abs(Vector3.Dot(Window.Camera.transform.forward, (Target.rotation * StartingRotationInv) * Vector3.forward)) > 0.8;
            }
            return false;
        }

        private Quaternion ScreenRotation(Vector3 delta)
        {
            Vector3 cameraAxis = m_targetInverseMatrix.MultiplyVector(Window.Camera.cameraToWorldMatrix.MultiplyVector(-Vector3.forward));
            if (SelectedAxis == RuntimeHandleAxis.Screen)
            {
                Quaternion rotation = Quaternion.AngleAxis(delta.x, cameraAxis);
                if (LockObject == null || !LockObject.RotationScreen)
                {
                    return rotation;
                }
            }
            else
            {
                if (SelectedAxis == RuntimeHandleAxis.X)
                {
                    Vector3 axis = Quaternion.Inverse(Target.rotation) * ((Target.rotation * StartingRotationInv) * Vector3.right);
                    Quaternion rotation = Quaternion.AngleAxis(delta.x * Mathf.Sign(Vector3.Dot(axis, cameraAxis)), axis);
                    if (LockObject == null || !LockObject.RotationX)
                    {
                        return rotation;
                    }
                }
                else if (SelectedAxis == RuntimeHandleAxis.Y)
                {
                    Vector3 axis = Quaternion.Inverse(Target.rotation) * ((Target.rotation * StartingRotationInv) * Vector3.up);
                    Quaternion rotation = Quaternion.AngleAxis(delta.x * Mathf.Sign(Vector3.Dot(axis, cameraAxis)), axis);
                    if (LockObject == null || !LockObject.RotationY)
                    {
                        return rotation;
                    }
                }
                else if (SelectedAxis == RuntimeHandleAxis.Z)
                {
                    Vector3 axis = Quaternion.Inverse(Target.rotation) * ((Target.rotation * StartingRotationInv) * Vector3.forward);
                    Quaternion rotation = Quaternion.AngleAxis(delta.x * Mathf.Sign(Vector3.Dot(axis, cameraAxis)), axis);
                    if (LockObject == null || !LockObject.RotationZ)
                    {
                        return rotation;
                    }
                }
            }

            return Quaternion.identity;
        }

        private bool m_forceScreenRotationMode;
        protected override bool OnBeginDrag()
        {
            if(Target == null)
            {
                return false;
            }

            m_targetRotation = Target.rotation;
            m_targetInverseMatrix = Matrix4x4.TRS(Target.position, Target.rotation * StartingRotationInv, Vector3.one).inverse;

            if (!base.OnBeginDrag())
            {
                return false;
            }

            m_deltaX = 0.0f;
            m_deltaY = 0.0f;

            Vector2 point;
            if (Window.Pointer.XY(Target.position, out point))
            {
                m_prevPointer = point;
            }
            else
            {
                SelectedAxis = RuntimeHandleAxis.None;
            }

            m_forceScreenRotationMode = ForceScreenRotationMode();
            if (SelectedAxis == RuntimeHandleAxis.Screen || m_forceScreenRotationMode)
            {
                Vector2 center;

                if (Window.Pointer.WorldToScreenPoint(Target.position, Target.position, out center))
                {
                    if (Window.Pointer.XY(Target.position, out point))
                    {
                        float angle = Mathf.Atan2(point.y - center.y, point.x - center.x);
                        m_targetInverse = Quaternion.Inverse(Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.forward));
                        m_targetInverseMatrix = Matrix4x4.TRS(Target.position, Target.rotation, Vector3.one).inverse;
                        m_prevPointer = point;
                    }
                    else
                    {
                        SelectedAxis = RuntimeHandleAxis.None;
                    }
                }
                else
                {
                    SelectedAxis = RuntimeHandleAxis.None;
                }
            }
            else
            {
                if (SelectedAxis == RuntimeHandleAxis.X)
                {
                    m_startingRotationAxis = (Target.rotation * Quaternion.Inverse(StartingRotation)) * Vector3.right;
                }
                else if (SelectedAxis == RuntimeHandleAxis.Y)
                {
                    m_startingRotationAxis = (Target.rotation * Quaternion.Inverse(StartingRotation)) * Vector3.up;
                }
                else if (SelectedAxis == RuntimeHandleAxis.Z)
                {
                    m_startingRotationAxis = (Target.rotation * Quaternion.Inverse(StartingRotation)) * Vector3.forward;
                }

                m_targetInverse = Quaternion.Inverse(Target.rotation);
            }

            return SelectedAxis != RuntimeHandleAxis.None;
        }

        protected override void OnDrag()
        {
            base.OnDrag();

            Vector2 point;
            if (!Window.Pointer.XY(Target.position, out point))
            {
                return;
            }

            float deltaX = point.x - m_prevPointer.x;
            float deltaY = point.y - m_prevPointer.y;
            m_prevPointer = point;

            deltaX = deltaX * XSpeed;
            deltaY = deltaY * YSpeed;

            m_deltaX += deltaX;
            m_deltaY += deltaY;

            Matrix4x4 toWorldMatrix;
            if (!Window.Pointer.ToWorldMatrix(Target.position, out toWorldMatrix))
            {
                return;
            }

            Vector3 delta = StartingRotation * Quaternion.Inverse(Target.rotation) * toWorldMatrix.MultiplyVector(new Vector3(m_deltaY, -m_deltaX, 0));
            Quaternion rotation = Quaternion.identity;

            if (SelectedAxis == RuntimeHandleAxis.Screen || m_forceScreenRotationMode)
            {
                delta = m_targetInverse * new Vector3(m_deltaY, -m_deltaX, 0);
                if (EffectiveGridUnitSize != 0.0f)
                {
                    if (Mathf.Abs(delta.x) >= EffectiveGridUnitSize)
                    {
                        delta.x = Mathf.Sign(delta.x) * EffectiveGridUnitSize;
                        m_deltaX = 0.0f;
                        m_deltaY = 0.0f;
                    }
                    else
                    {
                        delta.x = 0.0f;
                    }
                }

                rotation = ScreenRotation(delta);
            }

            else if (SelectedAxis == RuntimeHandleAxis.X)
            {
                Vector3 rotationAxis = Quaternion.Inverse(Target.rotation) * m_startingRotationAxis;

                if (EffectiveGridUnitSize != 0.0f)
                {
                    if (Mathf.Abs(delta.x) >= EffectiveGridUnitSize)
                    {
                        delta.x = Mathf.Sign(delta.x) * EffectiveGridUnitSize;
                        m_deltaX = 0.0f;
                        m_deltaY = 0.0f;
                    }
                    else
                    {
                        delta.x = 0.0f;
                    }
                }

                if (LockObject != null && LockObject.RotationX)
                {
                    delta.x = 0.0f;
                }

                rotation = Quaternion.AngleAxis(delta.x, rotationAxis);
            }
            else if (SelectedAxis == RuntimeHandleAxis.Y)
            {
                Vector3 rotationAxis = Quaternion.Inverse(Target.rotation) * m_startingRotationAxis;

                if (EffectiveGridUnitSize != 0.0f)
                {
                    if (Mathf.Abs(delta.y) >= EffectiveGridUnitSize)
                    {
                        delta.y = Mathf.Sign(delta.y) * EffectiveGridUnitSize;
                        m_deltaX = 0.0f;
                        m_deltaY = 0.0f;
                    }
                    else
                    {
                        delta.y = 0.0f;
                    }
                }

                if (LockObject != null && LockObject.RotationY)
                {
                    delta.y = 0.0f;
                }

                rotation = Quaternion.AngleAxis(delta.y, rotationAxis);

            }
            else if (SelectedAxis == RuntimeHandleAxis.Z)
            {
                Vector3 rotationAxis = Quaternion.Inverse(Target.rotation) * m_startingRotationAxis;

                if (EffectiveGridUnitSize != 0.0f)
                {
                    if (Mathf.Abs(delta.z) >= EffectiveGridUnitSize)
                    {
                        delta.z = Mathf.Sign(delta.z) * EffectiveGridUnitSize;
                        m_deltaX = 0.0f;
                        m_deltaY = 0.0f;
                    }
                    else
                    {
                        delta.z = 0.0f;
                    }
                }

                if (LockObject != null && LockObject.RotationZ)
                {
                    delta.z = 0.0f;
                }

                rotation = Quaternion.AngleAxis(delta.z, rotationAxis);

            }
            else
            {
                delta = StartingRotationInv * delta;

                if (LockObject != null && LockObject.RotationFree)
                {
                    delta.x = 0.0f;
                    delta.y = 0.0f;
                    delta.z = 0.0f;
                }

                rotation = Quaternion.Euler(delta.x, delta.y, delta.z);
                m_deltaX = 0.0f;
                m_deltaY = 0.0f;
            }
           

            if (EffectiveGridUnitSize == 0.0f)
            {
                m_deltaX = 0.0f;
                m_deltaY = 0.0f;
            }


            for (int i = 0; i < ActiveTargets.Length; ++i)
            {
                ActiveTargets[i].rotation *= rotation;
            }
        }

        protected override void OnDrop()
        {
            base.OnDrop();
            m_targetRotation = Target.rotation;
            OnPivotRotationChanged();
        }

        protected override void SyncModelTransform()
        {
            base.SyncModelTransform();
            if (Target != null)
            {
                Model.transform.rotation = Target.rotation * StartingRotationInv;
            }
        }

        private RTHDrawingSettings m_settings = new RTHDrawingSettings();
        protected override void RefreshCommandBuffer(IRTECamera camera)
        {
            m_settings.Position = Target.position;
            m_settings.Rotation = Target.rotation * StartingRotationInv;
            m_settings.SelectedAxis = SelectedAxis;
            m_settings.LockObject = LockObject;

            Appearance.DoRotationHandle(camera.CommandBuffer, camera.Camera, m_settings, Editor.IsVR);
        }

        public override RuntimeHandleAxis HitTest(out float distance)
        {
            if (Model != null)
            {
                return Model.HitTest(Window.Pointer, out distance);
            }
            return Appearance.HitTestRotationHandle(Window.Camera, Window.Pointer, m_settings, out distance);
        }
    }
}
