using System;

namespace LicenseManager
{
    [Flags]
    public enum LicenseRemarks
    {
        NonCommercial   = 1 << 0,
        Viral           = 1 << 1,
    }
}
