﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqliteUpdateBuilder : UpdateBuilder
    {
        public override string ReSetValueBySqlExpListType { get; set; }="sqlite";
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine(string.Join("\r\n", groupList.Select(t =>
            {
                var updateTable = string.Format("UPDATE {0} SET", base.GetTableNameStringNoWith);
                var setValues = string.Join(",", t.Where(s => !s.IsPrimarykey).Where(s=> OldPrimaryKeys==null||!OldPrimaryKeys.Contains(s.DbColumnName)).Select(m => GetOracleUpdateColums(i,m,false)).ToArray());
                var pkList = t.Where(s => s.IsPrimarykey).ToList();
                if (this.IsWhereColumns&& this.PrimaryKeys?.Any()==true) 
                {
                    var whereColumns = pkList.Where(it => this.PrimaryKeys?.Any(p => p.EqualCase(it.PropertyName) || p.EqualCase(it.DbColumnName))==true).ToList();
                    if (whereColumns.Any()) 
                    {
                        pkList = whereColumns;
                    }
                }
                List<string> whereList = new List<string>();
                foreach (var item in pkList)
                {
                    var isFirst = pkList.First() == item;
                    var whereString = "";
                    whereString += GetOracleUpdateColums(i,item,true);
                    whereList.Add(whereString);
                }
                i++;
                return string.Format("{0} {1} WHERE {2};", updateTable, setValues, string.Join(" AND", whereList));
            }).ToArray()));
            return sb.ToString();
        }

        private string GetOracleUpdateColums(int i,DbColumnInfo m,bool iswhere)
        {
            var result= string.Format("\"{0}\"={1}", m.DbColumnName.ToUpper(),base.GetDbColumn(m, FormatValue(i,m.DbColumnName,m.Value,iswhere)));
            if (iswhere && m.Value == null)
            {
                result = result.Replace("=NULL", " is NULL ");
            }
            return result;
        }

        public  object FormatValue(int i,string name,object value,bool iswhere)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.DateType && iswhere == false)
                {
                    var date = value.ObjToDate();
                    if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
                    {
                        date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                    }
                    if (this.Context.CurrentConnectionConfig?.MoreSettings?.DisableMillisecond == true)
                    {
                        return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                    else 
                    {
                        return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                    }
                }
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    return GetDateTimeOffsetString(value);
                }
                else if (type == UtilConstants.DateType && iswhere) 
                {
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
                }
                else if (type.IsEnum())
                {
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        return value.ToSqlValue();
                    }
                    else
                    {
                        return Convert.ToInt64(value);
                    }
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "'" + value.ToString() + "'";
                }
            }
        }

        private object GetDateTimeOffsetString(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
            {
                date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
            }
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "'";
        }
    }
}
