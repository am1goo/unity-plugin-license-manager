using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    public class DefaultAssetParser : IAssetParser
    {
        public IAssetParser.Result TryGetContent(UnityEngine.Object asset, out string result)
        {
            if (asset is DefaultAsset defaultAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(defaultAsset);
                var assetFile = new FileInfo(assetPath);
                if (!assetFile.Exists)
                {
                    result = default;
                    return IAssetParser.Result.Failed;
                }

                try
                {
                    result = File.ReadAllText(assetFile.FullName);
                    return IAssetParser.Result.Success;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    result = default;
                    return IAssetParser.Result.Failed;
                }
            }
            else
            {
                result = default;
                return IAssetParser.Result.Unavailable;
            }
        }
    }
}
