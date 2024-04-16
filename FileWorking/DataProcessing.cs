using Serilog;


namespace FileWorking
{
    public class DataProcessing
    {
        private WiFiLibrary[] _wifi;
        public DataProcessing() { }
        /// <summary>
        /// This constructor initializes WiFiLibrary array.
        /// </summary>
        /// <param name="wifi"></param>
        public DataProcessing(WiFiLibrary[] wifi)
        {
            _wifi = wifi;
        }
        /// <summary>
        /// This method selects data from the table according to user's value.
        /// </summary>
        /// <param name="indexColumn1"></param>
        /// <param name="value1"></param>
        /// <param name="indexColumn2"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public WiFiLibrary[] Select(int indexColumn1, string value1, int indexColumn2 = -1, string value2 = "")
        {

            Log.Information($"{nameof(Select)} {LogStates.start}");

            // LINQ selecting.
            var res = from wifi in _wifi
                      where indexColumn1 == 1 && wifi.AdmArea == value1
                      || indexColumn1 == 2 && wifi.WiFiName == value1
                      || indexColumn1 == 3 && indexColumn2 == 4 && wifi.FunctionFlag == value1 && wifi.AccessFlag == value2
                      select wifi;

            List<WiFiLibrary> list = new List<WiFiLibrary>();

            foreach(WiFiLibrary wifi in res) 
            {
                list.Add(wifi);
            }

            Log.Information($"{nameof(Select)} {LogStates.end}");
            return list.ToArray();

        }
        /// <summary>
        /// This method sorts table and returns processed data.
        /// </summary>
        /// <param name="indexColumn"></param>
        /// <returns></returns>
        public WiFiLibrary[] Sort(int indexColumn)
        {
            Log.Information($"{nameof(Sort)} {LogStates.start}");

            IEnumerable<WiFiLibrary> res;

            // Sorting LibraryNames.
            if (indexColumn == 5)
            {
                res = from wifi in _wifi
                      orderby wifi.LibraryName
                      select wifi;
            }
            // Sorting CoverageArea.
            else
            {
                res = from wifi in _wifi
                      orderby wifi.CoverageArea descending
                      select wifi;
            }
            List<WiFiLibrary> list = new List<WiFiLibrary>();
            foreach (WiFiLibrary wifi in res)
            {
                list.Add(wifi);
            }
            Log.Information($"{nameof(Sort)} {LogStates.end}");

            return list.ToArray();
        }
    }
}
