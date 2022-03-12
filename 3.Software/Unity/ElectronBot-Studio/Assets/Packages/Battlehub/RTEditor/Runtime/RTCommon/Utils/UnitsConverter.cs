using UnityEngine;

namespace Battlehub.RTCommon
{
    public static class UnitsConverter 
    {
        public static Vector3 MetersToFeet(Vector3 meters)
        {
            return meters * 3.2808f;
        }

        public static Vector3 FeetToMeters(Vector3 feet)
        {
            return feet * 0.3048f;
        }

        public static float MetersToFeet(float meters)
        {
            return meters * 3.2808f;
        }

        public static float FeetToMeters(float feet)
        {
            return feet * 0.3048f;
        }

        public static Vector3 MetersToInches(Vector3 meters)
        {
            return meters * 39.37007874f;
        }

        public static Vector3 InchesToMeters(Vector3 inches)
        {
            return inches * 0.0254f;
        }

        public static float MetersToInches(float meters)
        {
            return meters * 39.37007874f;
        }

        public static float InchesToMeters(float inches)
        {
            return inches * 0.0254f;
        }

        public static string MetersToFeetInches(float meters)
        {
            int feet, inchesleft;
            MetersToFeetInches(meters, out feet, out inchesleft);
            return feet.ToString("0′ ") + inchesleft.ToString("0″").PadLeft(3, '0');
        }

        public static void MetersToFeetInches(float meters, out int feet, out int inchesleft)
        {
            float inchfeet = Mathf.Abs(meters) / 0.3048f;
            feet = (int)inchfeet;
            inchesleft = (int)((inchfeet - System.Math.Truncate(inchfeet)) / 0.08333f);
            if (inchesleft == 12)
            {
                inchesleft = 0;
                feet += 1;
            }
        }
    }
}
