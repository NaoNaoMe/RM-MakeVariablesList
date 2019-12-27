using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rm_MakeVariablesList
{
    class Program
    {
        static int Main(string[] args)
        {
            int returnValue = -1;

            System.Diagnostics.FileVersionInfo ver =
                System.Diagnostics.FileVersionInfo.GetVersionInfo(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            Console.WriteLine(ver.OriginalFilename + " " + ver.FileVersion);

            if (args.Length == 0)
            {
                System.Console.WriteLine(" This program converts [[readelf.exe --debug-dump=info xxx.elf] output] to RM address map form ");
                return returnValue;

            }
            else if (args.Length != 2)
            {
                System.Console.WriteLine("Error: Invalid parameter.");
                return returnValue;

            }

            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string inputPath;
            string outputPath;

            if (System.IO.Path.IsPathRooted(args[0]))
            {
                inputPath = args[0];
            }
            else
            {
                inputPath = currentDir + @"\" + args[0];
            }

            if (System.IO.Path.IsPathRooted(args[1]))
            {
                outputPath = args[1];
            }
            else
            {
                outputPath = currentDir + @"\" + args[1];
            }

            List<string> lines = new List<string>();

            try
            {
                using (var sr = new System.IO.StreamReader(inputPath, Encoding.GetEncoding("utf-8")))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Replace("\r\n", "\n");
                        lines.Add(line);
                    }

                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return returnValue;
            }

            List<VariableElement> elements = new List<VariableElement>();

            elements.Clear();

            var debugList = new List<DebugInfo>();
            ReadElf.ConstractDebugList(lines, ref debugList);

            var newDebugList = new List<DebugInfo>();
            ReadElf.FormatDebugList(ref debugList, ref newDebugList);

            ReadElf.ConstructVariablesList(ref newDebugList, ref elements);

            if (elements.Count > 0)
            {
                var mapList = ReadElf.Convert(elements);

                List<string> textList = new List<string>();

                if (RmAddressMap.Convert(textList, mapList))
                {
                    try
                    {
                        using (var sw = new System.IO.StreamWriter(outputPath, false, System.Text.Encoding.GetEncoding("utf-8")))
                        {
                            foreach (var item in textList)
                            {
                                sw.WriteLine(item);

                            }

                        }

                        System.Console.WriteLine("Success.");

                        returnValue = 0;

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }

                }
                else
                {
                    System.Console.WriteLine("Error: Failed to convert to map file.");

                }

            }
            else
            {
                System.Console.WriteLine("Error: Failed to interpret file as map file.");

            }

            return returnValue;
        }
    }
}
