using UnityEngine;
using System.Linq;

namespace Battlehub.RTCommon
{
    public class LockObject
    {
        private bool m_positionX;
        private bool m_positionY;
        private bool m_positionZ;
        private bool m_rotationX;
        private bool m_rotationY;
        private bool m_rotationZ;
        private bool m_rotationFree;
        private bool m_rotationScreen;
        private bool m_scaleX;
        private bool m_scaleY;
        private bool m_scaleZ;

        public bool PositionX { get { return m_positionX || (m_globalLock != null ? m_globalLock.m_positionX : false); } set { m_positionX = value; } }
        public bool PositionY { get { return m_positionY || (m_globalLock != null ? m_globalLock.m_positionY : false); } set { m_positionY = value; } }
        public bool PositionZ { get { return m_positionZ || (m_globalLock != null ? m_globalLock.m_positionZ : false); } set { m_positionZ = value; } }
        public bool RotationX { get { return m_rotationX || (m_globalLock != null ? m_globalLock.m_rotationX : false); } set { m_rotationX = value; } }
        public bool RotationY { get { return m_rotationY || (m_globalLock != null ? m_globalLock.m_rotationY : false); } set { m_rotationY = value; } }
        public bool RotationZ { get { return m_rotationZ || (m_globalLock != null ? m_globalLock.m_rotationZ : false); } set { m_rotationZ = value; } }
        public bool RotationFree { get { return m_rotationFree || (m_globalLock != null ? m_globalLock.m_rotationFree : false); } set { m_rotationFree = value; } }
        public bool RotationScreen { get { return m_rotationScreen || (m_globalLock != null ? m_globalLock.m_rotationScreen : false); } set { m_rotationScreen = value; } }
        public bool ScaleX { get { return m_scaleX || (m_globalLock != null ? m_globalLock.m_scaleX : false); } set { m_scaleX = value; } }
        public bool ScaleY { get { return m_scaleY || (m_globalLock != null ? m_globalLock.m_scaleY : false); } set { m_scaleY = value; } }
        public bool ScaleZ { get { return m_scaleZ || (m_globalLock != null ? m_globalLock.m_scaleZ : false); } set { m_scaleZ = value; } }

        public RuntimePivotMode? PivotMode { get; set; }
        public RuntimePivotRotation? PivotRotation { get; set; }

        public bool IsPositionLocked
        {
            get { return PositionX && PositionY && PositionZ; }
        }

        public bool IsRotationLocked
        {
            get { return RotationX && RotationY && RotationZ && RotationFree && RotationScreen; }
        }

        public bool IsScaleLocked
        {
            get { return ScaleX && ScaleY && ScaleZ; }
        }

        private LockObject m_globalLock;
        public void SetGlobalLock(LockObject gLock)
        {
            if(gLock == null)
            {
                m_globalLock = new LockObject();
            }
            else
            {
                m_globalLock = gLock;
            }
        }
    }

    public class LockAxes : MonoBehaviour
    {
        public bool PositionX;
        public bool PositionY;
        public bool PositionZ;
        public bool RotationX;
        public bool RotationY;
        public bool RotationZ;
        public bool RotationFree;
        public bool RotationScreen;
        public bool ScaleX;
        public bool ScaleY;
        public bool ScaleZ;

        public bool PivotMode;
        public RuntimePivotMode PivotModeValue;
        public bool PivotRotation;
        public RuntimePivotRotation PivotRotationValue;
        

        public static LockObject Eval(LockAxes[] lockAxes)
        {
            LockObject lockObject = new LockObject();
            if(lockAxes != null)
            {
                lockObject.PositionX = lockAxes.Any(la => la.PositionX);
                lockObject.PositionY = lockAxes.Any(la => la.PositionY);
                lockObject.PositionZ = lockAxes.Any(la => la.PositionZ);

                lockObject.RotationX = lockAxes.Any(la => la.RotationX);
                lockObject.RotationY = lockAxes.Any(la => la.RotationY);
                lockObject.RotationZ = lockAxes.Any(la => la.RotationZ);
                lockObject.RotationFree = lockAxes.Any(la => la.RotationFree);
                lockObject.RotationScreen = lockAxes.Any(la => la.RotationScreen);

                lockObject.ScaleX = lockAxes.Any(la => la.ScaleX);
                lockObject.ScaleY = lockAxes.Any(la => la.ScaleY);
                lockObject.ScaleZ = lockAxes.Any(la => la.ScaleZ);

                lockObject.PivotMode = null;
                if(lockAxes.Any(la => la.PivotMode))
                {
                    if(lockAxes.All(la => la.PivotModeValue == RuntimePivotMode.Center))
                    {
                        lockObject.PivotMode = RuntimePivotMode.Center;
                    }
                    else if(lockAxes.All(la => la.PivotModeValue == RuntimePivotMode.Pivot))
                    {
                        lockObject.PivotMode = RuntimePivotMode.Pivot;
                    }
                }

                lockObject.PivotRotation = null;
                if(lockAxes.Any(la => la.PivotRotation))
                {
                    if (lockAxes.All(la => la.PivotRotationValue == RuntimePivotRotation.Global))
                    {
                        lockObject.PivotRotation = RuntimePivotRotation.Global;
                    }
                    else if (lockAxes.All(la => la.PivotRotationValue == RuntimePivotRotation.Local))
                    {
                        lockObject.PivotRotation = RuntimePivotRotation.Local;
                    }
                }
            }

            return lockObject;
        }
    }

}
