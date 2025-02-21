using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    public static class LicenseEditorUtility
    {
        private const string _packageJsonFilename = "package.json";

        private const string _packagesPrefix = "Packages/";
        private const string _assetsPrefix = "Assets/";

        public delegate bool OnLookupPredicate<T>(T asset, LookupOptions options) where T : UnityEngine.Object;

        public static void Refresh(LicenseCollector collector)
        {
            var assets = new List<UnityEngine.Object>();
            var options = new LookupOptions
            {
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
                    Update(entry, ref changed);
                    existed.Add(entry.asset);
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

                var entry = new LicenseCollector.Entry(asset, isIncluded: true);
                Update(entry, ref changed);

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

        private static void FindAllLicenses(LookupOptions options, List<UnityEngine.Object> result)
        {
            var textAssets = new List<TextAsset>();
            FindAllAssets(textAssets, options, OnTextAssetLicenseLookup);

            var defaultAssets = new List<UnityEditor.DefaultAsset>();
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

        private static void FindAllAssets<T>(List<T> result, LookupOptions options, OnLookupPredicate<T> predicate) where T : UnityEngine.Object
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

        private static bool OnTextAssetLicenseLookup(TextAsset textAsset, LookupOptions options)
        {
            return OnLicenseLookup(textAsset, options);
        }

        private static bool OnDefaultAssetLicenseLookup(DefaultAsset textAsset, LookupOptions options)
        {
            var isLicense = OnLicenseLookup(textAsset, options);
            if (!isLicense)
                return false;

            var assetPath = AssetDatabase.GetAssetPath(textAsset);
            return System.IO.File.Exists(assetPath);
        }

        private static bool OnLicenseLookup(UnityEngine.Object asset, LookupOptions options)
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

        private static void Update(LicenseCollector.Entry entry, ref bool changed)
        {
            if (entry == null)
                return;

            if (TryCalculateName(entry.asset, out var name))
            {
                entry.SetName(name);
                changed |= true;
            }
        }

        private static bool TryCalculateName(UnityEngine.Object asset, out string result)
        {
            if (asset == null)
            {
                result = default;
                return false;
            }

            var assetPath = AssetDatabase.GetAssetPath(asset);
            var assetFile = new System.IO.FileInfo(assetPath);
            var assetDirectory = assetFile.Directory;
            if (assetPath.StartsWith(_packagesPrefix))
            {
                var packageFile = new System.IO.FileInfo(System.IO.Path.Combine(assetDirectory.FullName, _packageJsonFilename));
                if (packageFile.Exists)
                {
                    var package = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
                    if (package == null)
                    {
                        Debug.LogError($"required '{_packageJsonFilename}' file is not found near '{assetPath}'");
                        result = default;
                        return false;
                    }

                    result = package.displayName;
                    return true;
                }
                else
                {
                    result = assetDirectory.Name;
                    return true;
                }
            }
            else if (assetPath.StartsWith(_assetsPrefix))
            {
                var packageFile = new System.IO.FileInfo(System.IO.Path.Combine(assetDirectory.FullName, _packageJsonFilename));
                if (packageFile.Exists)
                {
                    var packageJson = System.IO.File.ReadAllText(packageFile.FullName);
                    var package = JsonUtility.FromJson<PackageInfo>(packageJson);
                    if (package != null)
                    {
                        result = package.displayName;
                        return true;
                    }
                    else
                    {
                        var substringLength = assetPath.Length - _assetsPrefix.Length - assetFile.Name.Length - 1;
                        result = assetPath.Substring(_assetsPrefix.Length, substringLength);
                        return true;
                    }
                }
                else
                {
                    var substringLength = assetPath.Length - _assetsPrefix.Length - assetFile.Name.Length - 1;
                    result = assetPath.Substring(_assetsPrefix.Length, substringLength);
                    return true;
                }
            }
            else
            {
                Debug.LogError($"unsupported asset path '{assetPath}' of {asset}");
                result = default;
                return false;
            }
        }

        public struct LookupOptions
        {
            public IEnumerable<string> lookupNames;
        }

        [Serializable]
        public class PackageInfo
        {
            public string displayName;
            public string name;
            public string description;
            public string type;
            public string category;
            public string version;
            public string licensesUrl;
            public string changelogUrl;
            public string documentationUrl;
            public AuthorInfo author;
            public string[] keywords;

            [Serializable]
            public class AuthorInfo
            {
                public string name;
                public string email;
                public string url;
            }
        }
    }
}
