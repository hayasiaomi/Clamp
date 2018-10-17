using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.LSRestaurant.Factories
{
    public class SqlFactory
    {
        /// <summary>
        /// 获取model的表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTableName<T>()
        {
            string table = string.Empty;            //表名
            Type type = typeof(T);
            var attrs = System.Attribute.GetCustomAttributes(type);
            foreach (System.Attribute attr in attrs)
            {
                if (attr is TableAttribute)
                {
                    TableAttribute a = (TableAttribute)attr;
                    table = a.Name;
                    break;
                }
            }
            return table;
        }

        /// <summary>
        /// 获取Table表的列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetTableColumn<T>(string colPrefix = "")
        {
            List<string> member = new List<string>(); //全部列名
            Type type = typeof(T);
            foreach (PropertyInfo item in type.GetProperties())
            {
                string name = $"{colPrefix}.{item.Name}";
                member.Add(name);
            }
            return member;
        }

        /// <summary>
        /// 获取select语句(带where条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetSelect<T>()
        {
            string sqlStr = String.Empty;
            string table = string.Empty;            //表名
            List<string> member = new List<string>(); //全部列名
            Type type = typeof(T);
            var attrs = System.Attribute.GetCustomAttributes(type);
            foreach (System.Attribute attr in attrs)
            {
                if (attr is TableAttribute)
                {
                    TableAttribute a = (TableAttribute)attr;
                    table = a.Name;
                    break;
                }
            }
            foreach (PropertyInfo item in type.GetProperties())
            {
                string name = item.Name;
                member.Add(name);
            }
            sqlStr = $"select {string.Join(",", member)} from {table} ";
            return sqlStr;
        }



        /// <summary>
        /// 获取select语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public static string GetSelect<T>(string whereStr)
        {
            string sqlStr = GetSelect<T>();
            if (!string.IsNullOrWhiteSpace(sqlStr) && !string.IsNullOrWhiteSpace(whereStr))
            {
                sqlStr = $"{sqlStr} {whereStr}";
            }
            return sqlStr;
        }

        /// <summary>
        /// 生成Insert语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetInsert<T>(T model)
        {
            string sqlStr = String.Empty;
            if (model != null)
            {
                string table = string.Empty;            //表名
                List<string> member = new List<string>(); //全部列名
                List<string> member_value = new List<string>(); //全部的值
                Type type = typeof(T);
                var attrs = System.Attribute.GetCustomAttributes(type);
                foreach (System.Attribute attr in attrs)
                {
                    if (attr is TableAttribute)
                    {
                        TableAttribute a = (TableAttribute)attr;
                        table = a.Name;
                        break;
                    }
                }

                foreach (PropertyInfo item in type.GetProperties())
                {
                    string value = string.Empty;

                    if (typeof(string).IsAssignableFrom(item.PropertyType))
                    {
                        string temp = item.GetValue(model, null) == null ? "" : item.GetValue(model, null).ToString().Replace("'", "''");
                        value = $"'{temp}'";
                    }
                    //else if (typeof(bool).IsAssignableFrom(info.PropertyType))
                    //    info.SetValue(dto, Convert.ToBoolean(model.Value), null);
                    else if (typeof(DateTime).IsAssignableFrom(item.PropertyType))
                    {
                        DateTime time = (DateTime)item.GetValue(model, null);
                        value = $"'{time.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    }
                    else
                    {
                        value = item.GetValue(model, null).ToString();
                    }
                    member_value.Add(value);
                    member.Add(item.Name);
                }
                sqlStr = $"insert into {table} ({string.Join(",", member)}) values ({string.Join(",", member_value)}) ";

            }
            return sqlStr;
        }


        /// <summary>
        /// 生成Update语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetUpdate<T>(T model)
        {
            string sqlStr = String.Empty;
            if (model != null)
            {
                string table = string.Empty;            //表名
                Type type = typeof(T);
                var attrs = System.Attribute.GetCustomAttributes(type);
                foreach (System.Attribute attr in attrs)
                {
                    if (attr is TableAttribute)
                    {
                        TableAttribute a = (TableAttribute)attr;
                        table = a.Name;
                        break;
                    }
                }
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendFormat("update {0} set ", table);
                foreach (PropertyInfo item in type.GetProperties())
                {
                    string value = string.Empty;

                    if (typeof(string).IsAssignableFrom(item.PropertyType))
                    {
                        string temp = item.GetValue(model, null) == null ? "" : item.GetValue(model, null).ToString().Replace("'", "''");
                        value = $"'{temp}'";
                    }
                    //else if (typeof(bool).IsAssignableFrom(info.PropertyType))
                    //    info.SetValue(dto, Convert.ToBoolean(model.Value), null);
                    else if (typeof(DateTime).IsAssignableFrom(item.PropertyType))
                    {
                        DateTime time = (DateTime)item.GetValue(model, null);
                        value = $"'{time.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    }
                    else
                    {
                        value = item.GetValue(model, null).ToString();
                    }
                    sqlBuilder.AppendFormat(" {0}={1},", item.Name, value);
                }
                sqlStr = sqlBuilder.ToString().TrimEnd(',');
            }
            return sqlStr;
        }

        /// <summary>
        /// 生成Update语句(带where条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public static string GetUpdate<T>(T model, string whereStr)
        {
            string sqlStr = GetUpdate(model);
            if (!string.IsNullOrWhiteSpace(sqlStr) && !string.IsNullOrWhiteSpace(whereStr))
            {
                sqlStr = $"{sqlStr} {whereStr} ";
            }
            return sqlStr;
        }

    }
}
