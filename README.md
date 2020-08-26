# NPublic.Dapper
.NET Standard Library 是 .NET Core 和 .NET Framework 共有的基础。
Dapper是.NET平台数据库ORM最优选择之一，速度最快，最简单。

重新以.NET Standard封装Dapper并增加数据库操作错误日志。该Dapper适用.NET Core，.NET Framework

重新封装后的优点：
1.对原有Dapper继承，不会有性能损耗。
2.支持MySql,SqlServer,SqlLite,Oracle等常见数据库
3.业务逻辑层不再关心数据库操作异常，由底层统一处理
4.捕获一切数据库相关异常行为 如，
<1>极少情况下，极少用户操作与数据库有关的异常
<2>用户试探性攻击，注入
<3>快速定位系统异常出现的位置，错误的SQL语句

5.异常日志压入队列MQ，以下可根据需要自行扩展
<1>实时取队列数据，可以实时显示错误日志
<2>对特定日志，直接发生邮件提示
<3>加密解密，可根据需要对日志进行加密保存，用特定工具才能访问
<4>使用特定工具，监控log日志文件，环比，同比增加X即报警提示

【数据库操作Demo】
NDapper db = DapperManager.CreateDatabase(ConnectionStr1, DBType.SqlServer);
var list= db.Query<KLine>("select id,symbol from tb");//查询SqlServer

NDapper mdb1 = DapperManager.CreateDatabase(ConnectionStr1, DBType.MySql);
var list1 = mdb1.Query<KLine>("select id,symbol from tb1");//查询MySql数据库1的tb1数据表，返回Model
            
NDapper mdb2 = DapperManager.CreateDatabase(ConnectionStr2, DBType.MySql);
var list2 = mdb2.Query<KLine>("select id,symbol from tb2");//查询MySql数据库2的tb2数据表，返回Model

NDapper dbSqlLite = DapperManager.CreateDatabase(@"symbo.db", DBType.SqlLite);
string createtb = "create table  tb1 (id int, symbol varchar(50))";//SqlLite数据库中创建tb1数据表
int x = dbSqlLite.Execute(createtb);


【异常错误日志格式】
2020-08-26 10:30:53 - 169.254.120.171
Query:select id,symbol from tb1
对象名 'tb1' 无效。

2020-08-26 10:30:54 - 169.254.120.171
Query:select id,symbol from tb2
对象名 'tb2' 无效。

2020-08-26 10:30:54 - 169.254.120.171
Query:select id,symbol from tb001
SQLite Error 1: 'no such table: tb001'.
