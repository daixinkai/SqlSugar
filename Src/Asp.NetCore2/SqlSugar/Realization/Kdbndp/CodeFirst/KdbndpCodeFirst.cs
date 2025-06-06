﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class KdbndpCodeFirst : CodeFirstProvider
    {
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = GetTableName(entityInfo);
            //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Use Code First ,The primary key must not exceed 1");
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.HasValue())
            {
                foreach (var item in entityInfo.Columns.Where(it=>it.IsIgnore==false))
                {
                    DbColumnInfo dbColumnInfo = this.EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
                if (entityInfo.IsCreateTableFiledSort)
                {
                    columns = columns.OrderBy(c => c.CreateTableFieldSort).ToList();
                }
            }
            columns = columns.OrderBy(it => it.IsPrimarykey ? 0 : 1).ToList();
            this.Context.DbMaintenance.CreateTable(tableName, columns,true);
        }
        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var propertyType = UtilMethods.GetUnderType(item.PropertyInfo);
            var result = new DbColumnInfo()
            {
                TableId = entityInfo.Columns.IndexOf(item),
                DbColumnName = item.DbColumnName.HasValue() ? item.DbColumnName : item.PropertyName,
                IsPrimarykey = item.IsPrimarykey,
                IsIdentity = item.IsIdentity,
                TableName = tableName,
                IsNullable = item.IsNullable,
                DefaultValue = item.DefaultValue,
                ColumnDescription = item.ColumnDescription,
                Length = item.Length,
                CreateTableFieldSort = item.CreateTableFieldSort,
                DecimalDigits = item.DecimalDigits,
                Scale = item.DecimalDigits
            };
            GetDbType(item, propertyType, result);
            if (result.DataType.Equals("varchar", StringComparison.CurrentCultureIgnoreCase) && result.Length == 0)
            {
                result.Length = 1;
            }
            if (result.DataType.EqualCase("timestamp")&&this.Context.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.SqlServer) 
            {
                result.DataType = "datetime";
            }
            if (IsSqlServerModel())
            {
                if (result.DataType.EqualCase("int4"))
                {
                    result.DataType = "int";
                }
                if (result.DataType.EqualCase("int8"))
                {
                    result.DataType = "bigint";
                }
            }
            return result;
        }

        protected override void ConvertColumns(List<DbColumnInfo> dbColumns)
        {
            foreach (var item in dbColumns)
            {
                if (item.DataType == "DateTime")
                {
                    item.Length = 0;
                }
            }
        }

        protected override void ChangeKey(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
            if (!item.IsPrimarykey)
                this.Context.DbMaintenance.DropConstraint(tableName,null);
            if (item.IsPrimarykey)
                this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
        }

        private bool IsSqlServerModel()
        {
            return this.Context.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.SqlServer;
        }
    }
}
