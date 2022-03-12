using UnityEngine;

namespace Battlehub.Utils
{
    public static class TransformExtensions 
    {
        public static Bounds CalculateBounds(this Transform t)
        {
            Renderer renderer = t.GetComponentInChildren<Renderer>(false);
            if (renderer)
            {
                Bounds bounds = renderer.bounds;
                if (bounds.size == Vector3.zero && bounds.center != renderer.transform.position)
                {
                    bounds = TransformBounds(renderer.transform.localToWorldMatrix, bounds);
                }
                CalculateBounds(t, ref bounds);
                if (bounds.extents == Vector3.zero)
                {
                    bounds.extents = new Vector3(0.5f, 0.5f, 0.5f);
                }
                return bounds;
            }

            return new Bounds(t.position, new Vector3(0.5f, 0.5f, 0.5f));
        }

        private static void CalculateBounds(Transform t, ref Bounds totalBounds)
        {
            foreach (Transform child in t)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer)
                {
                    Bounds bounds = renderer.bounds;
                    if (bounds.size == Vector3.zero && bounds.center != renderer.transform.position)
                    {
                        bounds = TransformBounds(renderer.transform.localToWorldMatrix, bounds);
                    }
                    totalBounds.Encapsulate(bounds.min);
                    totalBounds.Encapsulate(bounds.max);
                }

                CalculateBounds(child, ref totalBounds);
            }
        }
        public static Bounds TransformBounds(Matrix4x4 matrix, Bounds bounds)
        {
            var center = matrix.MultiplyPoint(bounds.center);

            // transform the local extents' axes
            var extents = bounds.extents;
            var axisX = matrix.MultiplyVector(new Vector3(extents.x, 0, 0));
            var axisY = matrix.MultiplyVector(new Vector3(0, extents.y, 0));
            var axisZ = matrix.MultiplyVector(new Vector3(0, 0, extents.z));

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds { center = center, extents = extents };
        }


        private static Vector3[] s_Corners = new Vector3[4];
        public static Bounds CalculateRelativeRectTransformBounds(Transform root, Transform child)
        {
            RectTransform component = child as RectTransform;
            if (component == null)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            Vector3 v1 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 v2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;

            component.GetWorldCorners(s_Corners);
            for (int i = 0; i < 4; ++i)
            {
                Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[i]);
                v1 = Vector3.Min(lhs, v1);
                v2 = Vector3.Max(lhs, v2);
            }

            Bounds bounds = new Bounds(v1, Vector3.zero);
            bounds.Encapsulate(v2);
            return bounds;
        }

        public static Bounds CalculateRelativeRectTransformBounds(this Transform trans)
        {
            return CalculateRelativeRectTransformBounds(trans, trans);
        }
    }
}
