namespace LicenseManager.Editor
{
    public class TextAssetParser : IAssetParser
    {
        public IAssetParser.Result TryGetContent(UnityEngine.Object asset, out string result)
        {
            if (asset is UnityEngine.TextAsset textAsset)
            {
                result = textAsset.text;
                return IAssetParser.Result.Success;
            }
            else
            {
                result = default;
                return IAssetParser.Result.Unavailable;
            }
        }
    }
}
