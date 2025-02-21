using UnityEngine;
using UnityEditor;

namespace LicenseManager.Editor
{
    [CustomEditor(typeof(LicenseCollector))]
    public class LicenseCollectorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var collector = serializedObject.targetObject as LicenseCollector;
            if (GUILayout.Button("Refresh"))
            {
                LicenseEditorUtility.Refresh(collector);
            }
        }
    }
}
