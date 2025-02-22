using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    [CustomPropertyDrawer(typeof(LicenseCollector.Entry))]
    public class LicenseEntryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;
            height += EditorGUIUtility.singleLineHeight;

            var assetProp = property.FindPropertyRelative("_asset");
            height += EditorGUI.GetPropertyHeight(assetProp);

            var isIncludedProp = property.FindPropertyRelative("_isIncluded");
            height += EditorGUI.GetPropertyHeight(isIncludedProp);

            var reportProp = property.FindPropertyRelative("_report");
            height += EditorGUI.GetPropertyHeight(reportProp);

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;

            var r = new Rect(position);
            r.height = EditorGUIUtility.singleLineHeight;
            r.y += r.height;

            var assetProp = property.FindPropertyRelative("_asset");
            EditorGUI.PropertyField(r, assetProp);
            r.y += EditorGUI.GetPropertyHeight(assetProp);

            var isIncludedProp = property.FindPropertyRelative("_isIncluded");
            EditorGUI.PropertyField(r, isIncludedProp);
            r.y += EditorGUI.GetPropertyHeight(isIncludedProp);

            var reportProp = property.FindPropertyRelative("_report");
            EditorGUI.PropertyField(r, reportProp);
            r.y += EditorGUI.GetPropertyHeight(reportProp);

            var labelRect = new Rect(position);
            labelRect.height = EditorGUIUtility.singleLineHeight;

            var nameProp = reportProp.FindPropertyRelative("_name");

            var prevColor = GUI.contentColor;
            GUI.contentColor = GetReportColor(reportProp, prevColor);
            GUI.Label(labelRect, nameProp.stringValue);
            GUI.contentColor = prevColor;

            EditorGUI.indentLevel--;

            property.serializedObject.ApplyModifiedProperties();
        }

        private static Color GetReportColor(SerializedProperty property, Color defaultValue)
        {
            var licenseProp = property.FindPropertyRelative("_license");
            var remarksProp = property.FindPropertyRelative("_remarks");
            if (LicenseSharedDrawer.TryGetRemarksColor(remarksProp, out var color))
            {
                return color;
            }
            else if (LicenseSharedDrawer.TryGetLicenseColor(licenseProp, out color))
            {
                return color;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
