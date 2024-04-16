using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using FileWorking;
using Newtonsoft.Json.Linq;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileWorking
{
    public class FileChecking
    {
        private string _button;
        private string[] _columnNames;
        public FileChecking() { }

        /// <summary>
        /// This constructor initializes button of inline menu.
        /// </summary>
        /// <param name="button"></param>
        public FileChecking(string button)
        {
            _button = button;
        }
        /// <summary>
        /// This method checks file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsCorrect(Message file)
        {
            if (string.IsNullOrEmpty(file.Document.FileName) || (_button == "button1" && file.Document.FileName[^4..] != ".csv") || (_button == "button2" && file.Document.FileName[^5..] != ".json"))
                return false;
            return true;
        }
        /// <summary>
        /// This method helps to read table from csv or json file.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<WiFiLibrary[]?> ReadData(ITelegramBotClient botClient, CancellationToken cancellationToken, string filePath)
        {
            Log.Information($"{ReadData} {LogStates.start}");

            CsvProcessing? csv = null;
            JsonProcessing? json = null;
            string destinationFile;

            // Csv downloading.
            if (_button == "button1")
            {
                csv = new CsvProcessing();
                destinationFile = "wifi-library.csv";
            }
            // Json downloading.
            else
            {
                json = new JsonProcessing();
                destinationFile = "wifi-library.json";
            }
            // Creating new stream for file with data from bot.
            await using (Stream stream = System.IO.File.Create(destinationFile))
            {
                await botClient.DownloadFileAsync(filePath, stream, cancellationToken);
            }

            FileStream fileStream = new FileStream(destinationFile, FileMode.OpenOrCreate);
            try
            {
                if (_button == "button1")
                {
                    Log.Information($"{ReadData} {LogStates.end}");

                    return await csv.Read(fileStream);
                }
                else
                {
                    Log.Information($"{ReadData} {LogStates.end}");

                    return await json.Read(fileStream);
                }
            }
            catch (JsonException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (NotSupportedException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (PathTooLongException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (IOException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (ArgumentNullException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (ArgumentException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }
            catch (NullReferenceException)
            {
                Log.Warning($"{nameof(ReadData)} {LogStates.error}");
                return null;
            }


        }
        /// <summary>
        /// This method helps to upload csv or json file with result data.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="wifi"></param>
        /// <returns></returns>
        public async Task<Stream> WriteData(ITelegramBotClient botClient, CancellationToken cancellationToken, WiFiLibrary[] wifi)
        {
            Log.Information($"{nameof(WriteData)} {LogStates.start}");

            CsvProcessing? csv = null;
            JsonProcessing? json = null;
            try
            {
                // Csv uploading.
                if (_button == "button3_1")
                {
                    csv = new CsvProcessing();
                    Log.Information($"{nameof(WriteData)} {LogStates.end}");
                    return csv.Write(wifi);
                }
                // Json uploading.
                else
                {
                    json = new JsonProcessing();
                    Log.Information($"{nameof(WriteData)} {LogStates.end}");
                    return json.Write(wifi);
                }
            }
            catch (JsonException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (NotSupportedException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (PathTooLongException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (IOException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (ArgumentNullException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (ArgumentException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
            catch (NullReferenceException)
            {
                Log.Warning($"{nameof(WriteData)} {LogStates.error}");
                return null;
            }
        }
    }
}
