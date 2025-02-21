namespace LicenseManager.Editor
{
    public interface ILicenseParser
    {
        public bool TryGetLicense(string content, out string result);
    }
}
