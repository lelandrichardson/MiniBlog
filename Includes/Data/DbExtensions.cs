
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace MiniBlog.Includes.Data
{
    public static partial class SqlMapper
    {
        public static IEnumerable<T> QueryAndClose<T>(
            this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var ret = SqlMapper.Query<T>(cnn, sql, param, transaction, buffered, commandTimeout, commandType);
            cnn.Close();
            return ret;
        }

        public static IEnumerable<T> QueryOpenAndClose<T>(
            this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            cnn.Open();
            var ret = Query<T>(cnn, sql, param as object, transaction, buffered, commandTimeout, commandType);
            cnn.Close();
            return ret;
        }

        public static T Scalar<T>(
            this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var ret = QueryInternal<T>(cnn, sql, param as object, transaction, commandTimeout, commandType).First(); //TODO: for some reason this isn't allowing .First()?
            return ret;
        }

        public static T ScalarOpenAndClose<T>(
            this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            T ret;
            using(cnn)
            {
                cnn.Open();
                ret = QueryInternal<T>(cnn, sql, param as object, transaction, commandTimeout, commandType).First();
                cnn.Close();
            }
            
            return ret;
        }

        public static T ScalarAndClose<T>(
            this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var ret = QueryInternal<T>(cnn, sql, param as object, transaction, commandTimeout, commandType).First();
            cnn.Close();
            return ret;
        }
    }
}