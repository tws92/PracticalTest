using PracticalTest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace PracticalTest.Extension
{
    public static class Extensions
    {
        public static DataTable ToDataTable(this List<int> values)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            if (values != null)
            {
                foreach (var val in values)
                {
                    table.Rows.Add(val);
                }
            }

            return table;
        }
    }
}