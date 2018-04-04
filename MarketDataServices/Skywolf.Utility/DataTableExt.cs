using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Prcm.OptimusRisk.Utility
{
    public interface IColumn<T>
    {
        string Name { get; }
        Type Type { get; }

        object Eval(T t);
    }

    public class Column<T, V> : IColumn<T>
    {
        public Column(Func<T, V> func, string name)
        {
            this.func = func;
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return typeof (V); }
        }

        public object Eval(T t)
        {
            return func(t);
        }

        private readonly Func<T, V> func;
        private readonly string name;
    }

    public class DataTableConversion<T> : IEnumerable<IColumn<T>>
    {
        public DataTableConversion()
        {
            cols = new List<IColumn<T>>();
        }

        public void Add<V>(Expression<Func<T, V>> expr)
        {
            var body = expr.Body as MemberExpression;

            if (body == null)
                throw new InvalidOperationException(
                    "Cannot determine column name unless the lambda is of the form x => x.Member");

            Add(expr.Compile(), body.Member.Name);
        }

        public void Add<V>(Func<T, V> func, string colName)
        {
            cols.Add(new Column<T, V>(func, colName));
        }

        public void Add(IColumn<T> col)
        {
            cols.Add(col);
        }

        public DataTable Convert(IEnumerable<T> data)
        {
            var n = cols.Count;
            var dcols = new DataColumn[n];

            var t = new DataTable();
            for (var i = 0; i < n; i++)
                dcols[i] = t.Columns.Add(cols[i].Name, cols[i].Type);

            foreach (var e in data)
            {
                var r = t.Rows.Add();
                for (var i = 0; i < n; i++)
                    r[dcols[i]] = cols[i].Eval(e);
            }

            return t;
        }

        public DataTable Map(IEnumerable<T> data, string name)
        {
            var t = Convert(data);
            t.TableName = name;
            return t;
        }

        public IEnumerator<IColumn<T>> GetEnumerator()
        {
            return cols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<IColumn<T>> cols;
    }

    public static class DataTableExt
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> self)
        {
            return self.ToDataTable(typeof(T).Name);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> self, string name)
        {
            var conv = new DataTableConversion<T>();

            foreach (var p in typeof(T).GetProperties())
                conv.Add(new Column<T>(p));

            return conv.Map(self, name);
        }

        private class Column<T> : IColumn<T>
        {
            internal Column(PropertyInfo p)
            {
                this.p = p;
            }

            public string Name
            {
                get { return p.Name; }
            }

            public Type Type
            {
                get
                {
                    var t = p.PropertyType;
                    if (t.IsGenericType)
                        if (t.GetGenericTypeDefinition() == typeof(Nullable<>))
                            t = t.GetGenericArguments()[0];
                    return t;
                }
            }

            public object Eval(T t)
            {
                var val = p.GetValue(t, null);
                return val != null ? val : DBNull.Value;
            }

            private readonly PropertyInfo p;
        }
    }
}
