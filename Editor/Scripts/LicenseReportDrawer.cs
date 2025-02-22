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

            var remarksProp = property.FindPropertyRelative("_remarks");
            height += LicenseEditorGUI.GetPropertyHeight(remarksProp);

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r = new Rect(position);

            var nameProp = property.FindPropertyRelative("_name");
            EditorGUI.PropertyField(r, nameProp);
            r.y += EditorGUI.GetPropertyHeight(nameProp);

            var prevColor = GUI.contentColor;

            var licenseProp = property.FindPropertyRelative("_license");
            GUI.contentColor = LicenseSharedDrawer.GetLicenseColor(licenseProp, prevColor);
            EditorGUI.PropertyField(r, licenseProp);
            r.y += EditorGUI.GetPropertyHeight(licenseProp);
            GUI.contentColor = prevColor;

            var remarksProp = property.FindPropertyRelative("_remarks");
            GUI.contentColor = LicenseSharedDrawer.GetRemarksColor(remarksProp, prevColor);
            LicenseEditorGUI.PropertyField(r, remarksProp);
            r.y += LicenseEditorGUI.GetPropertyHeight(remarksProp);
            GUI.contentColor = prevColor;

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
