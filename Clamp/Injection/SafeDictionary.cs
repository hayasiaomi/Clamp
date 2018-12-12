using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.OSGI.Injection
{
    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly ReaderWriterLockSlim _padlock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _Dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            set
            {
                _padlock.EnterWriteLock();

                try
                {
                    TValue current;
                    if (_Dictionary.TryGetValue(key, out current))
                    {
                        var disposable = current as IDisposable;

                        if (disposable != null)
                            disposable.Dispose();
                    }

                    _Dictionary[key] = value;
                }
                finally
                {
                    _padlock.ExitWriteLock();
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            _padlock.EnterReadLock();
            try
            {
                return _Dictionary.TryGetValue(key, out value);
            }
            finally
            {
                _padlock.ExitReadLock();
            }
        }

        public bool Remove(TKey key)
        {
            _padlock.EnterWriteLock();
            try
            {
                return _Dictionary.Remove(key);
            }
            finally
            {
                _padlock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _padlock.EnterWriteLock();
            try
            {
                _Dictionary.Clear();
            }
            finally
            {
                _padlock.ExitWriteLock();
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                _padlock.EnterReadLock();
                try
                {
                    return new List<TKey>(_Dictionary.Keys);
                }
                finally
                {
                    _padlock.ExitReadLock();
                }
            }
        }


        public void Dispose()
        {
            _padlock.EnterWriteLock();

            try
            {
                var disposableItems = from item in _Dictionary.Values
                                      where item is IDisposable
                                      select item as IDisposable;

                foreach (var item in disposableItems)
                {
                    item.Dispose();
                }
            }
            finally
            {
                _padlock.ExitWriteLock();
            }

            GC.SuppressFinalize(this);
        }
    }
}
