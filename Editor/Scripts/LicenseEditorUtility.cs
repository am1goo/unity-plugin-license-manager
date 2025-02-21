using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    public static class LicenseEditorUtility
    {
        public delegate bool OnLookupPredicate<T>(T asset, LicenseLookupOptions options) where T : UnityEngine.Object;

        public static void Refresh(LicenseCollector collector)
        {
            var assets = new List<UnityEngine.Object>();
            var options = new LicenseLookupOptions
            {
                includeUnityPackages = collector.includeUnityPackages,
                lookupNames = collector.licenseLookupNames,
            };
            FindAllLicenses(options, assets);

            var changed = false;
            var existed = new HashSet<UnityEngine.Object>();
            var toDelete = new List<LicenseCollector.Entry>();

            //remove missed licenses
            foreach (var entry in collector.entries)
            {
                if (entry.asset != null && assets.Contains(entry.asset))
                {
                    var report = LicenseReporter.GetReport(options, entry.asset);
                    if (report != null)
                    {
                        entry.SetReport(report);
                        existed.Add(entry.asset);
                        changed |= true;
                    }
                    else
                    {
                        toDelete.Add(entry);
                    }
                }
                else
                {
                    toDelete.Add(entry);
                }
            }

            //add newest licenses
            foreach (var asset in assets)
            {
                if (existed.Contains(asset))
                    continue;

                var report = LicenseReporter.GetReport(options, asset);
                if (report == null)
                    continue;

                var entry = new LicenseCollector.Entry(asset, isIncluded: true, report);
                changed |= collector.Add(entry);
            }

            foreach (var entry in toDelete)
            {
                changed |= collector.Remove(entry);
            }

            if (changed)
            {
                EditorUtility.SetDirty(collector);
            }
        }

        private static void FindAllLicenses(LicenseLookupOptions options, List<UnityEngine.Object> result)
        {
            var textAssets = new List<TextAsset>();
            FindAllAssets(textAssets, options, OnTextAssetLicenseLookup);

            var defaultAssets = new List<DefaultAsset>();
            FindAllAssets(defaultAssets, options, OnDefaultAssetLicenseLookup);

            foreach (var asset in textAssets)
            {
                result.Add(asset);
            }

            foreach (var asset in defaultAssets)
            {
                result.Add(asset);
            }
        }

        public static void FindAllAssets<T>(List<T> result) where T : UnityEngine.Object
        {
            FindAllAssets(result, default, null);
        }

        private static void FindAllAssets<T>(List<T> result, LicenseLookupOptions options, OnLookupPredicate<T> predicate) where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (predicate == null || predicate(asset, options))
                {
                    result.Add(asset);
                }
            }
        }

        private static bool OnTextAssetLicenseLookup(TextAsset textAsset, LicenseLookupOptions options)
        {
            return OnLicenseLookup(textAsset, options);
        }

        private static bool OnDefaultAssetLicenseLookup(DefaultAsset textAsset, LicenseLookupOptions options)
        {
            var isLicense = OnLicenseLookup(textAsset, options);
            if (!isLicense)
                return false;

            var assetPath = AssetDatabase.GetAssetPath(textAsset);
            return System.IO.File.Exists(assetPath);
        }

        private static bool OnLicenseLookup(UnityEngine.Object asset, LicenseLookupOptions options)
        {
            if (asset == null)
                return false;

            foreach (var lookupName in options.lookupNames)
            {
                if (asset.name.Equals(lookupName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
