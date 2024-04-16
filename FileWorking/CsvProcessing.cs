using Serilog;
using System.IO;
using Telegram.Bot.Types;

namespace FileWorking
{
    public class CsvProcessing
    {
        private string[] _columnNames;

        /// <summary>
        /// This constructor initializes column names in csv file.
        /// </summary>
        public CsvProcessing()
        {
            _columnNames = new string[2];
            _columnNames[0] = string.Join(";", new string[] { "\"ID\"", "\"LibraryName\"", "\"AdmArea\"", "\"District\"", "\"Address\"", "\"NumberOfAccessPoints\"", "\"WiFiName\"", "\"CoverageArea\"", "\"FunctionFlag\"", "\"AccessFlag\"", "\"Password\"", "\"Latitude_WGS84\"", "\"Longitude_WGS84\"", "\"global_id\"", "\"geodata_center\"", "\"geoarea\"" }) + ";";
            _columnNames[1] = string.Join(";", new string[] { "\"Код\"", "\"Наименование библиотеки\"", "\"Административный округ\"", "\"Район\"", "\"Адрес\"", "\"Количество точек доступа\"", "\"Имя Wi-Fi сети\"", "\"Зона покрытия, в метрах\"", "\"Признак функционирования\"", "\"Условия доступа\"", "\"Пароль\"", "\"Широта в WGS-84\"", "\"Долгота в WGS-84\"", "\"global_id\"", "\"geodata_center\"", "\"geoarea\"" }) + ";";
        }
        /// <summary>
        /// This method reads file from the stream to WiFiLibrary array and checks it.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<WiFiLibrary[]> Read(Stream stream)
        {

            
            Log.Information($"{nameof(Read)} {LogStates.start}");
            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            List<string> tableRows = new List<string>();

            // Column names for comparing.
            string[] alphabetEng = _columnNames[0].Split(';');
            string[] alphabetRus = _columnNames[1].Split(';');
            
            // Repeating cycle that doesn't alllow to do next step until a correct data is entered.
            try
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string a;
                    while ((a = sr.ReadLine()) != null)
                        tableRows.Add(a);
                }
                // Checking data in the file.
                // Number of table rows should be >= 3 because a file must include two columns' name rows and at least one row with data.
                if (tableRows is null || tableRows.Count < 3 || tableRows[0].Split(";")[..^1].Length != 16)
                {
                    throw new ArgumentNullException();
                }

                for (int i = 2; i < tableRows.Count; i++)
                {
                    string[] elems = tableRows[i].Split(";");
                    // Checking length of every row. It mast be equal to the first two lines. Without last element because it is \n.
                    if (elems[..^1].Length != tableRows[0].Split(";")[..^1].Length)
                    {
                        throw new ArgumentNullException();
                    }
                    // Checking data format. Every element's first and last symbol mast be a quote.
                    for (int j = 0; j < tableRows[i].Split(";")[..^1].Length; j++)
                    {
                        if (elems[j][0] != '\"' || elems[j][^1] != '\"' || !int.TryParse(elems[0].Trim('\"'), out _) || !int.TryParse(elems[5].Trim('\"'), out _) || !int.TryParse(elems[7].Trim('\"'), out _)|| !double.TryParse(elems[11].Trim('\"'), out _) || !double.TryParse(elems[12].Trim('\"'), out _)|| !long.TryParse(elems[13].Trim('\"'), out _))
                        {
                            throw new ArgumentNullException();
                        }
                    }
                }
                // Checking first two rows, every one must consists of initial names of columns.
                for (int j = 0; j < tableRows[0].Split(';')[..^1].Length; j++)
                {
                    if (tableRows[0].Split(";")[j] != alphabetEng[j] || tableRows[1].Split(";")[j] != alphabetRus[j])
                    {
                        throw new ArgumentNullException();
                    }
                }
                _columnNames = tableRows.ToArray()[..2];

                Log.Information($"{nameof(Read)} {LogStates.end}");
                return InitData(tableRows.ToArray()[2..]);
            }
            catch (ArgumentOutOfRangeException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;
            }
            catch (ArgumentNullException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;
            }
            catch (ArgumentException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;

            }
            catch (PathTooLongException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;
            }
            catch (IOException)
            {
                Log.Warning($"{nameof(Read)} {LogStates.error}");
                return null;
            }

        }
        /// <summary>
        /// This method parses file into correct data for creating rray of WiFiLibrary objects.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private WiFiLibrary[] InitData(string[] rows)
        {
            string[][] tableValues = new string[rows.Length][];
            for (int i = 0; i < rows.Length; i++)
            {
                // Without last element of a row because it is "\n".
                tableValues[i] = rows[i].Split(";")[..^1];
                for (int j = 0; j < tableValues[i].Length; j++)
                {
                    // Deleting quotes around each element.
                    tableValues[i][j] = tableValues[i][j].Trim('\"');
                }
            }
            WiFiLibrary[] wifi = new WiFiLibrary[tableValues.Length];

            // Initializing array.
            for (int i = 0; i < tableValues.Length; i++)
            {
                wifi[i] = new WiFiLibrary(int.Parse(tableValues[i][0]), tableValues[i][1], tableValues[i][2], tableValues[i][3], tableValues[i][4], int.Parse(tableValues[i][5]), tableValues[i][6], int.Parse(tableValues[i][7]), tableValues[i][8], tableValues[i][9], tableValues[i][10], double.Parse(tableValues[i][11]), double.Parse(tableValues[i][12]), long.Parse(tableValues[i][13]), tableValues[i][14], tableValues[i][15]);
            }
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
            while (System.IO.File.Exists($"wifi-library{count}.csv"))
            {
                count += 1;
            }

            using (StreamWriter sw = new StreamWriter($"wifi-library{count}.csv"))
            {
                // Writing column names.
                for (int i = 0; i < _columnNames.Length; i++)
                {
                    sw.WriteLine(_columnNames[i]);
                }

                // Writing resulting table.
                for (int i = 0; i < wifi.Length; i++)
                {
                    sw.WriteLine(wifi[i]);
                }
            }
            Log.Information($"{nameof(Write)} {LogStates.end}");
            Log.Information($"{nameof(Write)} {LogStates.uploadCsv}"); 
            return System.IO.File.OpenRead($"wifi-library{count}.csv");
            
        }

    }
}