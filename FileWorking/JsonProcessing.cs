using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileWorking
{
    public class JsonProcessing
    {
        /// <summary>
        /// This method reads file from the stream to WiFiLibrary array and checks it.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<WiFiLibrary[]> Read(Stream stream)
        {
            Log.Information($"{nameof(Read)} {LogStates.start}");
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            StringBuilder sb = new StringBuilder();
            WiFiLibrary[]? wifi;

            // Stream reading.
            using (StreamReader sr = new StreamReader(stream))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line);
                }
            }
            
            wifi = System.Text.Json.JsonSerializer.Deserialize<WiFiLibrary[]>(sb.ToString());

            // Checking correctness of deserealization.
            if (wifi is null || wifi.Length <= 0)
                throw new ArgumentNullException();
            Log.Information($"{nameof(Read)} {LogStates.start}");
            Log.Information($"{nameof(Read)} {LogStates.end}");

            return wifi;
        }
        /// <summary>
        /// This method writes data to stream from WiFiLibrary array.
        /// </summary>
        /// <param name="wifi"></param>
        /// <returns></returns>
        public Stream Write(WiFiLibrary[] wifi)
        {
            Log.Information($"{nameof(Write)} {LogStates.start}");

            int count = 1;

            // Creating file with unique name.
            while (System.IO.File.Exists($"wifi-library{count}.json"))
            {
                count += 1;
            }
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");

            // Writing data.
            using (StreamWriter sw = new StreamWriter($"wifi-library{count}.json"))
            {
                sw.WriteLine("[");
                for (int i = 0; i < wifi.Length; i++)
                {
                    if (i != wifi.Length - 1)
                        sw.WriteLine($"  {wifi[i].ToJSON()},");
                    else
                        sw.WriteLine($"  {wifi[i].ToJSON()}");
                }
                sw.WriteLine("]");
            }
            Log.Information($"{nameof(Read)} {LogStates.uploadJson}");

            Log.Information($"{nameof(Read)} {LogStates.end}");

            return File.OpenRead($"wifi-library{count}.json");
        }

    }
}
