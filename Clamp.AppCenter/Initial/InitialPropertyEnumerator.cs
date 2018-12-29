namespace Clamp.AppCenter.Initial
{
    /// <summary>
    /// 用于处理数组类型
    /// </summary>
    internal sealed class InitialPropertyEnumerator
    {
        private readonly string _stringValue;
        private readonly bool _shouldCalcElemString;
        private int _idxInString;
        private readonly int _lastRBraceIdx;
        private int _prevElemIdxInString;
        private int _braceBalance;
        private bool _isInQuotes;
        private bool _isDone;

        public InitialPropertyEnumerator(string value, bool shouldCalcElemString)
        {
            _stringValue = value;
            _idxInString = -1;
            _lastRBraceIdx = -1;
            _shouldCalcElemString = shouldCalcElemString;
            IsValid = true;
            _isDone = false;

            for (int i = 0; i < value.Length; ++i)
            {
                char ch = value[i];

                if (ch != ' ' && ch != '{')
                    break;

                if (ch == '{')
                {
                    _idxInString = i + 1;
                    _braceBalance = 1;
                    _prevElemIdxInString = i + 1;
                    break;
                }
            }

            if (_idxInString < 0)
            {
                IsValid = false;
                _isDone = true;
                return;
            }

            for (int i = value.Length - 1; i >= 0; --i)
            {
                char ch = value[i];
                if (ch != ' ' && ch != '}')
                    break;

                if (ch == '}')
                {
                    _lastRBraceIdx = i;
                    break;
                }
            }

            if (_lastRBraceIdx < 0)
            {
                IsValid = false;
                _isDone = true;
                return;
            }

            if (_idxInString == _lastRBraceIdx ||
                !IsNonEmptyValue(_stringValue, _idxInString, _lastRBraceIdx))
            {
                IsValid = true;
                _isDone = true;
                return;
            }
        }

        private void UpdateElementString(int idx)
        {
            Current = _stringValue.Substring(
              _prevElemIdxInString,
              idx - _prevElemIdxInString
              );

            Current = Current.Trim(new[] { ' ', '\"' });
            Current = Current.Replace("\\\"", "\"");
        }

        public bool Next()
        {
            if (_isDone)
                return false;

            int idx = _idxInString;
            while (idx <= _lastRBraceIdx)
            {
                char ch = _stringValue[idx];
                if (ch == '{' && !_isInQuotes)
                {
                    ++_braceBalance;
                }
                else if (ch == '}' && !_isInQuotes)
                {
                    --_braceBalance;
                    if (idx == _lastRBraceIdx)
                    {
                        if (!IsNonEmptyValue(_stringValue, _prevElemIdxInString, idx))
                        {
                            IsValid = false;
                        }
                        else if (_shouldCalcElemString)
                        {
                            UpdateElementString(idx);
                        }
                        _isDone = true;
                        break;
                    }
                }
                else if (ch == '\"')
                {
                    int iNextQuoteMark = _stringValue.IndexOf('\"', idx + 1);
                    if (iNextQuoteMark > 0 && _stringValue[iNextQuoteMark - 1] != '\\')
                    {
                        idx = iNextQuoteMark;
                        _isInQuotes = false;
                    }
                    else
                    {
                        _isInQuotes = true;
                    }
                }
                else if (ch == InitialFile.ArrayElementSeparator && _braceBalance == 1 && !_isInQuotes)
                {
                    if (!IsNonEmptyValue(_stringValue, _prevElemIdxInString, idx))
                    {
                        // Empty value in-between commas; this is an invalid array.
                        IsValid = false;
                    }
                    else if (_shouldCalcElemString)
                    {
                        UpdateElementString(idx);
                    }

                    _prevElemIdxInString = idx + 1;

                    // Yield.
                    ++idx;
                    break;
                }

                ++idx;
            }

            _idxInString = idx;

            if (_isInQuotes)
                IsValid = false;

            return IsValid;
        }

        private static bool IsNonEmptyValue(string s, int begin, int end)
        {
            for (; begin < end; ++begin)
                if (s[begin] != ' ')
                    return true;

            return false;
        }

        public string Current { get; private set; }

        public bool IsValid { get; private set; }
    }
}
