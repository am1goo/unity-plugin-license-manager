using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    public static class LicenseSharedDrawer
    {
        public static Color GetLicenseColor(SerializedProperty property, Color defaultValue)
        {
            if (TryGetLicenseColor(property, out var result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool TryGetLicenseColor(SerializedProperty property, out Color color)
        {
            if (property.stringValue == LicenseReporter.unknownLicense)
            {
                color = Color.yellow;
                return true;
            }
            else
            {
                color = default;
                return false;
            }
        }

        public static Color GetRemarksColor(SerializedProperty property, Color defaultValue)
        {
            if (TryGetRemarksColor(property, out var result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool TryGetRemarksColor(SerializedProperty property, out Color color)
        {
            if (property.intValue > 0)
            {
                color = Color.red;
                return true;
            }
            else
            {
                color = default;
                return false;
            }
        }
    }
}
