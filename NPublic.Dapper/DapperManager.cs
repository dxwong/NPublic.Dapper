﻿using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
//using Oracle.ManagedDataAccess.Client;需要请自行引用NuGet包

/// <summary>
/// dxwang
/// 4638912@qq.com
/// </summary>
namespace NPublic.Dapper
{
    public static partial class DapperManager
    {
        /// <summary>
        /// 创建DapperBase对象；默认为SqlServer
        /// </summary>
        /// <param name="strconn">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="WriteLog">是否写错误日志</param>
        /// <returns></returns>
        public static NDapper CreateDatabase(string strconn, DBType dbType = DBType.SqlServer)
        {
            NPublic.Dapper.Log.LogInit();//日志处理入口

            if (dbType == DBType.SqlLite)
            {
                strconn = string.Format("Data Source={0}", strconn);
                return new NDapper(new SqliteConnection(strconn));
            }

            if (dbType == DBType.MySql)
            {
                return new NDapper(new MySqlConnection(strconn));
            }

            if (dbType == DBType.SqlServer)
            {
                return new NDapper(new SqlConnection(strconn));
            }

            if (dbType == DBType.Oracle)
            {
                //IDbConnection conn = new OracleConnection(strconn);请自行引用NuGet包
                //return new DapperBase(conn, WriteLog);
            }

            return null;
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum DBType
        {
            SqlServer,
            MySql,
            SqlLite,
            Oracle
        }
    }
}
