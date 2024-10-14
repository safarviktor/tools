using System;
using System.Linq;

namespace VClipboardHelper
{
	internal static class ArcGISFieldsToCSharpClasses
	{

        /*
****************************** INPUT ******************************
Fields:
OBJECTID ( type: esriFieldTypeOID, alias: OBJECTID, editable: false, nullable: false, defaultValue: null, modelName: OBJECTID )
Lys ( type: esriFieldTypeInteger, alias: Lys, editable: true, nullable: true, defaultValue: null, modelName: Lys )
lysfarge ( type: esriFieldTypeInteger, alias: lysfarge, editable: true, nullable: true, defaultValue: null, modelName: lysfarge )

****************************** OUTPUT ******************************
private int _OBJECTID;
public int OBJECTID
{
	get { return _OBJECTID; }
    set { SetProperty(ref _OBJECTID, value); }
}

private int? _Lys;
public int? Lys
{
	get { return _Lys; }
    set { SetProperty(ref _Lys, value); }
}

private int? _lysfarge;
public int? lysfarge
{
	get { return _lysfarge; }
    set { SetProperty(ref _lysfarge, value); }
}

// =========== regular properties =========== 
public int OBJECTID { get; set; }
public int? Lys { get; set; }
public int? lysfarge { get; set; }

// =========== field constants =========== 
public const string OBJECTID = "OBJECTID";
public const string Lys = "Lys";
public const string lysfarge = "lysfarge";


		 */

        public static string Execute(string input)
		{
			var rows = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			if (rows[0].StartsWith("Fields:")) 
			{
				rows = rows.Skip(1).ToArray();
			}

			var viewModelResult = string.Empty;
			var modelResult = string.Empty;
			var filedConstantsResult = string.Empty;

            foreach (var row in rows)
			{
				viewModelResult += GetFullPropertyFromRow(row) + Environment.NewLine;
				modelResult += GetPropertyFromRow(row) + Environment.NewLine;
                filedConstantsResult += GetFieldConstantFromRow(row) + Environment.NewLine;
            }

			return viewModelResult + Environment.NewLine 
				+ "// =========== regular properties =========== " + Environment.NewLine 
				+ modelResult + Environment.NewLine
                + "// =========== field constants =========== " + Environment.NewLine
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

		private static string GetFullPropertyFromRow(string row)
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
private {dotnetType}{nullableChar} _{name};
public {dotnetType}{nullableChar} {name}
{{
	get {{ return _{name}; }}
    set {{ SetProperty(ref _{name}, value); }}
}}";
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
