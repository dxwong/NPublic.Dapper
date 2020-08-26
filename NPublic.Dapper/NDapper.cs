using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

/// <summary>
/// dxwang
/// 4638912@qq.com
/// </summary>
namespace NPublic.Dapper
{
    public partial class DapperManager
    {
        /// <summary>
        /// 执行主要操作的类,重写Dapper
        /// </summary>
        public class NDapper
        {
            public IDbTransaction dbTransaction { get; set; }
            private readonly IDbConnection conn;

            /// <summary>
            /// 构造函数
            /// </summary>
            public NDapper(IDbConnection conn)
            {
                this.conn = conn;
            }

            #region 判断数据库连接状态
            /// <summary>
            /// 判断数据库连接状态
            /// </summary>
            /// <returns></returns>
            public ConnectionState State()
            {
                try
                {
                    conn.Open();
                    return conn.State;
                }
                catch { }
                finally
                {
                    conn.Close();
                }
                return conn.State;
            }
            #endregion

            #region 事务提交

            /// <summary>
            /// 事务开始
            /// </summary>
            /// <returns></returns>
            public NDapper BeginTransaction()
            {
                conn.BeginTransaction();//需要手动开启事务控制
                return this; ;
            }

            /// <summary>
            /// 提交当前操作的结果
            /// </summary>
            public int Commit()
            {
                try
                {
                    if (dbTransaction != null)
                    {
                        dbTransaction.Commit();
                        this.Close();
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    Log.EnqueueMessage(Log.LogLevel.Error, ex, "Commit");
                    return -1;
                }
                finally
                {
                    if (dbTransaction == null)
                    {
                        this.Close();
                    }
                }
            }

            /// <summary>
            /// 把当前操作回滚成未提交状态
            /// </summary>
            public void Rollback()
            {
                this.dbTransaction.Rollback();
                this.dbTransaction.Dispose();
                this.Close();
            }

            /// <summary>
            /// 关闭连接 内存回收
            /// </summary>
            public void Close()
            {
                IDbConnection dbConnection = dbTransaction.Connection;
                if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }

            #endregion

            #region 实例方法

            #region 查询

            /// <summary>
            /// 查询
            /// </summary>
            /// <typeparam name="T">返回类型</typeparam>
            /// <param name="sql">sql语句</param>
            /// <param name="dbConnKey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public T QueryFirst<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return conn.QueryFirstOrDefault<T>(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "QueryFirst", sql);
                        return default(T);
                    }
                }
                else
                {
                    return dbTransaction.Connection.QueryFirstOrDefault<T>(sql, param, dbTransaction, commandTimeout, commandType);
                }

            }

            /// <summary>
            /// 查询(异步版本)
            /// </summary>
            /// <typeparam name="T">返回类型</typeparam>
            /// <param name="sql">sql语句</param>
            /// <param name="dbConnKey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public async Task<T> QueryFirstAsync<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "QueryFirstAsync", sql);
                        return default(T);
                    }
                }
                else
                {
                    return await dbTransaction.Connection.QueryFirstOrDefaultAsync<T>(sql, param, dbTransaction, commandTimeout, commandType);
                }

            }


            /// <summary>
            /// 查询
            /// </summary>
            /// <typeparam name="T">返回类型</typeparam>
            /// <param name="sql">sql语句</param>
            /// <param name="dbConnKey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="buffered">是否缓冲</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return conn.Query<T>(sql, param, null, buffered, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "Query", sql);
                        return null;
                    }
                }
                else
                {
                    return dbTransaction.Connection.Query<T>(sql, param, dbTransaction, buffered, commandTimeout, commandType);
                }

            }


            /// <summary>
            /// 查询(异步版本)
            /// </summary>
            /// <typeparam name="T">返回类型</typeparam>
            /// <param name="sql">sql语句</param>
            /// <param name="dbConnKey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="buffered">是否缓冲</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return await conn.QueryAsync<T>(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "QueryAsync", sql);
                        return null;
                    }
                }
                else
                {
                    return await dbTransaction.Connection.QueryAsync<T>(sql, param, dbTransaction, commandTimeout, commandType);
                }

            }



            /// <summary>
            /// 查询返回 IDataReader
            /// </summary>
            /// <param name="sql">sql语句</param>
            /// <param name="dbConnKey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public IDataReader ExecuteReader(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return conn.ExecuteReader(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "ExecuteReader", sql);
                        return null;
                    }
                }
                else
                {
                    return dbTransaction.Connection.ExecuteReader(sql, param, dbTransaction, commandTimeout, commandType);
                }
            }

            /// <summary>
            /// 查询单个返回值
            /// </summary>
            /// <typeparam name="T">返回类型</typeparam>
            /// <param name="sql">sql语句</param>
            /// <param name="dbConnKey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return conn.ExecuteScalar<T>(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "ExecuteScalar", sql);
                        return default(T);
                    }
                }
                else
                {
                    return dbTransaction.Connection.ExecuteScalar<T>(sql, param, dbTransaction, commandTimeout, commandType);
                }

            }
            #endregion

            /// <summary>
            /// 执行增删改sql
            /// </summary>
            /// <param name="sql">sql</param>
            /// <param name="dbkey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public int Execute(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return conn.Execute(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "ExecuteSql", sql);
                        return -1;
                    }
                }
                else
                {
                    return dbTransaction.Connection.Execute(sql, param, dbTransaction);
                }
            }

            /// <summary>
            /// 执行增删改sql(异步版本)
            /// </summary>
            /// <param name="sql">sql</param>
            /// <param name="dbkey">数据库连接</param>
            /// <param name="param">sql查询参数</param>
            /// <param name="commandTimeout">超时时间</param>
            /// <param name="commandType">命令类型</param>
            /// <returns></returns>
            public async Task<int> ExecuteSqlAsync(string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
            {
                if (dbTransaction == null)
                {
                    try
                    {
                        return await conn.ExecuteAsync(sql, param, null, commandTimeout, commandType);
                    }
                    catch (Exception ex)
                    {
                        Log.EnqueueMessage(Log.LogLevel.Error, ex, "ExecuteSqlAsync", sql);
                        return -1;
                    }
                }
                else
                {
                    await dbTransaction.Connection.ExecuteAsync(sql, param, dbTransaction);
                    return 0;
                }
            }
            #endregion
        }
    }
}
