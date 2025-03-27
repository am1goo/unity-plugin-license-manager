using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LicenseManager.Editor
{
    public class PredefinedLicenseParser : ILicenseParser
    {
        private static readonly StringBuilder _sb = new StringBuilder();

        private IEnumerable<PredefinedLicenseData> _list;

        private PredefinedLicenseParser()
        {
            //do nothing
        }

        public PredefinedLicenseParser(IEnumerable<PredefinedLicenseData> list)
        {
            this._list = list;
        }

        public bool TryGetLicense(string content, out string result, out LicenseRemarks remarks)
        {
            foreach (var data in _list)
            {
                if (data == null)
                    continue;

                if (IsRequiredStrings(content, data.requiredStrings))
                {
                    result = data.licenseName;
                    remarks = data.remarks;
                    return true;
                }

                if (content.Contains(data.licenseUrl))
                {
                    result = data.licenseName;
                    remarks = data.remarks;
                    return true;
                }
            }

            result = default;
            remarks = default;
            return false;
        }

        private static bool IsRequiredStrings(string content, string[] requiredStrings)
        {
            if (requiredStrings == null || requiredStrings.Length <= 0)
                return false;

            var contentWords = new List<string>();
            NormalizeText(content, contentWords);

            var result = true;
            foreach (var requiredString in requiredStrings)
            {
                var requiredStringWords = new List<string>();
                NormalizeText(requiredString, requiredStringWords);

                if (!Contains(contentWords, requiredStringWords))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private static bool Contains(List<string> list, List<string> lookup)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var equals = true;
                for (int j = 0; j < lookup.Count; ++j)
                {
                    var ii = i + j;
                    if (ii < list.Count && list[ii] == lookup[j])
                        continue;

                    equals = false;
                    break;
                }

                if (equals)
                    return true;
            }
            return false;
        }

        private static void NormalizeText(string text, List<string> words)
        {
            _sb.Clear();
            for (int i = 0; i < text.Length; ++i)
            {
                var c = text[i];
                if (char.IsLetterOrDigit(c))
                {
                    _sb.Append(c);
                }
                else
                {
                    if (_sb.Length > 0)
                    {
                        var word = _sb.ToString().ToLowerInvariant();
                        words.Add(word);
                        _sb.Clear();
                    }
                }
            }
            if (_sb.Length > 0)
            {
                var word = _sb.ToString().ToLowerInvariant();
                words.Add(word);
                _sb.Clear();
            }
        }
    }
}
