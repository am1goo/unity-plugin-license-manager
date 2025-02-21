using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LicenseManager
{
    [CreateAssetMenu(fileName = "LicenseCollector", menuName = "License Collector")]
    public class LicenseCollector : ScriptableObject
    {
        [SerializeField]
        private string[] _licenseLookupNames = _defaultLicenseLookupNames.ToArray();
        public IEnumerable<string> licenseLookupNames => _licenseLookupNames;

        [SerializeField]
        private List<Entry> _entries;
        public IEnumerable<Entry> entries => _entries;

        private static readonly IEnumerable<string> _defaultLicenseLookupNames = new string[]
        {
            "LICENSE",
            "OSF",
            "ApacheLicense2.0",
        };

        private void OnValidate()
        {
            if (_licenseLookupNames == null || _licenseLookupNames.Length == 0)
            {
                _licenseLookupNames = _defaultLicenseLookupNames.ToArray();
            }
        }

        public bool Add(Entry entry)
        {
            if (entry == null)
                return false;

            if (_entries.Contains(entry))
                return false;

            _entries.Add(entry);
            return true;
        }

        public bool Remove(Entry entry)
        {
            return _entries.Remove(entry);
        }

        public List<LicenseReport> GetReport()
        {
            var list = new List<LicenseReport>();
            GetReport(list);
            return list;
        }

        public void GetReport(List<LicenseReport> result)
        {
            foreach (var entry in _entries)
            {
                if (entry.isIncluded == false)
                    continue;

                result.Add(entry.report);
            }
        }

        [Serializable]
        public class Entry
        {
            [SerializeField]
            private UnityEngine.Object _asset;
            public UnityEngine.Object asset => _asset;
            [SerializeField]
            private bool _isIncluded;
            public bool isIncluded => _isIncluded;
            [SerializeField]
            private LicenseReport _report;
            public LicenseReport report => _report;

            public Entry(UnityEngine.Object asset, bool isIncluded, LicenseReport report)
            {
                this._asset = asset;
                this._isIncluded = isIncluded;
                this._report = report;
            }

            public void SetReport(LicenseReport report)
            {
                this._report = report;
            }
        }
    }
}