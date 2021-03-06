﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;

namespace iS3.Core.Serialization
{
    public enum DbType { Unknown, MDB, XLS, SQLServer };

    public abstract class DbAdapter
    {
        #region Member Variables
        // Database file name
        protected string _dbFile;

        // This is a table name prefix.
        // When you export tables from SQL Server to MDB,
        // an additional 'dbo_' prefix will appear in MDB table names.
        // To deal with this issue, TableNamePrefix be added to each table name.
        // For example, Boreholes will be dbo_Boreholes.
        // 前缀处理类，对MDB传入表格时候，自动在前面加上dbo_
        protected string _tableNamePrefix;

        protected DbType _dbType;

        // Connection string
        protected string _connStr;
        #endregion

        #region Public Properties
        public string TableNamePrefix
        {
            get { return _tableNamePrefix; }
            set { _tableNamePrefix = value; }
        }
        public DbType DatabaseType { get { return _dbType; } }
        public string ConnectionStr { get { return _connStr; } }
        #endregion

        #region Constructor
        //数据适配器
        public DbAdapter(string dbFile)
        {
            //首先初始化受保护的字段
            _dbFile = dbFile;
            _dbType = DbType.Unknown;
            _connStr = "Unknown file format";

            string dbTypeStr = DbFileExtension();
            dbTypeStr = dbTypeStr.ToUpper();
            if (dbTypeStr == "MDB")
            {
                _tableNamePrefix = "dbo_";
                _dbType = DbType.MDB;
            }
            else if (dbTypeStr == "XLS")
            {
                _dbType = DbType.XLS;
            }
        }
        #endregion

        //数据扩展方法，返回string
        public string DbFileExtension()
        {
            int i = _dbFile.LastIndexOf('.');
            int len = _dbFile.Length - i - 1;
            string dbTypeStr = _dbFile.Substring(i + 1, len);
            return dbTypeStr;
        }

        public abstract DbConnection NewConnection();
        public abstract DbDataReader ExcuteCommand(DbConnection conn,
            string strCmd);
        public abstract DbDataAdapter GetDbDataAdapter(DbConnection conn,
            string strCmd);
    }

    public class OdbcAdapter : DbAdapter
    {
        public OdbcAdapter(string _dbFile)
            : base (_dbFile)
        {
            //如果是MDB格式的，就使用access路径
            if (_dbType == DbType.MDB)
            {
                
                _connStr =
                    "DSN=MS Access Database;DBQ=" + _dbFile;
            }
            //如果是xls格式的，就是用excel路径
            else if (_dbType == DbType.XLS)
            {
                _connStr =
                    "DSN=Excel Files;DBQ=" + _dbFile;
            }
        }

        public override DbConnection NewConnection()
        {
            return new OdbcConnection(_connStr);
        }

        public override DbDataReader ExcuteCommand(DbConnection conn,
            string strCmd)
        {
            OdbcConnection odbcConn = conn as OdbcConnection;
            OdbcCommand cmd = new OdbcCommand(strCmd, odbcConn);
            return cmd.ExecuteReader();
        }

        public override DbDataAdapter GetDbDataAdapter(DbConnection conn,
            string strCmd)
        {
            return new OdbcDataAdapter(strCmd, conn as OdbcConnection);
        }
    }

    public class OleDbAdapter : DbAdapter
    {
        public OleDbAdapter(string _dbFile)
            : base(_dbFile)
        {
            if (_dbType == DbType.MDB || _dbType == DbType.XLS)
            {
                _connStr =
                    "Provider=Microsoft.Jet.OLEDB.4.0; Data Source="
                    + _dbFile;
            }
        }

        public override DbConnection NewConnection()
        {
            return new OleDbConnection(_connStr);
        }

        public override DbDataReader ExcuteCommand(DbConnection conn,
            string strCmd)
        {
            OleDbConnection oledbConn = conn as OleDbConnection;
            OleDbCommand cmd = new OleDbCommand(strCmd, oledbConn);
            return cmd.ExecuteReader();
        }

        public override DbDataAdapter GetDbDataAdapter(DbConnection conn,
            string strCmd)
        {
            return new OleDbDataAdapter(strCmd, conn as OleDbConnection);
        }
    }

}
