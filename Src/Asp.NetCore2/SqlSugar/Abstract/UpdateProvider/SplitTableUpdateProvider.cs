﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitTableUpdateProvider<T> where T : class, new()
    {
        public SqlSugarProvider Context;
        public UpdateableProvider<T> updateobj;

        public IEnumerable<SplitTableInfo> Tables { get; set; }

        public int ExecuteCommandWithOptLock(bool isThrowError = false) 
        {
            var updates=updateobj.UpdateObjs;
            var tableName = this.Context.SplitHelper(updates.FirstOrDefault()).GetTableName();
            var names=updateobj.UpdateBuilder.DbColumnInfoList.Select(it => it.DbColumnName).Distinct().ToArray();
            return this.Context.Updateable(updates).AS(tableName)
                .UpdateColumns(names).ExecuteCommandWithOptLock(isThrowError);
        }
        public int ExecuteCommand()
        {
            if (this.Context.Ado.Transaction == null)
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result = _ExecuteCommand();
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception)
                {
                    this.Context.Ado.RollbackTran();
                    throw;
                }
            }
            else 
            {
                return _ExecuteCommand();
            }
        }
        public async Task<int> ExecuteCommandWithOptLockAsync(bool isThrowError = false)
        {
            var updates = updateobj.UpdateObjs;
            var tableName = this.Context.SplitHelper(updates.FirstOrDefault()).GetTableName();
            var names = updateobj.UpdateBuilder.DbColumnInfoList.Select(it => it.DbColumnName).Distinct().ToArray();
            return await this.Context.Updateable(updates).AS(tableName)
                .UpdateColumns(names).ExecuteCommandWithOptLockAsync(isThrowError);
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (this.Context.Ado.Transaction == null)
            {
                try
                {
                    this.Context.Ado.BeginTran();
                    var result = await _ExecuteCommandAsync();
                    this.Context.Ado.CommitTran();
                    return result;
                }
                catch (Exception)
                {
                    this.Context.Ado.RollbackTran();
                    throw;
                }
            }
            else
            {
                return await _ExecuteCommandAsync();
            }
        }
        private int _ExecuteCommand()
        {
            var result = 0;
            var sqlobj = updateobj.ToSql();

            foreach (var item in Tables)
            {
                var newsqlobj = GetSqlObj(sqlobj, item.TableName);
                result += this.Context.Ado.ExecuteCommand(newsqlobj.Key, newsqlobj.Value);
            }
            return result;
        }

        private async Task<int> _ExecuteCommandAsync()
        {
            var result = 0;
            var sqlobj = updateobj.ToSql();
            foreach (var item in Tables)
            {
                var newsqlobj = GetSqlObj(sqlobj, item.TableName);
                result += await this.Context.Ado.ExecuteCommandAsync(newsqlobj.Key, newsqlobj.Value);
            }
            return result;
        }

        private KeyValuePair<string, List<SugarParameter>> GetSqlObj(KeyValuePair<string, List<SugarParameter>> keyValuePair, string asName)
        {
            List<SugarParameter> pars = new List<SugarParameter>();
            string sql = keyValuePair.Key;
            if (keyValuePair.Value != null)
            {
                pars = keyValuePair.Value.Select(it => new SugarParameter(it.ParameterName, it.Value)).ToList();
            }
            sql = Regex.Replace(sql, updateobj.EntityInfo.DbTableName, asName, RegexOptions.IgnoreCase);
            return new KeyValuePair<string, List<SugarParameter>>(sql, pars);
        }
    }
}
