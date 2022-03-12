using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.Utils
{
    public static class MathHelper
    {
        public static float CountOfDigits(float number)
        {
            return (number == 0) ? 1.0f : Mathf.Ceil(Mathf.Log10(Mathf.Abs(number) + 0.5f));
        }

        public static bool Approximately(Vector3 a, Vector3 b, float epsilonSq = 0.01f * 0.01f)
        {
            return Vector3.SqrMagnitude(a - b) <= epsilonSq;
        }

        public static bool Approximately(Quaternion a, Quaternion b, float range = 1 - 0.99999998f)
        {
            return Quaternion.Dot(a, b) >= 1f - range;
        }

        public static bool RayIntersectsTriangle(Ray inRay, Vector3 inTriA, Vector3 inTriB, Vector3 inTriC, out float outDistance, out Vector3 outPoint)
        {
            outDistance = 0f;
            outPoint = Vector3.zero;

            //Find vectors for two edges sharing V1
            Vector3 e1 = inTriB - inTriA;
            Vector3 e2 = inTriC - inTriA;

            //Begin calculating determinant - also used to calculate `u` parameter
            Vector3 P = Vector3.Cross(inRay.direction, e2);

            //if determinant is near zero, ray lies in plane of triangle
            float det = Vector3.Dot(e1, P);

            if (det > -Mathf.Epsilon && det < Mathf.Epsilon)
            {
                return false;
            }

            float inv_det = 1f / det;

            //calculate distance from V1 to ray origin
            Vector3 T = inRay.origin - inTriA;

            // Calculate u parameter and test bound
            float u = Vector3.Dot(T, P) * inv_det;

            //The intersection lies outside of the triangle
            if (u < 0f || u > 1f)
            {
                return false;
            }

            //Prepare to test v parameter
            Vector3 Q = Vector3.Cross(T, e1);

            //Calculate V parameter and test bound
            float v = Vector3.Dot(inRay.direction, Q) * inv_det;

            //The intersection lies outside of the triangle
            if (v < 0f || u + v > 1f)
            {
                return false;
            }

            float t = Vector3.Dot(e2, Q) * inv_det;

            if (t > Mathf.Epsilon)
            {
                //ray intersection
                outDistance = t;

                outPoint.x = (u * inTriB.x + v * inTriC.x + (1 - (u + v)) * inTriA.x);
                outPoint.y = (u * inTriB.y + v * inTriC.y + (1 - (u + v)) * inTriA.y);
                outPoint.z = (u * inTriB.z + v * inTriC.z + (1 - (u + v)) * inTriA.z);

                return true;
            }

            return false;
        }

        private static Transform tempChild = null;
        private static Transform tempParent = null;

        private static Vector3[] positionRegister;
        private static float[] posTimeRegister;
        private static int positionSamplesTaken = 0;

        private static Quaternion[] rotationRegister;
        private static float[] rotTimeRegister;
        private static int rotationSamplesTaken = 0;

        public static void Init()
        {

            tempChild = (new GameObject("Math3d_TempChild")).transform;
            tempParent = (new GameObject("Math3d_TempParent")).transform;

            tempChild.gameObject.hideFlags = HideFlags.HideAndDontSave;
            MonoBehaviour.DontDestroyOnLoad(tempChild.gameObject);

            tempParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
            MonoBehaviour.DontDestroyOnLoad(tempParent.gameObject);

            //set the parent
            tempChild.parent = tempParent;
        }

        //Get a point on a Catmull-Rom spline.
        //The percentage is in range 0 to 1, which starts at the second control point and ends at the second last control point. 
        //The array cPoints should contain all control points. The minimum amount of control points should be 4. 
        //Source: https://forum.unity.com/threads/waypoints-and-constant-variable-speed-problems.32954/#post-213942
        public static Vector2 GetPointOnSpline(float percentage, Vector2[] cPoints)
        {

            //Minimum size is 4
            if (cPoints.Length >= 4)
            {

                //Convert the input range (0 to 1) to range (0 to numSections)
                int numSections = cPoints.Length - 3;
                int curPoint = Mathf.Min(Mathf.FloorToInt(percentage * (float)numSections), numSections - 1);
                float t = percentage * (float)numSections - (float)curPoint;

                //Get the 4 control points around the location to be sampled.
                Vector2 p0 = cPoints[curPoint];
                Vector2 p1 = cPoints[curPoint + 1];
                Vector2 p2 = cPoints[curPoint + 2];
                Vector2 p3 = cPoints[curPoint + 3];

                //The Catmull-Rom spline can be written as:
                // 0.5 * (2*P1 + (-P0 + P2) * t + (2*P0 - 5*P1 + 4*P2 - P3) * t^2 + (-P0 + 3*P1 - 3*P2 + P3) * t^3)
                //Variables P0 to P3 are the control points.
                //Variable t is the position on the spline, with a range of 0 to numSections.
                //C# way of writing the function. Note that f means float (to force precision).
                Vector2 result = .5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) + (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t));

                return new Vector2(result.x, result.y);
            }

            else
            {

                return new Vector2(0, 0);
            }
        }

        //Finds the intersection points between a straight line and a spline. Solves a Cubic polynomial equation
        //The output is in the form of a percentage along the length of the spline (range 0 to 1).
        //The linePoints array should contain two points which form a straight line.
        //The cPoints array should contain all the control points of the spline.
        //Use case: create a gauge with a non-linear scale by defining an array with needle angles vs the number it should point at. The array creates a spline.
        //Driving the needle with a float in range 0 to 1 gives an unpredictable result. Instead, use the GetLineSplineIntersections() function to find the angle the
        //gauge needle should have for a given number it should point at. In this case, cPoints should contain x for angle and y for scale number.
        //Make a horizontal line at the given scale number (y) you want to find the needle angle for. The returned float is a percentage location on the spline (range 0 to 1). 
        //Plug this value into the GetPointOnSpline() function to get the x coordinate which represents the needle angle.
        //Source: https://medium.com/@csaba.apagyi/finding-catmull-rom-spline-and-line-intersection-part-2-mathematical-approach-dfb969019746
        public static float[] GetLineSplineIntersections(Vector2[] linePoints, Vector2[] cPoints)
        {

            List<float> list = new List<float>();
            float[] crossings;

            int numSections = cPoints.Length - 3;

            //The line spline intersection can only be calculated for one segment of a spline, meaning 4 control points,
            //with a spline segment between the middle two control points. So check all spline segments.
            for (int i = 0; i < numSections; i++)
            {

                //Get the 4 control points around the location to be sampled.
                Vector2 p0 = cPoints[i];
                Vector2 p1 = cPoints[i + 1];
                Vector2 p2 = cPoints[i + 2];
                Vector2 p3 = cPoints[i + 3];

                //The Catmull-Rom spline can be written as:
                // 0.5 * (2P1 + (-P0 + P2) * t + (2P0 - 5P1 + 4P2 - P3) * t^2 + (-P0 + 3P1 - 3P2 + P3) * t^3)
                //Variables P0 to P3 are the control points.
                //Notation: 2P1 means 2*controlPoint1
                //Variable t is the position on the spline, converted from a range of 0 to 1.
                //C# way of writing the function is below. Note that f means float (to force precision).
                //Vector2 result = .5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) + (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t));

                //The variable t is the only unknown, so the rest can be substituted:
                //a = 0.5 * (-p0 + 3*p1 - 3*p2 + p3)
                //b = 0.5 * (2*p0 - 5*p1 + 4*p2 - p3) 
                //c = 0.5 * (-p0 + p2)
                //d = 0.5 * (2*p1)

                //This gives rise to the following Cubic equation:
                //a * t^3 + b * t^2 + c * t + d = 0

                //The spline control points (p0-3) consist of two variables: the x and y coordinates. They are independent so we can handle them separately.
                //Below, a1 is substitution a where the x coordinate of each point is used, like so:  a1 = 0.5 * (-p0.x + 3*p1.x - 3*p2.x + p3.x)
                //Below, a2 is substitution a where the y coordinate of each point is used, like so:  a2 = 0.5 * (-p0.y + 3*p1.y - 3*p2.y + p3.y)
                //The same logic applies for substitutions b, c, and d.

                float a1 = 0.5f * (-p0.x + 3f * p1.x - 3f * p2.x + p3.x);
                float a2 = 0.5f * (-p0.y + 3f * p1.y - 3f * p2.y + p3.y);
                float b1 = 0.5f * (2f * p0.x - 5f * p1.x + 4f * p2.x - p3.x);
                float b2 = 0.5f * (2f * p0.y - 5f * p1.y + 4f * p2.y - p3.y);
                float c1 = 0.5f * (-p0.x + p2.x);
                float c2 = 0.5f * (-p0.y + p2.y);
                float d1 = 0.5f * (2f * p1.x);
                float d2 = 0.5f * (2f * p1.y);

                //We now have two Cubic functions. One for x and one for y.
                //Note that a, b, c, and d are not vector variables itself but substituted functions.
                //x = a1 * t^3 + b1 * t^2 + c1 * t + d1
                //y = a2 * t^3 + b2 * t^2 + c2 * t + d2

                //Line formula, standard form:
                //Ax + By + C = 0
                float A = linePoints[0].y - linePoints[1].y;
                float B = linePoints[1].x - linePoints[0].x;
                float C = (linePoints[0].x - linePoints[1].x) * linePoints[0].y + (linePoints[1].y - linePoints[0].y) * linePoints[0].x;

                //Substituting the values of x and y from the separated Spline formula into the Line formula, we get:
                //A * (a1 * t^3 + b1 * t^2 + c1 * t + d1) + B * (a2 * t^3 + b2 * t^2 + c2 * t + d2) + C = 0

                //Rearranged version:		
                //(A * a1 + B * a2) * t^3 + (A * b1 + B * b2) * t^2 + (A * c1 + B * c2) * t + (A * d1 + B * d2 + C) = 0

                //Substituting gives rise to a Cubic function:
                //a * t^3 + b * t^2 + c * t + d = 0
                float a = A * a1 + B * a2;
                float b = A * b1 + B * b2;
                float c = A * c1 + B * c2;
                float d = A * d1 + B * d2 + C;


                //This is again a Cubic equation, combined from the Line and the Spline equation. If you solve this you can get up to 3 line-spline cross points.
                //How to solve a Cubic equation is described here: 
                //https://www.cs.rit.edu/~ark/pj/lib/edu/rit/numeric/Cubic.shtml
                //https://www.codeproject.com/Articles/798474/To-Solve-a-Cubic-Equation

                int crossAmount;
                float cross1;
                float cross2;
                float cross3;
                float crossCorrected;

                //Two different implementations of solving a Cubic equation.
                //	SolveCubic2(out crossAmount, out cross1, out cross2, out cross3, a, b, c, d);
                SolveCubic(out crossAmount, out cross1, out cross2, out cross3, a, b, c, d);

                //Get the highest and lowest value (in range 0 to 1) of the current section and calculate the difference.
                float currentSectionLowest = (float)i / (float)numSections;
                float currentSectionHighest = ((float)i + 1f) / (float)numSections;
                float diff = currentSectionHighest - currentSectionLowest;

                //Only use the result if it is within range 0 to 1.
                //The range 0 to 1 is within the current segment. It has to be converted to the range of the entire spline,
                //which still uses a range of 0 to 1.
                if (cross1 >= 0 && cross1 <= 1)
                {

                    //Map an intermediate range (0 to 1) to the lowest and highest section values.
                    crossCorrected = (cross1 * diff) + currentSectionLowest;

                    //Add the result to the list.
                    list.Add(crossCorrected);
                }

                if (cross2 >= 0 && cross2 <= 1)
                {

                    //Map an intermediate range (0 to 1) to the lowest and highest section values.
                    crossCorrected = (cross2 * diff) + currentSectionLowest;

                    //Add the result to the list.
                    list.Add(crossCorrected);
                }

                if (cross3 >= 0 && cross3 <= 1)
                {

                    //Map an intermediate range (0 to 1) to the lowest and highest section values.
                    crossCorrected = (cross3 * diff) + currentSectionLowest;

                    //Add the result to the list.
                    list.Add(crossCorrected);
                }
            }

            //Convert the list to an array.
            crossings = list.ToArray();

            return crossings;
        }

        //Solve cubic equation according to Cardano. 
        //Source: https://www.cs.rit.edu/~ark/pj/lib/edu/rit/numeric/Cubic.shtml
        private static void SolveCubic(out int nRoots, out float x1, out float x2, out float x3, float a, float b, float c, float d)
        {

            float TWO_PI = 2f * Mathf.PI;
            float FOUR_PI = 4f * Mathf.PI;

            // Normalize coefficients.
            float denom = a;
            a = b / denom;
            b = c / denom;
            c = d / denom;

            // Commence solution.
            float a_over_3 = a / 3f;
            float Q = (3f * b - a * a) / 9f;
            float Q_CUBE = Q * Q * Q;
            float R = (9f * a * b - 27f * c - 2f * a * a * a) / 54f;
            float R_SQR = R * R;
            float D = Q_CUBE + R_SQR;

            if (D < 0.0f)
            {

                // Three unequal real roots.
                nRoots = 3;
                float theta = Mathf.Acos(R / Mathf.Sqrt(-Q_CUBE));
                float SQRT_Q = Mathf.Sqrt(-Q);
                x1 = 2f * SQRT_Q * Mathf.Cos(theta / 3f) - a_over_3;
                x2 = 2f * SQRT_Q * Mathf.Cos((theta + TWO_PI) / 3f) - a_over_3;
                x3 = 2f * SQRT_Q * Mathf.Cos((theta + FOUR_PI) / 3f) - a_over_3;
            }

            else if (D > 0.0f)
            {

                // One real root.
                nRoots = 1;
                float SQRT_D = Mathf.Sqrt(D);
                float S = CubeRoot(R + SQRT_D);
                float T = CubeRoot(R - SQRT_D);
                x1 = (S + T) - a_over_3;
                x2 = float.NaN;
                x3 = float.NaN;
            }

            else
            {

                // Three real roots, at least two equal.
                nRoots = 3;
                float CBRT_R = CubeRoot(R);
                x1 = 2 * CBRT_R - a_over_3;
                x2 = CBRT_R - a_over_3;
                x3 = x2;
            }
        }

        //Mathf.Pow is used as an alternative for cube root (Math.cbrt) here.
        private static float CubeRoot(float d)
        {

            if (d < 0.0f)
            {

                return -Mathf.Pow(-d, 1f / 3f);
            }

            else
            {

                return Mathf.Pow(d, 1f / 3f);
            }
        }


        //increase or decrease the length of vector by size
        public static Vector3 AddVectorLength(Vector3 vector, float size)
        {

            //get the vector length
            float magnitude = Vector3.Magnitude(vector);

            //calculate new vector length
            float newMagnitude = magnitude + size;

            //calculate the ratio of the new length to the old length
            float scale = newMagnitude / magnitude;

            //scale the vector
            return vector * scale;
        }

        //create a vector of direction "vector" with length "size"
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {

            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }


        //caclulate the rotational difference from A to B
        public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
        {

            Quaternion C = Quaternion.Inverse(A) * B;
            return C;
        }

        //Add rotation B to rotation A.
        public static Quaternion AddRotation(Quaternion A, Quaternion B)
        {

            Quaternion C = A * B;
            return C;
        }

        //Same as the build in TransformDirection(), but using a rotation instead of a transform.
        public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
        {

            Vector3 output = rotation * vector;
            return output;
        }

        //Same as the build in InverseTransformDirection(), but using a rotation instead of a transform.
        public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
        {

            Vector3 output = Quaternion.Inverse(rotation) * vector;
            return output;
        }

        //Rotate a vector as if it is attached to an object with rotation "from", which is then rotated to rotation "to".
        //Similar to TransformWithParent(), but rotating a vector instead of a transform.
        public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
        {
            //Note: comments are in case all inputs are in World Space.
            Quaternion Q = SubtractRotation(to, from);              //Output is in object space.
            Vector3 A = InverseTransformDirectionMath(from, vector);//Output is in object space.
            Vector3 B = Q * A;                                      //Output is in local space.
            Vector3 C = TransformDirectionMath(from, B);            //Output is in world space.
            return C;
        }

        //Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
        //The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
        //the function outputs true, otherwise false.
        public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
        {

            linePoint = Vector3.zero;
            lineVec = Vector3.zero;

            //We can get the direction of the line of intersection of the two planes by calculating the 
            //cross product of the normals of the two planes. Note that this is just a direction and the line
            //is not fixed in space yet. We need a point for that to go with the line vector.
            lineVec = Vector3.Cross(plane1Normal, plane2Normal);

            //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
            //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
            //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
            //the cross product of the normal of plane2 and the lineDirection.		
            Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

            float denominator = Vector3.Dot(plane1Normal, ldir);

            //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
            if (Mathf.Abs(denominator) > 0.006f)
            {

                Vector3 plane1ToPlane2 = plane1Position - plane2Position;
                float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
                linePoint = plane2Position + t * ldir;

                return true;
            }

            //output not valid
            else
            {
                return false;
            }
        }

        //Get the intersection between a line and a plane. 
        //If the line and plane are not parallel, the function outputs true, otherwise false.
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
        {

            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineVec, planeNormal);

            //line and plane are not parallel
            if (dotDenominator != 0.0f)
            {
                length = dotNumerator / dotDenominator;

                //create a vector from the linePoint to the intersection point
                vector = SetVectorLength(lineVec, length);

                //get the coordinates of the line-plane intersection point
                intersection = linePoint + vector;

                return true;
            }

            //output not valid
            else
            {
                return false;
            }
        }

        //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
        //same plane, use ClosestPointsOnTwoLines() instead.
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parrallel
            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }

            else
            {
                return false;
            }
        }

        public static float DistanceToLine(Vector3 position, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 projectedPoint = ProjectPointOnLineSegment(linePoint1, linePoint2, position);
            Vector3 vector = projectedPoint - position;
            return vector.magnitude;
        }

        public static float DistanceToLine(Camera camera, Vector3 mousePosition, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 projectedPoint;
            return DistanceToLine(camera, mousePosition, linePoint1, linePoint2, out projectedPoint);
        }

        public static float DistanceToLine(Camera camera, Vector3 mousePosition, Vector3 linePoint1, Vector3 linePoint2, out Vector3 projectedPoint)
        {
            Vector3 screenPos1 = camera.WorldToScreenPoint(linePoint1);
            Vector3 screenPos2 = camera.WorldToScreenPoint(linePoint2);
            projectedPoint = ProjectPointOnLineSegment(screenPos1, screenPos2, mousePosition);

            //set z to zero
            projectedPoint = new Vector3(projectedPoint.x, projectedPoint.y, 0f);

            Vector3 vector = projectedPoint - mousePosition;
            return vector.magnitude;
        }


        //This function returns a point which is a projection from a point to a line.
        //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
        public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
        {

            //get vector from point on line to point in space
            Vector3 linePointToPoint = point - linePoint;

            float t = Vector3.Dot(linePointToPoint, lineVec);

            return linePoint + lineVec * t;
        }

        //This function returns a point which is a projection from a point to a line segment.
        //If the projected point lies outside of the line segment, the projected point will 
        //be clamped to the appropriate line edge.
        //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
        public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {

            Vector3 vector = linePoint2 - linePoint1;

            Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

            int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

            //The projected point is on the line segment
            if (side == 0)
            {

                return projectedPoint;
            }

            if (side == 1)
            {

                return linePoint1;
            }

            if (side == 2)
            {

                return linePoint2;
            }

            //output is invalid
            return Vector3.zero;
        }

        //This function returns a point which is a projection from a point to a plane.
        public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {

            float distance;
            Vector3 translationVector;

            //First calculate the distance from the point to the plane:
            distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

            //Reverse the sign of the distance
            distance *= -1;

            //Get a translation vector
            translationVector = SetVectorLength(planeNormal, distance);

            //Translate the point to form a projection
            return point + translationVector;
        }

        //Projects a vector onto a plane. The output is not normalized.
        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        {

            return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
        }

        //Get the shortest distance between a point and a plane. The output is signed so it holds information
        //as to which side of the plane normal the point is.
        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {

            return Vector3.Dot(planeNormal, (point - planePoint));
        }

        //This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
        //to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
        //by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
        //the result of a dot product only has signed information when an angle is transitioning between more or less
        //than 90 degrees.
        public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {

            Vector3 perpVector;
            float dot;

            //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
            perpVector = Vector3.Cross(normal, vectorA);

            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            dot = Vector3.Dot(perpVector, vectorB);

            return dot;
        }

        public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
        {
            Vector3 perpVector;
            float angle;

            //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
            perpVector = Vector3.Cross(normal, referenceVector);

            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            angle = Vector3.Angle(referenceVector, otherVector);
            angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

            return angle;
        }

        //Calculate the angle between a vector and a plane. The plane is made by a normal vector.
        //Output is in radians.
        public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
        {

            float dot;
            float angle;

            //calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
            dot = Vector3.Dot(vector, normal);

            //this is in radians
            angle = (float)Math.Acos(dot);

            return 1.570796326794897f - angle; //90 degrees - angle
        }

        //Calculate the dot product as an angle
        public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
        {

            double dot;
            double angle;

            //get the dot product
            dot = Vector3.Dot(vec1, vec2);

            //Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
            if (dot < -1.0f)
            {
                dot = -1.0f;
            }
            if (dot > 1.0f)
            {
                dot = 1.0f;
            }

            //Calculate the angle. The output is in radians
            //This step can be skipped for optimization...
            angle = Math.Acos(dot);

            return (float)angle;
        }

        //Convert a plane defined by 3 points to a plane defined by a vector and a point. 
        //The plane point is the middle of the triangle defined by the 3 points.
        public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {

            planeNormal = Vector3.zero;
            planePoint = Vector3.zero;

            //Make two vectors from the 3 input points, originating from point A
            Vector3 AB = pointB - pointA;
            Vector3 AC = pointC - pointA;

            //Calculate the normal
            planeNormal = Vector3.Normalize(Vector3.Cross(AB, AC));

            //Get the points in the middle AB and AC
            Vector3 middleAB = pointA + (AB / 2.0f);
            Vector3 middleAC = pointA + (AC / 2.0f);

            //Get vectors from the middle of AB and AC to the point which is not on that line.
            Vector3 middleABtoC = pointC - middleAB;
            Vector3 middleACtoB = pointB - middleAC;

            //Calculate the intersection between the two lines. This will be the center 
            //of the triangle defined by the 3 points.
            //We could use LineLineIntersection instead of ClosestPointsOnTwoLines but due to rounding errors 
            //this sometimes doesn't work.
            Vector3 temp;
            ClosestPointsOnTwoLines(out planePoint, out temp, middleAB, middleABtoC, middleAC, middleACtoB);
        }

        //Returns the forward vector of a quaternion
        public static Vector3 GetForwardVector(Quaternion q)
        {
            return q * Vector3.forward;
        }

        //Returns the up vector of a quaternion
        public static Vector3 GetUpVector(Quaternion q)
        {
            return q * Vector3.up;
        }

        //Returns the right vector of a quaternion
        public static Vector3 GetRightVector(Quaternion q)
        {
            return q * Vector3.right;
        }

        //Gets a quaternion from a matrix
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        //Gets a position from a matrix
        public static Vector3 PositionFromMatrix(Matrix4x4 m)
        {
            Vector4 vector4Position = m.GetColumn(3);
            return new Vector3(vector4Position.x, vector4Position.y, vector4Position.z);
        }

        //This is an alternative for Quaternion.LookRotation. Instead of aligning the forward and up vector of the game 
        //object with the input vectors, a custom direction can be used instead of the fixed forward and up vectors.
        //alignWithVector and alignWithNormal are in world space.
        //customForward and customUp are in object space.
        //Usage: use alignWithVector and alignWithNormal as if you are using the default LookRotation function.
        //Set customForward and customUp to the vectors you wish to use instead of the default forward and up vectors.
        public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
        {
            //Set the rotation of the destination
            Quaternion rotationA = Quaternion.LookRotation(alignWithVector, alignWithNormal);

            //Set the rotation of the custom normal and up vectors. 
            //When using the default LookRotation function, this would be hard coded to the forward and up vector.
            Quaternion rotationB = Quaternion.LookRotation(customForward, customUp);

            //Calculate the rotation
            gameObjectInOut.transform.rotation = rotationA * Quaternion.Inverse(rotationB);
        }

        //This function transforms one object as if it was parented to the other.
        //Before using this function, the Init() function must be called
        //Input: parentRotation and parentPosition: the current parent transform.
        //Input: startParentRotation and startParentPosition: the transform of the parent object at the time the objects are parented.
        //Input: startChildRotation and startChildPosition: the transform of the child object at the time the objects are parented.
        //Output: childRotation and childPosition.
        //All transforms are in world space.
        public static void TransformWithParent(out Quaternion childRotation, out Vector3 childPosition, Quaternion parentRotation, Vector3 parentPosition, Quaternion startParentRotation, Vector3 startParentPosition, Quaternion startChildRotation, Vector3 startChildPosition)
        {

            childRotation = Quaternion.identity;
            childPosition = Vector3.zero;

            //set the parent start transform
            tempParent.rotation = startParentRotation;
            tempParent.position = startParentPosition;
            tempParent.localScale = Vector3.one; //to prevent scale wandering

            //set the child start transform
            tempChild.rotation = startChildRotation;
            tempChild.position = startChildPosition;
            tempChild.localScale = Vector3.one; //to prevent scale wandering

            //translate and rotate the child by moving the parent
            tempParent.rotation = parentRotation;
            tempParent.position = parentPosition;

            //get the child transform
            childRotation = tempChild.rotation;
            childPosition = tempChild.position;
        }

        //With this function you can align a triangle of an object with any transform.
        //Usage: gameObjectInOut is the game object you want to transform.
        //alignWithVector, alignWithNormal, and alignWithPosition is the transform with which the triangle of the object should be aligned with.
        //triangleForward, triangleNormal, and trianglePosition is the transform of the triangle from the object.
        //alignWithVector, alignWithNormal, and alignWithPosition are in world space.
        //triangleForward, triangleNormal, and trianglePosition are in object space.
        //trianglePosition is the mesh position of the triangle. The effect of the scale of the object is handled automatically.
        //trianglePosition can be set at any position, it does not have to be at a vertex or in the middle of the triangle.
        public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
        {

            //Set the rotation.
            LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);

            //Get the world space position of trianglePosition
            Vector3 trianglePositionWorld = gameObjectInOut.transform.TransformPoint(trianglePosition);

            //Get a vector from trianglePosition to alignWithPosition
            Vector3 translateVector = alignWithPosition - trianglePositionWorld;

            //Now transform the object so the triangle lines up correctly.
            gameObjectInOut.transform.Translate(translateVector, Space.World);
        }


        //Convert a position, direction, and normal vector to a transform
        public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
        {

            gameObjectInOut.transform.position = positionVector;
            gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
        }

        //This function finds out on which side of a line segment the point is located.
        //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
        //the line segment, project it on the line using ProjectPointOnLine() first.
        //Returns 0 if point is on the line segment.
        //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
        //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
        public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {

            Vector3 lineVec = linePoint2 - linePoint1;
            Vector3 pointVec = point - linePoint1;

            float dot = Vector3.Dot(pointVec, lineVec);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0)
            {

                //point is on the line segment
                if (pointVec.magnitude <= lineVec.magnitude)
                {

                    return 0;
                }

                //point is not on the line segment and it is on the side of linePoint2
                else
                {

                    return 2;
                }
            }

            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            else
            {

                return 1;
            }
        }

        //Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
        //made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
        //not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
        public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
        {

            bool pointAInside = false;
            bool pointBInside = false;

            pointAInside = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);

            if (!pointAInside)
            {

                pointBInside = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
            }

            //none of the points are inside, so check if a line is crossing
            if (!pointAInside && !pointBInside)
            {

                bool lineACrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
                bool lineBCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
                bool lineCCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
                bool lineDCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);

                if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)
                {

                    return true;
                }

                else
                {

                    return false;
                }
            }

            else
            {

                return true;
            }
        }

        //Returns true if "point" is in a rectangle mad up of RectA to RectD. The line point is assumed to be on the same 
        //plane as the rectangle. If the point is not on the plane, use ProjectPointOnPlane() first.
        public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
        {

            Vector3 vector;
            Vector3 linePoint;

            //get the center of the rectangle
            vector = rectC - rectA;
            float size = -(vector.magnitude / 2f);
            vector = AddVectorLength(vector, size);
            Vector3 middle = rectA + vector;

            Vector3 xVector = rectB - rectA;
            float width = xVector.magnitude / 2f;

            Vector3 yVector = rectD - rectA;
            float height = yVector.magnitude / 2f;

            linePoint = ProjectPointOnLine(middle, xVector.normalized, point);
            vector = linePoint - point;
            float yDistance = vector.magnitude;

            linePoint = ProjectPointOnLine(middle, yVector.normalized, point);
            vector = linePoint - point;
            float xDistance = vector.magnitude;

            if ((xDistance <= width) && (yDistance <= height))
            {

                return true;
            }

            else
            {

                return false;
            }
        }

        //Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of
        //pointB1 and pointB2. The two lines are assumed to be in the same plane.
        public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
        {

            Vector3 closestPointA;
            Vector3 closestPointB;
            int sideA;
            int sideB;

            Vector3 lineVecA = pointA2 - pointA1;
            Vector3 lineVecB = pointB2 - pointB1;

            bool valid = ClosestPointsOnTwoLines(out closestPointA, out closestPointB, pointA1, lineVecA.normalized, pointB1, lineVecB.normalized);

            //lines are not parallel
            if (valid)
            {

                sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
                sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);

                if ((sideA == 0) && (sideB == 0))
                {

                    return true;
                }

                else
                {

                    return false;
                }
            }

            //lines are parallel
            else
            {

                return false;
            }
        }

        //This function calculates the acceleration vector in meter/second^2.
        //Input: position. If the output is used for motion simulation, the input transform
        //has to be located at the seat base, not at the vehicle CG. Attach an empty GameObject
        //at the correct location and use that as the input for this function.
        //Gravity is not taken into account but this can be added to the output if needed.
        //A low number of samples can give a jittery result due to rounding errors.
        //If more samples are used, the output is more smooth but has a higher latency.
        public static bool LinearAcceleration(out Vector3 vector, Vector3 position, int samples)
        {

            Vector3 averageSpeedChange = Vector3.zero;
            vector = Vector3.zero;
            Vector3 deltaDistance;
            float deltaTime;
            Vector3 speedA;
            Vector3 speedB;

            //Clamp sample amount. In order to calculate acceleration we need at least 2 changes
            //in speed, so we need at least 3 position samples.
            if (samples < 3)
            {

                samples = 3;
            }

            //Initialize
            if (positionRegister == null)
            {

                positionRegister = new Vector3[samples];
                posTimeRegister = new float[samples];
            }

            //Fill the position and time sample array and shift the location in the array to the left
            //each time a new sample is taken. This way index 0 will always hold the oldest sample and the
            //highest index will always hold the newest sample. 
            for (int i = 0; i < positionRegister.Length - 1; i++)
            {

                positionRegister[i] = positionRegister[i + 1];
                posTimeRegister[i] = posTimeRegister[i + 1];
            }
            positionRegister[positionRegister.Length - 1] = position;
            posTimeRegister[posTimeRegister.Length - 1] = Time.time;

            positionSamplesTaken++;

            //The output acceleration can only be calculated if enough samples are taken.
            if (positionSamplesTaken >= samples)
            {

                //Calculate average speed change.
                for (int i = 0; i < positionRegister.Length - 2; i++)
                {

                    deltaDistance = positionRegister[i + 1] - positionRegister[i];
                    deltaTime = posTimeRegister[i + 1] - posTimeRegister[i];

                    //If deltaTime is 0, the output is invalid.
                    if (deltaTime == 0)
                    {

                        return false;
                    }

                    speedA = deltaDistance / deltaTime;
                    deltaDistance = positionRegister[i + 2] - positionRegister[i + 1];
                    deltaTime = posTimeRegister[i + 2] - posTimeRegister[i + 1];

                    if (deltaTime == 0)
                    {

                        return false;
                    }

                    speedB = deltaDistance / deltaTime;

                    //This is the accumulated speed change at this stage, not the average yet.
                    averageSpeedChange += speedB - speedA;
                }

                //Now this is the average speed change.
                averageSpeedChange /= positionRegister.Length - 2;

                //Get the total time difference.
                float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];

                //Now calculate the acceleration, which is an average over the amount of samples taken.
                vector = averageSpeedChange / deltaTimeTotal;

                return true;
            }

            else
            {

                return false;
            }
        }


        /*
        //This function calculates angular acceleration in object space as deg/second^2, encoded as a vector. 
        //For example, if the output vector is 0,0,-5, the angular acceleration is 5 deg/second^2 around the object Z axis, to the left. 
        //Input: rotation (quaternion). If the output is used for motion simulation, the input transform
        //has to be located at the seat base, not at the vehicle CG. Attach an empty GameObject
        //at the correct location and use that as the input for this function.
        //A low number of samples can give a jittery result due to rounding errors.
        //If more samples are used, the output is more smooth but has a higher latency.
        //Note: the result is only accurate if the rotational difference between two samples is less than 180 degrees.
        //Note: a suitable way to visualize the result is:
        Vector3 dir;
        float scale = 2f;	
        dir = new Vector3(vector.x, 0, 0);
        dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
        dir = gameObject.transform.TransformDirection(dir);
        Debug.DrawRay(gameObject.transform.position, dir, Color.red);	
        dir = new Vector3(0, vector.y, 0);
        dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
        dir = gameObject.transform.TransformDirection(dir);
        Debug.DrawRay(gameObject.transform.position, dir, Color.green);	
        dir = new Vector3(0, 0, vector.z);
        dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
        dir = gameObject.transform.TransformDirection(dir);
        Debug.DrawRay(gameObject.transform.position, dir, Color.blue);	*/
        public static bool AngularAcceleration(out Vector3 vector, Quaternion rotation, int samples)
        {

            Vector3 averageSpeedChange = Vector3.zero;
            vector = Vector3.zero;
            Quaternion deltaRotation;
            float deltaTime;
            Vector3 speedA;
            Vector3 speedB;

            //Clamp sample amount. In order to calculate acceleration we need at least 2 changes
            //in speed, so we need at least 3 rotation samples.
            if (samples < 3)
            {

                samples = 3;
            }

            //Initialize
            if (rotationRegister == null)
            {

                rotationRegister = new Quaternion[samples];
                rotTimeRegister = new float[samples];
            }

            //Fill the rotation and time sample array and shift the location in the array to the left
            //each time a new sample is taken. This way index 0 will always hold the oldest sample and the
            //highest index will always hold the newest sample. 
            for (int i = 0; i < rotationRegister.Length - 1; i++)
            {

                rotationRegister[i] = rotationRegister[i + 1];
                rotTimeRegister[i] = rotTimeRegister[i + 1];
            }
            rotationRegister[rotationRegister.Length - 1] = rotation;
            rotTimeRegister[rotTimeRegister.Length - 1] = Time.time;

            rotationSamplesTaken++;

            //The output acceleration can only be calculated if enough samples are taken.
            if (rotationSamplesTaken >= samples)
            {
                //Calculate average speed change.
                for (int i = 0; i < rotationRegister.Length - 2; i++)
                {
                    deltaRotation = SubtractRotation(rotationRegister[i + 1], rotationRegister[i]);
                    deltaTime = rotTimeRegister[i + 1] - rotTimeRegister[i];

                    //If deltaTime is 0, the output is invalid.
                    if (deltaTime == 0)
                    {
                        return false;
                    }

                    speedA = RotDiffToSpeedVec(deltaRotation, deltaTime);
                    deltaRotation = SubtractRotation(rotationRegister[i + 2], rotationRegister[i + 1]);
                    deltaTime = rotTimeRegister[i + 2] - rotTimeRegister[i + 1];

                    if (deltaTime == 0)
                    {
                        return false;
                    }

                    speedB = RotDiffToSpeedVec(deltaRotation, deltaTime);

                    //This is the accumulated speed change at this stage, not the average yet.
                    averageSpeedChange += speedB - speedA;
                }

                //Now this is the average speed change.
                averageSpeedChange /= rotationRegister.Length - 2;

                //Get the total time difference.
                float deltaTimeTotal = rotTimeRegister[rotTimeRegister.Length - 1] - rotTimeRegister[0];

                //Now calculate the acceleration, which is an average over the amount of samples taken.
                vector = averageSpeedChange / deltaTimeTotal;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Get y from a linear function, with x as an input. The linear function goes through points
        //0,0 on the left ,and Qxy on the right.
        public static float LinearFunction2DBasic(float x, float Qx, float Qy)
        {
            float y = x * (Qy / Qx);
            return y;
        }

        //Get y from a linear function, with x as an input. The linear function goes through points
        //Pxy on the left ,and Qxy on the right.
        public static float LinearFunction2DFull(float x, float Px, float Py, float Qx, float Qy)
        {
            float y = 0f;

            float A = Qy - Py;
            float B = Qx - Px;
            float C = A / B;

            y = Py + (C * (x - Px));

            return y;
        }

        //Convert a rotation difference to a speed vector.
        //For internal use only.
        private static Vector3 RotDiffToSpeedVec(Quaternion rotation, float deltaTime)
        {
            float x;
            float y;
            float z;

            if (rotation.eulerAngles.x <= 180.0f)
            {
                x = rotation.eulerAngles.x;
            }

            else
            {
                x = rotation.eulerAngles.x - 360.0f;
            }

            if (rotation.eulerAngles.y <= 180.0f)
            {
                y = rotation.eulerAngles.y;
            }

            else
            {
                y = rotation.eulerAngles.y - 360.0f;
            }

            if (rotation.eulerAngles.z <= 180.0f)
            {
                z = rotation.eulerAngles.z;
            }

            else
            {
                z = rotation.eulerAngles.z - 360.0f;
            }

            return new Vector3(x / deltaTime, y / deltaTime, z / deltaTime);
        }
    }
}

