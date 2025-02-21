namespace LicenseManager.Editor
{
    public interface IAssetParser
    {
        public Result TryGetContent(UnityEngine.Object asset, out string result);

        public enum Result
        {
            Failed      = 0,
            Success     = 1,
            Unavailable = 2,
        }
    }
}
