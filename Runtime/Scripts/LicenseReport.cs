using System;
using UnityEngine;

namespace LicenseManager
{
    [Serializable]
    public class LicenseReport
    {
        [SerializeField]
        private string _name;
        public string name => _name;
        [SerializeField, TextArea]
        private string _content;
        public string content => _content;
        [SerializeField]
        private string _license;
        public string license => _license;
        [SerializeField]
        private LicenseRemarks _remarks;
        public LicenseRemarks remarks => _remarks;

        public LicenseReport(string name, string content, string license, LicenseRemarks remarks)
        {
            this._name = name;
            this._content = content;
            this._license = license;
            this._remarks = remarks;
        }
    }
}
