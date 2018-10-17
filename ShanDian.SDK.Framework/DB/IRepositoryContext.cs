using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Framework.DB
{
    public interface IRepositoryContext : IDisposable
    {
        #region 事务
        ///// <summary>
        ///// 开始事务
        ///// </summary>
        //void BeginTransaction();

        ///// <summary>
        ///// 提交事务
        ///// </summary>
        //void CommitTransaction();

        ///// <summary>
        ///// 回滚事务
        ///// </summary>
        //void Rollback();

        #endregion
        IDbConnection DbConnection { get; }

        List<T> GetSet<T>(string sql, object param = null) where T : class;
        /// <summary>
        /// 返回影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        int Execute(string sql, object t);
        int Delete(string sql, object t);
        bool Delete<T>(T t, IDbTransaction dbTransaction = null) where T : class;
        long Insert<T>(T t, IDbTransaction dbTransaction = null) where T : class;
        bool Update<T>(T t, IDbTransaction dbTransaction = null) where T : class;
        T Get<T>(T t, IDbTransaction dbTransaction = null) where T : class;
        T QueryFirstOrDefault<T>(string sql, object param, IDbTransaction dbTransaction = null) where T : class;

        List<T> GetSetPage<T>(string allSql, object param = null) where T : class;
        List<T> GetSetPage<T>(string fields, string table, string where, string orderby, int pageIndex, int pageSize, out int totalCount, object param = null, string countStr = null) where T : class;
        List<T> GetSetPageBySkip<T>(string fields, string table, string where, string orderby, int skipNum, int takeNum, out int totalCount, object param = null, string countStr = null) where T : class;
        object ExecuteScalar(string sql, object param = null);
        T FirstOrDefault<T>(string sql, object param, IDbTransaction dbTransaction = null) where T : class;
        List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null) where TReturn : class;
        List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null) where TReturn : class;
        List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null) where TReturn : class;
        List<TReturn> GetSet<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null) where TReturn : class;
        List<TReturn> GetSet<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null) where TReturn : class;
        List<TReturn> GetSet<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null) where TReturn : class;

        IDataReader ExecuteReader(string sql, object param = null);
    }
}