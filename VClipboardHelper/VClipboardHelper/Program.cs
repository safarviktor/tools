using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VClipboardHelper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            DoWork();

            SetLastActiveWindow();
        }

        private static void SetLastActiveWindow()
        {
            IntPtr lastWindowHandle = GetWindow(
                Process.GetCurrentProcess().MainWindowHandle,
                (uint)GetWindow_Cmd.GW_HWNDNEXT
            );
            while (true)
            {
                IntPtr temp = GetParent(lastWindowHandle);
                if (temp.Equals(IntPtr.Zero))
                    break;
                lastWindowHandle = temp;
            }
            SetForegroundWindow(lastWindowHandle);
        }

        private static void DoWork()
        {
            var mainInput = Clipboard.GetText().Trim();

            if (IsArcGISDomains(mainInput))
            {
                var update = ArcGISDomainsToCSharpEnums.Execute(mainInput);
                Clipboard.SetText(update);
                return;
            }

            if (IsArcGISFields(mainInput))
            {
                var update = ArcGISFieldsToCSharpClassesWithMvvmToolkit.Execute(mainInput);
                update +=
                    Environment.NewLine
                    + "========================== FULL PROPS =========================="
                    + Environment.NewLine
                    + ArcGISFieldsToCSharpClasses.Execute(mainInput);
                update +=
                    Environment.NewLine
                    + "========================== TYPESCRIPT =========================="
                    + Environment.NewLine
                    + ArcGISFieldsToTypescript.Execute(mainInput);

                Clipboard.SetText(update);
                return;
            }

            if (IsClass(mainInput))
            {
                var update = CSharpToSqlTableBuilder.CreateSqlTable(mainInput);
                Clipboard.SetText(update);
                return;
            }

            if (IsJson(mainInput))
            {
                var update = PrettyPrintJson(mainInput);
                Clipboard.SetText(update);
                return;
            }

            if (IsSqlTableInfo(mainInput))
            {
                var update = SqlToCSharpModelBuilder.GetCsharpModel(mainInput);
                Clipboard.SetText(update);
                return;
            }

            if (IsHttpLink(mainInput))
            {
                var update = $@"<a href=""{mainInput}"" target=""_blank"">{mainInput}</a>";
                Clipboard.SetText(update);
                return;
            }

            if (IsStackTrace(mainInput))
            {
                var update = mainInput.Replace(" at ", Environment.NewLine + " at ");
                Clipboard.SetText(update);
                return;
            }

            if (IsSql(mainInput))
            {
                Clipboard.SetText(BeautifySql(mainInput));
                return;
            }

            if (IsLinqToSqlVariables(mainInput))
            {
                var update = ProcessLinqToSqlVariables(mainInput);
                Clipboard.SetText(update);
                return;
            }

            // list to CSV
            if (mainInput.Contains(Environment.NewLine))
            {
                Clipboard.SetText(mainInput.Replace(Environment.NewLine, ","));
                return;
            }

            // CSV to single-quoted CSV
            if (mainInput.Contains(",") && !mainInput.StartsWith("'") && !mainInput.EndsWith("'"))
            {
                Clipboard.SetText(string.Concat("'", mainInput.Replace(",", "','"), "'"));
                return;
            }
        }

        private static bool IsArcGISDomains(string mainInput)
        {
            return mainInput.StartsWith("{\"domains\":");
        }

        private static bool IsArcGISFields(string mainInput)
        {
            return mainInput.StartsWith("Fields:");
        }

        private static bool IsClass(string mainInput)
        {
            mainInput = mainInput.Trim();

            return mainInput.StartsWith("public class")
                || mainInput.StartsWith("protected class")
                || mainInput.StartsWith("internal class");
        }

        private static string PrettyPrintJson(string mainInput)
        {
            var fnam = JsonConvert.DeserializeObject<dynamic>(mainInput);
            return JsonConvert.SerializeObject(fnam, Formatting.Indented);
        }

        private static bool IsJson(string mainInput)
        {
            var withoutWhiteSpace = Regex.Replace(mainInput, @"\s+", "");
            return (withoutWhiteSpace.StartsWith("{\"") || withoutWhiteSpace.StartsWith("{["))
                && (withoutWhiteSpace.EndsWith("\"}") || withoutWhiteSpace.EndsWith("]}"));
        }

        private static bool IsSqlTableInfo(string mainInput)
        {
            return mainInput.StartsWith(SqlToCSharpModelBuilder.SqlTableInfoIdentifier);
        }

        private static bool IsHttpLink(string mainInput)
        {
            return (mainInput.StartsWith("http://") || mainInput.StartsWith("https://"))
                && !mainInput.Contains(Environment.NewLine);
        }

        private static string BeautifySql(string mainInput)
        {
            var keyWordsToPrefixWithNewLine = new List<string>
            {
                "select ",
                "from ",
                "update ",
                "set ",
                "where ",
                "group by ",
                "order by ",
                "having ",
                "drop ",
                "create ",
                "cross ",
                "left join ",
                "left outer join ",
                "right join ",
                "right outer join ",
                "cross apply ",
                "outer apply ",
                "cross join ",
                "inner join ",
                "join ",
                "declare "
            };

            var keywordsToUpper = new List<string>
            {
                " desc",
                " asc",
                " table",
                " top ",
                " and ",
                " is not null",
                " is null",
                " not null",
                " null",
                " on ",
                " as ",
                " int ",
                " int" + Environment.NewLine,
                " int=",
                " varchar(",
                " varchar" + Environment.NewLine,
                " nvarchar(",
                " bit ",
                " bit" + Environment.NewLine,
            };

            foreach (var keyword in keyWordsToPrefixWithNewLine)
            {
                mainInput = mainInput.Replace(
                    keyword.ToUpper(),
                    Environment.NewLine + keyword.ToUpper()
                );
                mainInput = mainInput.Replace(keyword, Environment.NewLine + keyword.ToUpper());
            }

            foreach (var keyword in keywordsToUpper)
            {
                mainInput = mainInput.Replace(keyword, keyword.ToUpper());
            }

            return mainInput;
        }

        private static bool IsSql(string mainInput)
        {
            return (mainInput.ContainsIgnoreCase("SELECT") && mainInput.ContainsIgnoreCase("FROM"))
                || (mainInput.ContainsIgnoreCase("INSERT") && mainInput.ContainsIgnoreCase("INTO"))
                || (mainInput.ContainsIgnoreCase("UPDATE") && mainInput.ContainsIgnoreCase("SET"));
        }

        // expected input:
        // N'@p__linq__0 bit,@p__linq__1 int',@p__linq__0=1,@p__linq__1=7
        // N'@_msparam_0 nvarchar(4000)',@_msparam_0=N'master'
        private static string ProcessLinqToSqlVariables(string mainInput)
        {
            // remove leading N
            if (mainInput.StartsWith("N"))
            {
                mainInput = mainInput.Substring(1);
            }

            // remove leading ['] (apostrophe)
            if (mainInput.StartsWith("'"))
            {
                mainInput = mainInput.Substring(1);
            }

            // the next apostrophe is the delimiter between declarations and values
            var declarations = mainInput.Substring(0, mainInput.IndexOf('\''));
            var values = mainInput.Substring(mainInput.IndexOf('\'') + 1).Substring(2); // remove the ['] and the first [,@]

            declarations = $"DECLARE {declarations}";
            var valueList = values
                .Split(new string[] { ",@" }, StringSplitOptions.None)
                .Select(x => $"{Environment.NewLine}set @{x}");
            return declarations + string.Join("", valueList);
        }

        private static bool IsLinqToSqlVariables(string mainInput)
        {
            return mainInput.Contains("p__linq__") || mainInput.Contains("_msparam_");
        }

        private static bool IsStackTrace(string mainInput)
        {
            return mainInput.Contains("Exception")
                || mainInput.Contains("exception")
                || mainInput.Contains("stack trace");
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
