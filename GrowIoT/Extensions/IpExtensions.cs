using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace GrowIoT.Extensions
{
    public static class IpExtensions
    {
        public static string GetCurrentIp()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "hostname",  //my linux command i want to execute
                    Arguments = "-I",  //the argument to return ip address
                    UseShellExecute = false,
                    RedirectStandardOutput = true,  //redirect output to my code here
                    CreateNoWindow = true //do not show a window
                }
            };

            proc.Start();  //start the process
            var lines = new List<string>();
            while (!proc.StandardOutput.EndOfStream)  //wait until entire stream from output read in
            {
                lines.Add(proc.StandardOutput.ReadLine()); //this contains the ip output      
            }

            return lines.Any() ? lines.FirstOrDefault()?.Replace(" ", "") : null;
        }
    }
}