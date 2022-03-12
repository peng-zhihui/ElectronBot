using UnityEngine;

namespace Battlehub.RTCommon
{
    [DefaultExecutionOrder(-60)]
    public class MouseOrbit : MonoBehaviour
    {
        protected Camera m_camera;
        public Transform Target;
        public Transform SecondaryTarget;
        public float Distance = 5.0f;
        public float XSpeed = 5.0f;
        public float YSpeed = 5.0f;

        public float DistanceMin = 0.5f;
        public float DistanceMax = 5000f;

        public bool CanOrbit;
        public bool CanZoom;
        public bool ChangeOrthographicSizeOnly;

        private void Awake()
        {
            m_camera = GetComponent<Camera>();
        }

        private void Start()
        {
            if(Target != null && m_camera != null)
            {
                Distance = (Target.transform.position - m_camera.transform.position).magnitude;
            }
        }

        public virtual void Zoom(float deltaZ)
        {
            if(!CanZoom)
            {
                deltaZ = 0;
            }

            if (m_camera.orthographic)
            {
                m_camera.orthographicSize -= deltaZ * m_camera.orthographicSize;
                if (m_camera.orthographicSize < 0.01f)
                {
                    m_camera.orthographicSize = 0.01f;
                }

                if(ChangeOrthographicSizeOnly)
                {
                    return;
                }
            }

            Distance = Mathf.Clamp(Distance - deltaZ * Mathf.Max(1.0f, Distance), DistanceMin, DistanceMax);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
            Vector3 position = transform.rotation * negDistance + Target.position;
            transform.position = position;
        }

        public virtual void Orbit(float deltaX, float deltaY, float deltaZ)
        {
            if(!CanOrbit)
            {
                deltaX = 0;
                deltaY = 0;
            }

            if(m_camera == null)
            {
                return;
            }

            if(deltaX == 0 && deltaY == 0 && deltaZ == 0)
            {
                return;
            }

            deltaX = deltaX * XSpeed;
            deltaY = deltaY * YSpeed;

            Quaternion targetRotation = Quaternion.Inverse(
                Quaternion.Euler(deltaY, 0, 0) *
                Quaternion.Inverse(transform.rotation) *
                Quaternion.Euler(0, -deltaX, 0));

            transform.rotation = targetRotation;

            Zoom(deltaZ);
        }
    }
}
