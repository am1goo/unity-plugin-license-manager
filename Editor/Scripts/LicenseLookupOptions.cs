using System.Collections.Generic;

namespace LicenseManager.Editor
{
    public struct LicenseLookupOptions
    {
        public bool includeUnityPackages;
        public IEnumerable<string> lookupNames;
    }
}
