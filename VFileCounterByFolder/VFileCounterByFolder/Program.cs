using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFileCounterByFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            var dirToCheck = System.IO.Directory.GetCurrentDirectory();
            var allFiles = Directory.EnumerateFiles(dirToCheck, "*.*", SearchOption.AllDirectories);

            var results = new Dictionary<string, int>();
            results.Add(dirToCheck, 0);

            foreach (var f in allFiles)
            {
                var fi = new FileInfo(f);
                if (!results.ContainsKey(fi.Directory.FullName))
                {
                    results.Add(fi.Directory.FullName, 0);
                }

                results[fi.Directory.FullName]++;
            }

            var logContent = new StringBuilder();
            logContent.Append("Directory;NumberOfFiles" + Environment.NewLine);

            foreach (var dirResult in results)
            {
                logContent.Append($"{dirResult.Key};{dirResult.Value}{Environment.NewLine}");
            }

            File.WriteAllText($"{(typeof(Program)).FullName}_log_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv", logContent.ToString());
        }
    }
}
