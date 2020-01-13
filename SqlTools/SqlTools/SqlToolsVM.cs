using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using SqlTools.Models;

namespace SqlTools
{
    public class SqlToolsVM : BindableBase
    {
        private string _inputSql;
        private string _resultText;
        private List<CheckBoxEntry<OperationsEnum>> _operations;
        private List<CheckBoxEntry<string>> _dataSources;
        private bool _includeIdentity;

        public SqlToolsVM()
        {
            Operations = GetOperations(); 
            DataSources = GetDataSources();
            GoCommand = new DelegateCommand(Go);
            SelectDefault();
        }

        private void SelectDefault()
        {
            Operations.First().IsSelected = true;
            DataSources.First().IsSelected = true;
        }

        private static List<CheckBoxEntry<string>> GetDataSources()
        {
            var sources = new List<CheckBoxEntry<string>>();
            foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
            {
                sources.Add(new CheckBoxEntry<string>() {Name = cs.Name, Value = cs.ConnectionString});
            }
            return sources;
        }

        private static List<CheckBoxEntry<OperationsEnum>> GetOperations()
        {
            var ops = new List<CheckBoxEntry<OperationsEnum>>();

            foreach (var op in Enum.GetValues(typeof(OperationsEnum)))
            {
                ops.Add(new CheckBoxEntry<OperationsEnum>() { Name = op.ToString(), Value = (OperationsEnum)op});
            }
            
            return ops;
        }

        public bool IncludeIdentity
        {
            get => _includeIdentity;
            set
            {
                _includeIdentity = value;
                RaisePropertyChanged(nameof(IncludeIdentity));
            }
        }

        public string InputSql
        {
            get => _inputSql;
            set
            {
                _inputSql = value;
                RaisePropertyChanged(nameof(InputSql));
            }
        }

        public string ResultText
        {
            get => _resultText;
            set
            {
                _resultText = value;
                RaisePropertyChanged(nameof(ResultText));
            }
        }

