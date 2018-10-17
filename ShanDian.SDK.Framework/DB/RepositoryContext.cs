using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace ShanDian.SDK.Framework.DB
{
    public class RepositoryContext : IRepositoryContext
    {
        private readonly IDbConnection _dbConnection;

        private IDbTransaction _dbTransaction = null;

        private readonly ReaderWriterLockWrapper readerWriter;

        public RepositoryContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            readerWriter = new ReaderWriterLockWrapper(dbConnection.Database.GetHashCode());
        }

        public IDbConnection DbConnection { get { return _dbConnection; } }

        public List<T> GetSet<T>(string sql, object param = null) where T : class
        {
            IEnumerable<T> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query<T>(sql, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }

        public List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null) where TReturn : class
        {
            IEnumerable<TReturn> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }
        public List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null) where TReturn : class
        {
            IEnumerable<TReturn> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }
        public List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null) where TReturn : class
        {
            IEnumerable<TReturn> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query(sql, map, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }
        public List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null) where TReturn : class
        {
            IEnumerable<TReturn> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query(sql, map, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }
        public List<TReturn> GetSet<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null) where TReturn : class
        {
            IEnumerable<TReturn> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query(sql, map, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }
        public List<TReturn> GetSet<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null) where TReturn : class
        {
            IEnumerable<TReturn> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query(sql, map, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }


        /// <summary>
        /// 返回影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Execute(string sql, object t)
        {
            int queryable;
            try
            {
                readerWriter.BeginWrite();
                queryable = _dbConnection.Execute(sql, t);
            }
            finally
            {
                readerWriter.EndWrite();
            }
            return queryable;
        }

        public int Delete(string sql, object t)
        {
            int queryable;
            try
            {
                readerWriter.BeginWrite();
                queryable = _dbConnection.Execute(sql, t);
            }
            finally
            {
                readerWriter.EndWrite();
            }
            return queryable;
        }

        public bool Delete<T>(T t, IDbTransaction dbTransaction = null) where T : class
        {
            bool queryable;
            try
            {
                readerWriter.BeginWrite();
                queryable = _dbConnection.Delete<T>(t, dbTransaction, null);
            }
            finally
            {
                readerWriter.EndWrite();
            }
            return queryable;
        }

        public long Insert<T>(T t, IDbTransaction dbTransaction = null) where T : class
        {
            long queryable;
            try
            {
                readerWriter.BeginWrite();
                queryable = _dbConnection.Insert<T>(t, dbTransaction, null);
            }
            finally
            {
                readerWriter.EndWrite();
            }
            return queryable;
        }

        public bool Update<T>(T t, IDbTransaction dbTransaction = null) where T : class
        {
            bool queryable;
            try
            {
                readerWriter.BeginWrite();
                queryable = _dbConnection.Update(t, dbTransaction, null);
            }
            finally
            {
                readerWriter.EndWrite();
            }
            return queryable;
        }

        public T Get<T>(T t, IDbTransaction dbTransaction = null) where T : class
        {
            T queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Get<T>(t, dbTransaction, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable;
        }

        public T QueryFirstOrDefault<T>(string sql, object param, IDbTransaction dbTransaction = null) where T : class
        {
            T queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.QueryFirstOrDefault<T>(sql, param, dbTransaction);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable;
        }

        public T First<T>(string sql, object param, IDbTransaction dbTransaction = null) where T : class
        {
            T queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.QueryFirst<T>(sql, param, null, null, CommandType.Text);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable;
        }

        public T FirstOrDefault<T>(string sql, object param, IDbTransaction dbTransaction = null) where T : class
        {
            T queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.QueryFirstOrDefault<T>(sql, param, null, null, CommandType.Text);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable;
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
        }


        public List<T> GetSetPage<T>(string allSql, object param = null) where T : class
        {

            IEnumerable<T> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query<T>(allSql, param, null, true, null, null).ToList();
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable.ToList();
        }


        public List<T> GetSetPage<T>(string fields, string table, string where, string orderby, int pageIndex, int pageSize, out int totalCount, object param = null, string countStr = null) where T : class
        {
            return GetSetPageBySkip<T>(fields, table, where, orderby, (pageIndex - 1) * pageSize, pageSize, out totalCount, param, countStr);
        }

        public List<T> GetSetPageBySkip<T>(string fields, string table, string where, string orderby, int skipNum, int takeNum, out int totalCount, object param = null, string countStr = null) where T : class
        {
            string sql = string.Format("select {0} from {1} {2} {3} limit {4},{5}", fields, table, string.IsNullOrWhiteSpace(where) ? "" : " where " + where, string.IsNullOrWhiteSpace(orderby) ? "" : " order by " + orderby, skipNum, takeNum);

            IEnumerable<T> queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.Query<T>(sql, param, null, true, null, null);
            }
            finally
            {
                readerWriter.EndRead();
            }
            string sqlCount = string.Format("select {2} totalCount from {0} {1}", table, string.IsNullOrWhiteSpace(where) ? "" : " where " + where, string.IsNullOrWhiteSpace(countStr) ? "count(1)" : countStr);
            totalCount = Convert.ToInt32(ExecuteScalar(sqlCount, param));
            return queryable.ToList();
        }


        public object ExecuteScalar(string sql, object param = null)
        {
            object queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.ExecuteScalar(sql, param);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable;
        }

        public IDataReader ExecuteReader(string sql, object param = null)
        {
            IDataReader queryable;
            try
            {
                readerWriter.BeginRead();
                queryable = _dbConnection.ExecuteReader(sql, param);
            }
            finally
            {
                readerWriter.EndRead();
            }
            return queryable;
        }


        public void BeginTransaction()
        {
            CheckConnect();
            //_dbTransaction = _dbConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            CheckConnect();
            if (_dbTransaction != null)
            {
                _dbTransaction.Commit();
                _dbTransaction = null;
                _dbConnection.Dispose();
            }
        }

        public void Rollback()
        {
            CheckConnect();
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
                _dbTransaction = null;
                _dbConnection.Dispose();
            }
        }

        protected virtual void CheckConnect()
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
        }
    }
}