using System;
using System.Linq;

namespace VClipboardHelper
{
	internal static class AcrGisFieldsToCSharpViewModel
	{
		/*
		 Fields:
OBJECTID ( type: esriFieldTypeOID, alias: OBJECTID, editable: false, nullable: false, defaultValue: null, modelName: OBJECTID )
Lys ( type: esriFieldTypeInteger, alias: Lys, editable: true, nullable: true, defaultValue: null, modelName: Lys )
lysfarge ( type: esriFieldTypeInteger, alias: lysfarge, editable: true, nullable: true, defaultValue: null, modelName: lysfarge )
Sektortekst ( type: esriFieldTypeString, alias: Sektortekst, editable: true, nullable: true, length: 2, defaultValue: null, modelName: Sektortekst )
retningSektorlinje1 ( type: esriFieldTypeDouble, alias: retningSektorlinje1, editable: true, nullable: true, defaultValue: null, modelName: retningSektorlinje1 )
lengdeSektorlinje1 ( type: esriFieldTypeDouble, alias: lengdeSektorlinje1, editable: true, nullable: true, defaultValue: null, modelName: lengdeSektorlinje1 )
retningSektorlinje2 ( type: esriFieldTypeDouble, alias: retningSektorlinje2, editable: true, nullable: true, defaultValue: null, modelName: retningSektorlinje2 )
lengdeSektorlinje2 ( type: esriFieldTypeDouble, alias: lengdeSektorlinje2, editable: true, nullable: true, defaultValue: null, modelName: lengdeSektorlinje2 )
Beskrivelse ( type: esriFieldTypeString, alias: Beskrivelse, editable: true, nullable: true, length: 255, defaultValue: null, modelName: Beskrivelse )
SdevLinje1 ( type: esriFieldTypeDouble, alias: SdevLinje1, editable: true, nullable: true, defaultValue: null, modelName: SdevLinje1 )
AntallKontrollPunkt1 ( type: esriFieldTypeInteger, alias: AntallKontrollPunkt1, editable: true, nullable: true, defaultValue: null, modelName: AntallKontrollPunkt1 )
SdevLinje2 ( type: esriFieldTypeDouble, alias: SdevLinje2, editable: true, nullable: true, defaultValue: null, modelName: SdevLinje2 )
AntallKontrollPunkt2 ( type: esriFieldTypeInteger, alias: AntallKontrollPunkt2, editable: true, nullable: true, defaultValue: null, modelName: AntallKontrollPunkt2 )
MaltAv ( type: esriFieldTypeString, alias: MåltAv, editable: true, nullable: true, length: 25, defaultValue: null, modelName: MaltAv )
MaltDato ( type: esriFieldTypeDate, alias: MåltDato, editable: true, nullable: true, length: 8, defaultValue: null, modelName: MaltDato )
SektorID ( type: esriFieldTypeInteger, alias: SektorID, editable: true, nullable: true, defaultValue: null, modelName: SektorID )
lysstyrke ( type: esriFieldTypeDouble, alias: lysstyrke, editable: true, nullable: true, defaultValue: null, modelName: lysstyrke )
SektorRadius ( type: esriFieldTypeDouble, alias: SektorRadius, editable: true, nullable: true, defaultValue: null, modelName: SektorRadius )
GlobalID ( type: esriFieldTypeGlobalID, alias: GlobalID, editable: false, nullable: false, length: 38, defaultValue: null, modelName: GlobalID )
FKNavinst ( type: esriFieldTypeGUID, alias: FKNavinst, editable: true, nullable: true, length: 38, defaultValue: null, modelName: FKNavinst )
SektorNr ( type: esriFieldTypeInteger, alias: SektorNr, editable: true, nullable: true, defaultValue: null, modelName: SektorNr )
NFSStatus ( type: esriFieldTypeInteger, alias: NFSStatus, editable: true, nullable: true, defaultValue: null, modelName: NFSStatus , Coded Values: [2: Prosjektert] , [20: Operativ] , [21: Fjernet] , ...7 more... )
		 */

		public static string Execute(string input)
		{
			var rows = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			if (rows[0].StartsWith("Fields:")) 
			{
				rows = rows.Skip(1).ToArray();
			}

			var result = string.Empty;

			foreach (var row in rows)
			{
				result += GetPropertyFromRow(row) + Environment.NewLine + Environment.NewLine;
			}

			return result;
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
					return "decimal";
				case "esriFieldTypeGUID":
				case "esriFieldTypeGlobalID":
					return "Guid";
				default:
					return "string";
			}
		}
	}
}
