using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.SDK.Framework.DB
{
    public class ReaderWriterLockWrapper
    {
        private ConcurrentDictionary<int, ReaderWriterLockSlim> _readerWriterLockDictionary = new ConcurrentDictionary<int, ReaderWriterLockSlim>();
        ReaderWriterLockSlim _rwLock;
        bool _isInTransaction = false;
        public ReaderWriterLockWrapper(ReaderWriterLockSlim rwLock)
        {
            this._rwLock = rwLock;
        }

        public ReaderWriterLockWrapper(int key)
        {
            _readerWriterLockDictionary.AddOrUpdate(key, new ReaderWriterLockSlim(), (s, slim) => new ReaderWriterLockSlim());
            this._rwLock = _readerWriterLockDictionary[key];
        }

        public bool IsInTransaction { get { return this._isInTransaction; } }
        public bool IsLockHeld { get { return this._rwLock.IsReadLockHeld || this._rwLock.IsWriteLockHeld || this._rwLock.IsUpgradeableReadLockHeld; } }
        public void BeginTransaction()
        {
            if (this._isInTransaction)
                throw new Exception("Cannot call this method repeated.");

            this._rwLock.EnterWriteLock();
            this._isInTransaction = true;
        }
        public void EndTransaction()
        {
            if (this._isInTransaction)
            {
                if (this._rwLock.IsWriteLockHeld)
                    this._rwLock.ExitWriteLock();
                this._isInTransaction = false;
            }
        }

        public bool BeginRead(int timeout = -1)
        {
            if (!this._isInTransaction)
                return this._rwLock.TryEnterReadLock(timeout);
            return false;
        }
        public void EndRead()
        {
            if (!this._isInTransaction)
                if (this._rwLock.IsReadLockHeld)
                    this._rwLock.ExitReadLock();
        }

        public bool BeginWrite(int timeout = -1)
        {
            if (!this._isInTransaction)
                return _rwLock.TryEnterWriteLock(timeout);
            return false;
        }
        public void EndWrite()
        {
            if (!this._isInTransaction)
                if (this._rwLock.IsWriteLockHeld)
                    this._rwLock.ExitWriteLock();
        }
    }
}