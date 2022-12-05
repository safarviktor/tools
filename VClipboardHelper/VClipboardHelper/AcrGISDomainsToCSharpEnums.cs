using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VClipboardHelper
{
	internal static class AcrGISDomainsToCSharpEnums
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


            var results = string.Empty;
            var dynob = JsonConvert.DeserializeObject<dynamic>(input);

            foreach (var domain in dynob.domains)
            {
                if (domain.type != "codedValue")
                {
                    continue;
                }

                results += CreateEnum(domain) + Environment.NewLine + Environment.NewLine;
			}

            return results;
		}

		private static string CreateEnum(dynamic domain)
		{
            var values = CodedValuesToEnumMembers(domain.codedValues);

            return $@"
public enum {domain.name}
{{
    {values}
}}
";
		}

		private static string CodedValuesToEnumMembers(IEnumerable<dynamic> codedValues)
		{
            var result = string.Empty;

			Regex removeChars = new Regex("[:;,\t\r\\ ]|[\n]{2}");
			Regex aa = new Regex("[åÅ]");
			Regex ae = new Regex("[æÆ]");
			Regex oe = new Regex("[øØ]");

			foreach (var codedValue in codedValues.OrderBy(cv => cv.code))
            {
                string name = codedValue.name;
                removeChars.Replace(name, "");
				aa.Replace(name, "aa");
				ae.Replace(name, "ae");
				oe.Replace(name, "oe");
				name = name.Replace("-", "_");

				result += $"\t{name} = {codedValue.code},{Environment.NewLine}";
			}

            return result;
		}
	}
}
