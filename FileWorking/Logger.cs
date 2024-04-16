using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Telegram.Bot.Types;
using System.IO;
using System.Diagnostics;
using Serilog;

namespace FileWorking
{
    public class Logger
    {
       /// <summary>
       /// This method creates messages and loges it.
       /// </summary>
        public static void CreateLogger()
        {
            string logDirectory = @"..\..\..\..\var";

            logDirectory = logDirectory.Replace('\\', Path.DirectorySeparatorChar);
            Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(logDirectory, "messages.log"),
                    rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}