        private void Go()
        {
            try
            {
                if (InputSql.IsNotNullOrEmpty())
                {
                    var tableName = FindMainTableName(InputSql);

                    var connectionString = DataSources.FirstOrDefault(x => x.IsSelected);
                    if (connectionString == null)
                    {
                        MessageBox.Show("Please select a data source.");
                        return;
                    }

                    var data = new DataTable();
                    var schema = new DataTable();

                    using (var connection = new SqlConnection(connectionString.Value))
                    {
                        connection.Open();
                        var command = new SqlCommand(InputSql, connection);

                        using (var reader = command.ExecuteReader())
                        {
                            data.Load(reader);
                        }

                        using (var reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                        {
                            schema = reader.GetSchemaTable();
                        }

                        connection.Close();
                    }

                    ResultText = GenerateStatements(data, Operations.First(x => x.IsSelected).Value, tableName, schema);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string FindMainTableName(string inputSql)
        {
            var searchpattern = "FROM ";
            var tableBegin = inputSql.IndexOf(searchpattern, StringComparison.CurrentCultureIgnoreCase) + searchpattern.Length;
            var tableEnd = inputSql.IndexOf(" ", tableBegin);

            if (tableEnd < 0)
            {
                tableEnd = inputSql.IndexOf(Environment.NewLine, tableBegin);
            }
            else
            {
                var potentialTablename = inputSql.Substring(tableBegin, tableEnd - tableBegin);
                if (potentialTablename.Contains(Environment.NewLine))
                {
                    tableEnd = inputSql.IndexOf(Environment.NewLine, tableBegin);
                }
            }

            if (tableEnd < 0)
            {
                tableEnd = inputSql.Length; // assuming table is the last word in the statement
            }

            return inputSql.Substring(tableBegin, tableEnd - tableBegin);
        }
        
        private string GenerateStatements(DataTable data, OperationsEnum operation, string mainTable, DataTable schema)
        {
            var identityColumn = GetIdentityColumnName(schema);

            var statementConstant = GetSingleStatementBeginning(operation, mainTable);
            statementConstant += GetStatementColumns(data.Columns, identityColumn);
            statementConstant += GetSingleStatementEnding(operation);
            var valuesBeginning = GetSingleStatementValueBeginning(operation);
            var valuesEnding = GetSingleStatementValueEnding(operation);

            var statement = IncludeIdentity 
                ? $"SET IDENTITY_INSERT {mainTable} ON" + Environment.NewLine
                : string.Empty;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                statement += statementConstant;
                statement += valuesBeginning;
                statement += GetSingleStatementValues(data.Rows[i], data.Columns, identityColumn);
                statement += valuesEnding;
                statement += Environment.NewLine;
            }

            return IncludeIdentity 
                ? $"{statement}SET IDENTITY_INSERT {mainTable} OFF"
                : statement;
        }

        private string GetIdentityColumnName(DataTable schema)
        {
            for (int i = 0; i < schema.Rows.Count; i++)
            {
                var columnInfo = schema.Rows[i];
                if ((bool) columnInfo["IsIdentity"])
                {
                    return (string)columnInfo[0];
                }
            }

            return string.Empty;
        }

        private string GetSingleStatementValueEnding(OperationsEnum operation)
        {
            switch (operation)
            {
                case OperationsEnum.Insert:
                    return ") ";
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetSingleStatementValueBeginning(OperationsEnum operation)
        {
            switch (operation)
            {
                case OperationsEnum.Insert:
                    return " VALUES (";
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetSingleStatementValues(DataRow dataRow, DataColumnCollection columns, string identityColumn)
        {
            var statement = string.Empty;

            for (int i = 0; i < columns.Count; i++)
            {
                if (!IncludeIdentity && identityColumn.EqualsIgnoreCase(columns[i].ColumnName))
                {
                    continue;
                }

                statement += GetFormattedValueFromRowItem(dataRow, columns, i) + ", ";
            }

            return statement.Substring(0, statement.Length - 2);
        }

        private string GetFormattedValueFromRowItem(DataRow dataRow, DataColumnCollection columns, int index)
        {
            if (dataRow[index] is DBNull)
                return "NULL";

            if (columns[index].DataType == typeof(string))
            {
                var value = (string)dataRow[index];
                value = value.Replace("'", "''");
                return $"N'{value}'";
            }

            if (columns[index].DataType == typeof(Guid))
            {
                var value = (Guid)dataRow[index];
                return $"N'{value}'";
            }

            if (columns[index].DataType == typeof(DateTime))
                return $"'{Convert.ToDateTime(dataRow[index]):yyyy-MM-dd HH:mm:ss:fff}'";

            if (columns[index].DataType == typeof(bool))
                return Convert.ToBoolean(dataRow[index]) 
                    ? "1"
                    : "0";

            if (columns[index].DataType == typeof(byte[]))
                return $"0x{ByteArrayToString((byte[])dataRow[index])}";

            return $"{dataRow[index]}";
        }

        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private string GetSingleStatementEnding(OperationsEnum operation)
        {
            switch (operation)
            {
                case OperationsEnum.Insert:
                    return ") ";
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetStatementColumns(DataColumnCollection dataColumns, string identityColumn)
        {
            var columnStatement = string.Empty;
            foreach (DataColumn col in dataColumns)
            {
                if (!IncludeIdentity && identityColumn.EqualsIgnoreCase(col.ColumnName))
                {
                    continue;
                }

                columnStatement += col.ColumnName + ", ";
            }

            return columnStatement.Substring(0, columnStatement.Length - 2);
        }

        private static string GetSingleStatementBeginning(OperationsEnum operation, string mainTable)
        {
            switch (operation)
            {
                case OperationsEnum.Insert:
                    return $"INSERT INTO {mainTable}(";
                default:
                    throw new NotImplementedException();
            }
        }


        public ICommand GoCommand { get; private set; }

        public List<CheckBoxEntry<OperationsEnum>> Operations
        {
            get => _operations;
            set
            {
                _operations = value;
                RaisePropertyChanged(nameof(Operations));
            }
        }

        public List<CheckBoxEntry<string>> DataSources
        {
            get => _dataSources;
            set
            {
                _dataSources = value;
                RaisePropertyChanged(nameof(DataSources));
            }
        }
    }
}