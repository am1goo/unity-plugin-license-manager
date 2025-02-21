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
            r.y += r.height;

            var isIncludedProp = property.FindPropertyRelative("_isIncluded");
            EditorGUI.PropertyField(r, isIncludedProp);
            r.y += r.height;

            var reportProp = property.FindPropertyRelative("_report");
            EditorGUI.PropertyField(r, reportProp);
            r.y += r.height;

            var labelRect = new Rect(position);
            labelRect.height = EditorGUIUtility.singleLineHeight;

            var nameProp = reportProp.FindPropertyRelative("_name");
            var licenseProp = reportProp.FindPropertyRelative("_license");

            var prevColor = GUI.contentColor;
            GUI.contentColor = licenseProp.stringValue == LicenseReporter.unknownLicense ? Color.red : GUI.color;
            GUI.Label(labelRect, nameProp.stringValue);
            GUI.contentColor = prevColor;

            EditorGUI.indentLevel--;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
