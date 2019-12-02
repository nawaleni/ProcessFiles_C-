using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ProcessFiles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (StreamWriter w = new StreamWriter(@"D:\Log.txt"))
            {
                string path = @"D:\Process";
                int exitCode = 0;
                List<MyFile> myFileList = SortFiles(path);

                if (myFileList != null)
                {
                    foreach(var file in myFileList)
                    {
                        w.WriteLine("Process for file " + file.Filename + " has intiated.");
                        exitCode = ProcessFiles(file.Filename, w);
                        if (exitCode != 0)
                        {
                            w.WriteLine("Process for file " + file.Filename + " has terminated.");
                            break;
                        }
                        else
                        {
                            File.Delete(file.Filename);
                            w.WriteLine(file.Filename + " is deleted.");
                        }
                            
                    }
                }
            }
                
        }

        public static List<MyFile> SortFiles(string path)
        {
            if (Directory.Exists(path))
            {
                List<MyFile> myFileList = new List<MyFile>();

                IEnumerable<string> myFiles = Directory.GetFiles(path);

                foreach (string file in myFiles)
                {
                    MyFile myFile = new MyFile();
                    //myFile.Filename = newFile[0].Split('\\').Last();
                    myFile.Filename = file;
                    //string s = newFile[1].Split('.')[0];
                    myFile.LastmodifiedDTM = DateTime.ParseExact(file.Split('\\').Last().Split('.')[0], "yyyyMMddTHHmmss", null);
                    myFileList.Add(myFile);
                }

                myFileList = myFileList
                    .Where(f => f.LastmodifiedDTM.HasValue)
                    .OrderBy(f => f.LastmodifiedDTM).ToList();

                return myFileList;
            }
            else
            {
                Console.WriteLine("Please provide valid path.");
                return null;
            }
        }

        public static int ProcessFiles(string command, StreamWriter w)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            //log the outpu, error if any and existcode
            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            w.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            w.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            w.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();

            return exitCode;
        }
        
    }
    
}
