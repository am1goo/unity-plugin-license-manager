using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LicenseManager.Editor
{
    public class LicensePreprocessBuildWithReport : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var collectors = new List<LicenseCollector>();
            LicenseEditorUtility.FindAllAssets(collectors);

            if (collectors.Count == 0)
            {
                Debug.LogWarning("OnPreprocessBuild: no license collectors found");
                return;
            }

            var count = 0;
            foreach (var collector in collectors)
            {
                if (collector.refreshOnBuild == false)
                    continue;

                LicenseEditorUtility.Refresh(collector);
                count++;

                Debug.Log($"OnPreprocessBuild: '{collector.name}' collector was refreshed successfully");
            }
            Debug.Log($"OnPreprocessBuild: {count}/{collectors.Count} license collectors processed");
        }
    }
}
