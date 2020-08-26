# NPublic.Dapper
![image](https://github.com/dxwong/NPublic.Dapper/blob/master/NDapper.png)<br>

.NET Standard 是 .NET Core 和 .NET Framework 共有的基础。<br>
NPublic.Dapper重新以.NET Standard封装Dapper并增加数据库操作错误日志。<br>
适用.NET Core，.NET Framework(.NET Framework会生成很多DLL，后续可能考虑封装两个版本)<br><br>

重新封装后的优点：<br>
1.对原有Dapper继承，不会有性能损耗。<br>
2.支持MySql，SqlServer，SqlLite，Oracle等常见数据库<br>
3.捕获一切数据库相关异常行为 如，<br>
<1>极少情况下，极少用户操作与数据库有关的异常<br>
<2>用户试探性攻击，注入<br>
<3>快速定位系统异常出现的位置，错误的SQL语句<br>
4.上述异常由NPublic.Dapper数据库层统一处理。开发过程不再关心这些异常，专注业务开发。<br><br>


5.NPublic.Dapper将异常日志压入MQ队列，以下情况可根据需要自行扩展<br>
<1>实时取队列数据，即可实时显示系统所有错误日志<br>
<2>可轻松应对大规模分布式日志处理<br>
<2>对特定日志，可直接发送邮件提示<br>
<3>加密解密，可根据需要对日志进行加密保存，用特定工具才能访问<br>
<4>使用特定工具，监控log日志文件，环比，同比增加X即报警提示<br>

【数据库操作Demo】<br>
NDapper db = DapperManager.CreateDatabase(ConnectionStr1, DBType.SqlServer);<br>
var list= db.Query<KLine>("select id,symbol from tb");//查询SqlServer<br><br>

NDapper mdb1 = DapperManager.CreateDatabase(ConnectionStr1, DBType.MySql);<br>
var list1 = mdb1.Query<KLine>("select id,symbol from tb1");//查询MySql数据库1的tb1数据表，返回Model<br><br>
            
NDapper mdb2 = DapperManager.CreateDatabase(ConnectionStr2, DBType.MySql);<br>
var list2 = mdb2.Query<KLine>("select id,symbol from tb2");//查询MySql数据库2的tb2数据表，返回Model<br><br>

NDapper dbSqlLite = DapperManager.CreateDatabase(@"symbo.db", DBType.SqlLite);<br>
string createtb = "create table  tb1 (id int, symbol varchar(50))";//SqlLite数据库中创建tb1数据表<br>
int x = dbSqlLite.Execute(createtb);<br><br><br>


【异常错误日志格式】<br>
2020-08-26 10:30:53 - 169.254.120.171<br>
Query:select id,symbol from tb1<br>
对象名 'tb1' 无效。<br><br>

2020-08-26 10:30:54 - 169.254.120.171<br>
Query:select id,symbol from tb2<br>
对象名 'tb2' 无效。<br><br>

2020-08-26 10:30:54 - 169.254.120.171<br>
Query:select id,symbol from tb001<br>
SQLite Error 1: 'no such table: tb001'.



