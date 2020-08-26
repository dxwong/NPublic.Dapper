using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NPublic.Dapper.DapperManager;

namespace NPublic.Dapper.FrameworkTest
{
    class Program
    {
        public class KLine
        {
            public int ID { get; set; }
            public string symbol { get; set; }
        }
        static void Main(string[] args)
        {
            string ConnectionStr = "server=122.114.170.153,4004;UID=dxwang;PWD=qq520223;database=test";
            ConnectionStr = "server=(local);UID=dxwang;PWD=qq520223;database=test";

            string host = "Host = 101.132.40.166; UserName = root; Password = @Hy201919; Database = KLine; Port = 3316; CharSet = utf8; Allow Zero Datetime = true;";
            NDapper dbss = DapperManager.CreateDatabase(ConnectionStr, DBType.SqlServer);
            ConnectionState state = dbss.State();
            var list = dbss.Query<KLine>("select id,symbol from pp2009_min10");

            NDapper dbss1 = DapperManager.CreateDatabase(ConnectionStr, DBType.SqlServer);
            var list1 = dbss1.Query<KLine>("select id,symbol from pp2009_min11");

            NDapper dbss2 = DapperManager.CreateDatabase(ConnectionStr, DBType.SqlServer);
            var list2 = dbss2.Query<KLine>("select id,symbol from pp2009_min12");
            ////Database
            //DapperBase db = CreateDatabase.CreateDatabase(host, DBType.MySql);
            Console.ReadKey();

            //SqlLite
            NDapper dbSqlLite = DapperManager.CreateDatabase(@"symbo2.db", DBType.SqlLite);
            //ConnectionState DapperState = dbSqlLite.State();
            string createtb = "create table  hsi1903_min1 (id int, symbol varchar(50))";
            int x = dbSqlLite.Execute(createtb);
            int xs = dbSqlLite.Execute("insert into hsi1903_min1(id,symbol)values('1','122')");
            List<KLine> listSS = dbSqlLite.Query<KLine>("select id,symbol from hsi1903_min1").ToList();
           
        }
    }
}
