using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    public static class LicenseReporter
    {
        private const string _packageJsonFilename = "package.json";

        private const string _packagesPrefix = "Packages/";
        private const string _assetsPrefix = "Assets/";

        public const string unknownLicense = "Unknown License";

        private static readonly List<IAssetParser> _assetParsers = new List<IAssetParser>
        {
            new TextAssetParser(),
            new DefaultAssetParser(),
        };

        private static readonly List<ILicenseParser> _licenseParsers = new List<ILicenseParser>();

        public static LicenseReport GetReport(LicenseLookupOptions options, UnityEngine.Object asset)
        {
            _licenseParsers.Clear();
            var predefinedList = new List<PredefinedLicenseData>();
            LicenseEditorUtility.FindAllAssets(predefinedList);
            _licenseParsers.Add(new PredefinedLicenseParser(predefinedList));

            foreach (var parser in _assetParsers)
            {
                var result = parser.TryGetContent(asset, out var content);
                switch (result)
                {
                    case IAssetParser.Result.Unavailable:
                        continue;

                    case IAssetParser.Result.Success:
                        if (TryGetReport(asset, content, options, out var report))
                        {
                            return report;
                        }
                        else
                        {
                            return null;
                        }

                    case IAssetParser.Result.Failed:
                        {
                            Debug.LogError($"GetReport: we unable to get content from {asset}");
                            continue;
                        }

                    default:
                        throw new Exception($"unsupported type {result}");
                }
            }

            Debug.LogError($"GetReport: we unable to create report of {asset}");
            return null;
        }

        private static bool TryGetReport(UnityEngine.Object asset, string content, LicenseLookupOptions options, out LicenseReport result)
        {
            if (!TryGetName(asset, options, out var name))
            {
                result = default;
                return false;
            }

            if (TryGetLicense(content, out var license, out var licenseRemarks))
            {
                result = new LicenseReport(name, content, license, licenseRemarks);
                return true;
            }
            else
            {
                result = new LicenseReport(name, content, unknownLicense, (LicenseRemarks)0);
                return true;
            }
        }

        private static bool TryGetName(UnityEngine.Object asset, LicenseLookupOptions options, out string result)
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
                        Debug.LogError($"TryCalculateName: required '{_packageJsonFilename}' file is not found near '{assetPath}'");
                        result = default;
                        return false;
                    }

                    if (options.includeUnityPackages || !IsUnityPackage(package.name))
                    {
                        result = package.displayName;
                        return true;
                    }
                    else
                    {
                        result = default;
                        return false;
                    }
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
                        if (options.includeUnityPackages || !IsUnityPackage(package.name))
                        {
                            result = package.displayName;
                            return true;
                        }
                        else
                        {
                            result = default;
                            return false;
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
                    var substringLength = assetPath.Length - _assetsPrefix.Length - assetFile.Name.Length - 1;
                    result = assetPath.Substring(_assetsPrefix.Length, substringLength);
                    return true;
                }
            }
            else
            {
                Debug.LogError($"TryCalculateName: unsupported asset path '{assetPath}' of {asset}");
                result = default;
                return false;
            }
        }

        private static bool TryGetLicense(string content, out string result, out LicenseRemarks remarks)
        {
            foreach (var parser in _licenseParsers)
            {
                if (parser.TryGetLicense(content, out var license, out var licenseRemarks))
                {
                    result = license;
                    remarks = licenseRemarks;
                    return true;
                }
            }

            result = default;
            remarks = default;
            return false;
        }

        private static readonly IEnumerable<string> _unityThirdPartyPackages = new string[]
        {
            "com.autodesk.fbx",
            "com.havok.physics",
        };

        private static bool IsUnityPackage(string packageName)
        {
            return packageName.StartsWith("com.unity.") || _unityThirdPartyPackages.Contains(packageName);
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
