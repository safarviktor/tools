using System;
using System.Linq;

namespace VClipboardHelper
{
    internal static class ArcGISFieldsToCSharpClassesWithMvvmToolkit
    {
        /*
****************************** INPUT ******************************
Fields:
OBJECTID ( type: esriFieldTypeOID, alias: OBJECTID, editable: false, nullable: false, defaultValue: null, modelName: OBJECTID )
Lys ( type: esriFieldTypeInteger, alias: Lys, editable: true, nullable: true, defaultValue: null, modelName: Lys )
lysfarge ( type: esriFieldTypeInteger, alias: lysfarge, editable: true, nullable: true, defaultValue: null, modelName: lysfarge )

****************************** OUTPUT ******************************
internal partial class YOURCLASS : ObservableObject
{

    [ObservableProperty]
    private string _navn;

    [ObservableProperty]
    private string _lysKarakter;

}

         */

        public static string Execute(string input)
        {
            var rows = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            if (rows[0].StartsWith("Fields:"))
            {
                rows = rows.Skip(1).ToArray();
            }

            var viewModelResult = "internal partial class YOURCLASS : ObservableObject\r\n{";
            var modelResult = string.Empty;
            var filedConstantsResult = string.Empty;

            foreach (var row in rows)
            {
                viewModelResult += GetObservablePropertyFromRow(row) + Environment.NewLine;
                modelResult += GetPropertyFromRow(row) + Environment.NewLine;
                filedConstantsResult += GetFieldConstantFromRow(row) + Environment.NewLine;
            }

            viewModelResult += "}";

            return viewModelResult
                + Environment.NewLine
                + "// =========== regular properties =========== "
                + Environment.NewLine
                + modelResult
                + Environment.NewLine
                + "// =========== field constants =========== "
                + Environment.NewLine
                + filedConstantsResult;
        }

        private static string GetFieldConstantFromRow(string row)
        {
            var splits = row.Split(' ');
            var name = splits[0];
            return $@"public const string {name} = ""{name}"";";
        }

        private static string GetPropertyFromRow(string row)
        {
            var splits = row.Split(' ');

            var name = splits[0];
            var gisType = splits[3].Replace(",", "");
            var nullableChar = row.Contains("nullable: true") ? "?" : "";
            var dotnetType = GisTypeToDotNetType(gisType);

            if (row.Contains("Coded Values"))
            {
                dotnetType = "CodedValues";
            }

            return $@"public {dotnetType}{nullableChar} {name} {{ get; set; }}";
        }

        private static string GetObservablePropertyFromRow(string row)
        {
            var splits = row.Split(' ');

            var name = splits[0];
            var gisType = splits[3].Replace(",", "");
            var nullableChar = row.Contains("nullable: true") ? "?" : "";
            var dotnetType = GisTypeToDotNetType(gisType);

            if (row.Contains("Coded Values"))
            {
                dotnetType = "CodedValues";
            }

            return $@"
[ObservableProperty]
private {dotnetType}{nullableChar} _{name};
";
        }

        private static object GisTypeToDotNetType(string gisType)
        {
            switch (gisType)
            {
                case "esriFieldTypeOID":
                case "esriFieldTypeInteger":
                    return "int";
                case "esriFieldTypeDouble":
                    return "double";
                case "esriFieldTypeGUID":
                case "esriFieldTypeGlobalID":
                    return "Guid";
                case "esriFieldTypeDate":
                    return "DateTimeOffset";
                default:
                    return "string";
            }
        }
    }
}
