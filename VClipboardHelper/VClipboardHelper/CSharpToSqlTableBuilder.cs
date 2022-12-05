using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VClipboardHelper
{
    public static class CSharpToSqlTableBuilder
    {
        public static string CreateSqlTable(string mainInput)
        {
            var classRows = mainInput.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var definition = classRows.First();
            definition = definition.Replace("public class ", "");
            definition = definition.Replace("internal class ", "");
            definition = definition.Replace("protected class ", "");
            definition = definition.Trim();

            var classNameEnd = definition.IndexOf(" ");
            if (classNameEnd == -1)
                classNameEnd = definition.Length;

            var className = definition.Substring(0, classNameEnd);
            if (className.Contains(":"))
            {
                className = className.Substring(0, className.IndexOf(":"));
            }

            var columns = new List<string>();

            foreach (var propertyRow in classRows.Skip(1))
            {
                var rowContent = propertyRow.Trim();
                
                if (rowContent.StartsWith("public"))
                {
                    var typeStart = rowContent.IndexOf(" ");
                    var typeEnd = rowContent.IndexOf(" ", typeStart + 1);
                    var propType = rowContent.Substring(typeStart, typeEnd - typeStart);
                    
                    var nameStart = rowContent.IndexOf(" ", typeEnd);
                    var nameEnd = rowContent.IndexOf(" ", nameStart + 1);
                    var name = rowContent.Substring(nameStart, nameEnd - nameStart);

                    columns.Add($"    [{name.Trim()}] {ToSqlType(propType)} null");
                }
            }

            return $@"
DECLARE @{className} TABLE
(
{string.Join($"{Environment.NewLine},", columns)}
)
";          
        }

        private static string ToSqlType(string propType)
        {
            propType = propType.Trim();
            propType = propType.Replace("?", "");

            switch (propType)
            {
                case "decimal":
                    return "decimal(18,2)";
                case "string":
                    return "nvarchar(max)";
                case "Guid":
                    return "uniqueidentifier";
                case "Byte":
                    return "tinyint";
                case "Int32":
                    return "int";
                case "long":
                case "Int64":
                    return "bigint";
                case "bool":
                case "Boolean":
                    return "bit";

                default:
                    return propType;
            }
        }
    }
}
