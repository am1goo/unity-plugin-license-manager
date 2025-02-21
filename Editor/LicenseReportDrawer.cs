using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    [CustomPropertyDrawer(typeof(LicenseReport))]
    public class LicenseReportDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;

            var nameProp = property.FindPropertyRelative("_name");
            height += EditorGUI.GetPropertyHeight(nameProp);

            var licenseProp = property.FindPropertyRelative("_license");
            height += EditorGUI.GetPropertyHeight(licenseProp);

            var contentProp = property.FindPropertyRelative("_content");
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r = new Rect(position);

            var nameProp = property.FindPropertyRelative("_name");
            EditorGUI.PropertyField(r, nameProp);
            r.y += r.height;

            var licenseProp = property.FindPropertyRelative("_license");
            EditorGUI.PropertyField(r, licenseProp);
            r.y += r.height;

            var contentProp = property.FindPropertyRelative("_content");
            var contentClicked = false;
            if (GUI.Button(r, "Copy Content"))
            {
                contentClicked = true;
            }
            r.y += r.height;

            property.serializedObject.ApplyModifiedProperties();

            if (contentClicked)
            {
                EditorGUIUtility.systemCopyBuffer = contentProp.stringValue;
            }
        }
    }
}
