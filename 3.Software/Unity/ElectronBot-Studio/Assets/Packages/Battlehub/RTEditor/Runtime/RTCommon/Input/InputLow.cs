using UnityEngine;

namespace Battlehub.RTCommon
{
    public enum InputAxis
    {
        X,
        Y,
        Z,
        Horizontal,
        Vertical,
    }

    public interface IInput
    {
        bool IsAnyKeyDown();

        float GetAxis(InputAxis axis);
        bool GetKeyDown(KeyCode key);
        bool GetKeyUp(KeyCode key);
        bool GetKey(KeyCode key);

        Vector3 GetPointerXY(int pointer);

        bool GetPointerDown(int button);
        bool GetPointerUp(int button);
        bool GetPointer(int button);
    }

    public class DisabledInput : IInput
    {
        public float GetAxis(InputAxis axis)
        {
            return 0;
        }

        public bool GetKey(KeyCode key)
        {
            return false;
        }

        public bool GetKeyDown(KeyCode key)
        {
            return false;
        }

        public bool GetKeyUp(KeyCode key)
        {
            return false;
        }

        public bool GetPointer(int button)
        {
            return false;
        }

        public bool GetPointerDown(int button)
        {
            return false;
        }

        public bool GetPointerUp(int button)
        {
            return false;
        }

        public Vector3 GetPointerXY(int pointer)
        {
            if (pointer == 0)
            {
                return Input.mousePosition;
            }
            else
            {
                Touch touch = Input.GetTouch(pointer);
                return touch.position;
            }
        }

        public bool IsAnyKeyDown()
        {
            return false;
        }
    }

    public class InputLow : IInput
    {
        public virtual bool IsAnyKeyDown()
        {
            return Input.anyKeyDown;
        }

        public virtual bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public virtual bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public virtual bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }

        public virtual float GetAxis(InputAxis axis)
        {
            switch (axis)
            {
                case InputAxis.X:
                    return Input.GetAxis("Mouse X");
                case InputAxis.Y:
                    return Input.GetAxis("Mouse Y");
                case InputAxis.Z:
                    return Input.GetAxis("Mouse ScrollWheel");
                case InputAxis.Horizontal:
                    return Input.GetAxis("Horizontal");
                case InputAxis.Vertical:
                    return Input.GetAxis("Vertical");
                default:
                    return 0;
            }
        }

        public virtual Vector3 GetPointerXY(int pointer)
        {
            if (pointer == 0)
            {
                return Input.mousePosition;
            }
            else
            {
                Touch touch = Input.GetTouch(pointer);
                return touch.position;
            }
        }

        public virtual bool GetPointerDown(int index)
        {
            bool buttonDown = Input.GetMouseButtonDown(index);
            return buttonDown;
        }

        public virtual bool GetPointerUp(int index)
        {
            return Input.GetMouseButtonUp(index);
        }

        public virtual bool GetPointer(int index)
        {
            return Input.GetMouseButton(index);
        }
    }
}