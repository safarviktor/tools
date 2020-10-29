using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VClipboardHelper
{
    public static class CSharpModelBuilder
    {
        public const string SqlTableInfoIdentifier = "Column_name	Type	Computed";

        private static readonly string[] SqlServerTypes = { "bigint", "binary", "bit", "char", "date", "datetime", "datetime2", "datetimeoffset", "decimal", "filestream", "float", "geography", "geometry", "hierarchyid", "image", "int", "money", "nchar", "ntext", "numeric", "nvarchar", "real", "rowversion", "smalldatetime", "smallint", "smallmoney", "sql_variant", "text", "time", "timestamp", "tinyint", "uniqueidentifier", "varbinary", "varchar", "xml" };
        private static readonly string[] CSharpTypes = { "long", "byte[]", "bool", "char", "DateTime", "DateTime", "DateTime", "DateTimeOffset", "decimal", "byte[]", "double", "Microsoft.SqlServer.Types.SqlGeography", "Microsoft.SqlServer.Types.SqlGeometry", "Microsoft.SqlServer.Types.SqlHierarchyId", "byte[]", "int", "decimal", "string", "string", "decimal", "string", "Single", "byte[]", "DateTime", "short", "decimal", "object", "string", "TimeSpan", "byte[]", "byte", "Guid", "byte[]", "string", "string" };

        public static string ConvertSqlServerFormatToCSharp(string typeName)
        {
            var index = Array.IndexOf(SqlServerTypes, typeName);

            return index > -1
                ? CSharpTypes[index]
                : "object";
        }

        public static string GetCsharpModel(string mainInput)
        {
            var result = string.Empty;

            string[] tableColumns = mainInput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var columnDefinition = tableColumns.First(x => !x.StartsWith(SqlTableInfoIdentifier));
            var columnName = columnDefinition.Split('\t').First().Split('_');
            var tablePrefix = columnName.Length > 1
                ? columnName[0] + "_"
                : string.Empty;

            foreach (var tableColumn in tableColumns)
            {
                if (tableColumn.StartsWith(SqlTableInfoIdentifier))
                    continue;

                result += $"public {GetCsharpModelType(tableColumn)} {GetCsharpModelPropertyName(tableColumn, tablePrefix)} {{ get; set; }}" + Environment.NewLine;
            }

            return result;
        }

        private static string GetCsharpModelPropertyName(string tableColumn, string tablePrefix)
        {
            var splits = tableColumn.Split('\t');
            var columnName = splits[0];
            if (tablePrefix.Length > 0 && columnName.StartsWith(tablePrefix))
            {
                columnName = columnName.Substring(tablePrefix.Length);
            }

            return columnName.Substring(0, 1).ToUpper() + columnName.Substring(1);
        }

        private static string GetCsharpModelType(string tableColumn)
        {
            var splits = tableColumn.Split('\t');
            var sqlType = splits[1];
            var nullable = splits[6];
            var dotnetType = ConvertSqlServerFormatToCSharp(sqlType);
            string nullableSuffix = nullable == "yes" ? "?" : string.Empty;
            if (nullableSuffix != string.Empty && !TypeCanBeNullable(dotnetType))
            {
                nullableSuffix = string.Empty;
            }

            return $"{dotnetType}{nullableSuffix}";
        }

        private static bool TypeCanBeNullable(string dotnetType)
        {
            return dotnetType != "string";
        }
    }
}
