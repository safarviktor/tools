using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace VClipboardHelper
{
    internal static class ArcGISDomainsToCSharpEnums
    {
        public static string Execute(string input)
        {
            /*
             
             {"domains": [
 {
  "type": "codedValue",
  "name": "ProsjektOmradeStatus",
  "description": "",
  "codedValues": [
   {
    "name": "Plan",
    "code": 1
   },
   {
    "name": "I arbeid",
    "code": 2
   },
   {
    "name": "Avsluttet",
    "code": 3
   },
   {
    "name": "Reetablert",
    "code": 4
   }
  ],
  "fieldType": "esriFieldTypeInteger",
  "mergePolicy": "esriMPTDefaultValue",
  "splitPolicy": "esriSPTDefaultValue"
 },
 {
  "type": "codedValue",
  "name": "NFSStatus",
  "description": "",
  "codedValues": [
   {
    "name": "Prosjektert",
    "code": 2
   },
   {
    "name": "Operativ",
    "code": 20
   },
   {
    "name": "Fjernet",
    "code": 21
   },
   {
    "name": "Nedlagt",
    "code": 23
   },
   {
    "name": "Prosjektert nedlagt",
    "code": 4
   },
   {
    "name": "Utplassert/Endret",
    "code": 3
   },
   {
    "name": "Prosjektert Fjernet",
    "code": 5
   },
   {
    "name": "Fjernet/Endret",
    "code": 6
   },
   {
    "name": "Nedlagt/Endret",
    "code": 7
   },
   {
    "name": "Prosjektert/Eksisterende",
    "code": 1
   }
  ],
  "fieldType": "esriFieldTypeInteger",
  "mergePolicy": "esriMPTDefaultValue",
  "splitPolicy": "esriSPTDefaultValue"
 }
]}
             
             */


            var results =
                "using System.ComponentModel;" + Environment.NewLine + Environment.NewLine;
            var dynob = JsonConvert.DeserializeObject<dynamic>(input);

            foreach (var domain in dynob.domains)
            {
                if (domain.type != "codedValue")
                {
                    continue;
                }

                results += CreateEnum(domain);
            }

            return results;
        }

        private static string CreateEnum(dynamic domain)
        {
            var values = CodedValuesToEnumMembers(domain.codedValues, domain.name.ToString());

            string enumName = Convert.ToString(domain.name);

            return $@"
public enum {enumName.Substring(0, 1).ToUpper() + enumName.Substring(1)}
{{
    {values}
}}
";
        }

        private static string CodedValuesToEnumMembers(
            IEnumerable<dynamic> codedValues,
            string domainName
        )
        {
            var result = string.Empty;

            Regex removeChars = new Regex(@"[:;,\t\r\\ /()]|[\n]{2}");
            Regex underscoreReplace = new Regex(@"[-+.*><&]");
            Regex aa = new Regex("[åÅ]");
            Regex ae = new Regex("[æÆ]");
            Regex oe = new Regex("[øØ]");

            foreach (var codedValue in codedValues.OrderBy(cv => cv.code))
            {
                string name = codedValue.name;
                string originalName = name;
                name = removeChars.Replace(name, "");
                name = aa.Replace(name, "aa");
                name = ae.Replace(name, "ae");
                name = oe.Replace(name, "oe");
                name = underscoreReplace.Replace(name, "_");
                name = name.Replace("&", "and");

                int? intValue = GetEnumIntValue(codedValue);
                var intValueAssignment = intValue.HasValue ? $" = {intValue}" : string.Empty;

                if (!char.IsLetter(name.FirstOrDefault()))
                {
                    name = domainName + "_" + name;
                }

                result +=
                    $"\t[Description(\"{originalName}\")]{Environment.NewLine}"
                    + $"\t{name}{intValueAssignment},{Environment.NewLine}";
            }

            return result;
        }

        private static int? GetEnumIntValue(dynamic codedValue)
        {
            if (int.TryParse(codedValue.code.ToString(), out int intValue))
            {
                return intValue;
            }

            return null;
        }
    }
}
