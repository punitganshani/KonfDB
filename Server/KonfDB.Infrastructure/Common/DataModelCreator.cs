#region License and Product Information

// 
//     This file 'DataModelCreator.cs' is part of KonfDB application - 
//     a project perceived and developed by Punit Ganshani.
// 
//     KonfDB is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     KonfDB is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with KonfDB.  If not, see <http://www.gnu.org/licenses/>.
// 
//     You can also view the documentation and progress of this project 'KonfDB'
//     on the project website, <http://www.konfdb.com> or on 
//     <http://www.ganshani.com/applications/konfdb>

#endregion

using System;
using System.Data;
using System.IO;
using System.Text;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Infrastructure.Common
{
    public class DataModelCreator
    {
        /// <summary>
        ///     craetes a entity .cs file having fields of type and name same as columns in data table passed to it
        /// </summary>
        /// <param name="dt">datatable corresponding to which entity required</param>
        /// <param name="fileName">full file name including directory path </param>
        public static void DataTable2Entity(DataTable dt, string className)
        {
            StringBuilder sb = new StringBuilder();
            StreamWriter tw = null;
            FileInfo file = null;

            try
            {
                if (dt == null)
                    throw new ArgumentException("parameter dt of type datatable cannot be null");
                if (string.IsNullOrEmpty(className))
                    throw new ArgumentException(
                        "parameter filename of type string cannot be null enter full file path along with name");

                file = new FileInfo(className + ".cs");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using KonfDB.Infrastructure.DAL;");
                sb.AppendLine("public class " + className + ": IDataModel");
                sb.AppendLine("{");


                tw = file.CreateText();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn dc = dt.Columns[i];
                    sb.AppendLine("      public " +
                                  dc.DataType +
                                  " " +
                                  dc.ColumnName +
                                  " {get; set;}");
                }

                sb.AppendLine("///<Summary>Mapping List for " + className + ".<Summary>");
                sb.AppendLine("public static List<KeyValuePair<string, string>> CreateMapping() {");
                sb.AppendLine("//TODO: This may require modification if the columns names are not same in");
                sb.AppendLine("//TODO: Datatable and Enitity");
                sb.AppendLine(
                    "//mappings.Add(new KeyValuePair<string, string>(\"DataTable Column Name\", \"Entity Column Name\"));");

                sb.AppendLine(
                    "   List<KeyValuePair<string, string>> mappings = new List<KeyValuePair<string, string>>();");

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.AppendLine("    mappings.Add(new KeyValuePair<string, string>(\"" + dt.Columns[i].ColumnName +
                                  "\", \"" + dt.Columns[i].ColumnName + "\"));");
                }

                sb.AppendLine("    return mappings;");
                sb.AppendLine("}");


                sb.AppendLine("}");
                tw.WriteLine(sb.ToString());
            }
            catch (Exception ex)
            {
                CurrentContext.Default.Log.Error("An Exception Occured", ex);
            }
            finally
            {
                if (tw != null)
                    tw.Close();
            }
        }
    }
}