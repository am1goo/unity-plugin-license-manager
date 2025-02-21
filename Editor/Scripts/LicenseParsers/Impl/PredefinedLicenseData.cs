using UnityEngine;

namespace LicenseManager.Editor
{
    [CreateAssetMenu(fileName = "PredefinedLicenseData", menuName = "License Manager/Predefined License Data")]
    public class PredefinedLicenseData : ScriptableObject
    {
        [SerializeField]
        private string _licenseName;
        public string licenseName => _licenseName;
        [SerializeField]
        private string _licenseUrl;
        public string licenseUrl => _licenseUrl;
        [SerializeField, TextArea]
        private string[] _requiredStrings;
        public string[] requiredStrings => _requiredStrings;
    }
}
