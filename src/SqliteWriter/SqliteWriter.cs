using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;

namespace DonetSpider
{
    public class SqliteWriter:IDisposable
    {
        public string DbName { get; private set; }
        SqliteConnection conn;
        public SqliteWriter(string dbName) {
            if (dbName.EndsWith(".db") || dbName.EndsWith(".DB") || dbName.EndsWith(".Db"))
            {
                DbName = dbName;
            }
            else {
                DbName = $"{dbName}.db";
            }
            conn = new SqliteConnection($"Data Source = {DbName}");
            conn.Open();
        }
        public int Write(List<Dictionary<string,string>> keyValues,string tabName) {
            int result = 0;
            var data = keyValues.FirstOrDefault();
            if (data == null) return result;
            var createSql = BuildTable(data,tabName);
            var insertSql = BuildInsertSql(keyValues,createSql.insertSql);
            try
            {
                using (SqliteCommand cmd = new SqliteCommand(createSql.createSql, conn))
                {
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = insertSql;
                    result = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return result;

        }
        private string BuildInsertSql(List<Dictionary<string, string>> keyValues, string tabName) {

            StringBuilder sql = new StringBuilder();
            foreach (var data in keyValues) {
                sql.AppendLine($"{tabName} {Guid.NewGuid().ToString()} , {DateTime.Now}");
                foreach (var item in data) {
                    sql.Append($", {item.Value}");
                }
                sql.Append(");");
            }
            return sql.ToString();

        }
        private (string createSql,string insertSql) BuildTable(Dictionary<string, string> data, string tabName) {

            StringBuilder sql = new StringBuilder($" CREATE TABLE IF NOT EXISTS  {tabName}( id VARCHAR(32) PRIMARY KEY  NOT NULL, createdTime date ");
            StringBuilder iSql = new StringBuilder($" INSERT INTO {tabName}( id , createdTime");
            foreach (var d in data) {
                var name = $"d_{d.Key}";
                sql.AppendLine($", {name} TEXT");
                iSql.Append($", {name}");
            }
            sql.AppendLine(");");
            iSql.AppendLine(") VALUES (");
            return (sql.ToString(),iSql.ToString());
        }
        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                conn.Close();
                conn.Dispose();
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~SqliteWriter()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
