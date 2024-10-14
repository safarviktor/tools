using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VClipboardHelper
{
    internal class ArcGISFieldsToTypescript
    {
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
                result += GetField(row) + Environment.NewLine;                
            }

            return "export interface SOMECLASS_Fields {" + Environment.NewLine 
                + result + "}";
        }

        private static string GetField(string row)
        {
            var splits = row.Split(' ');

            var name = splits[0];
            var gisType = splits[2].Replace(",", "");
            var nullableChar = row.Contains("nullable: true") ? "?" : "";
            var typesriptType = GisTypeToTypesriptType(gisType);

            if (row.Contains("Coded Values"))
            {
                typesriptType = "CodedValues";
            }

            return $@"{name}{nullableChar}: {typesriptType};";
        }

        private static object GisTypeToTypesriptType(string gisType)
        {
            switch (gisType)
            {
                case "esriFieldTypeOID":
                case "esriFieldTypeInteger":
                case "esriFieldTypeDouble":
                    return "number";
                case "esriFieldTypeGUID":
                case "esriFieldTypeGlobalID":
                    return "string";
                case "esriFieldTypeDate":
                    return "string";
                default:
                    return "string";
            }
        }
    }
}
