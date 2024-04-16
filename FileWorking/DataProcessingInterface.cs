using FileWorking;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace FileWorking
{
    public class DataProcessingInterface
    {
        
        private DataProcessing _wifi;
        private int _idx1 = -1, _idx2 = -1;

        /// <summary>
        /// This property returns index of second value for seelction.
        /// </summary>
        public int Idx2 { get { return _idx2; } }
        public DataProcessingInterface() { }
        /// <summary>
        /// This constructor initializes index of the field and array.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="wifi"></param>
        public DataProcessingInterface(int idx, WiFiLibrary[] wifi)
        {
            _idx1 = idx;
            _wifi = new DataProcessing(wifi);
        }
        /// <summary>
        /// This constructor initializes indexes of fields and array.
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        /// <param name="wifi"></param>
        public DataProcessingInterface(int idx1, int idx2, WiFiLibrary[] wifi)
        {
            _idx1 = idx1;
            _idx2 = idx2;
            _wifi = new DataProcessing(wifi);
        }
        /// <summary>
        /// This method helps selecting and processing wrong selection values.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public WiFiLibrary[]? SelectInterface(string value1, string value2 = "")
        {
            Log.Information($"{nameof(SelectInterface)} {LogStates.start}");

            // Checking user's value.
            if (string.IsNullOrEmpty(value1))
                return null;

            WiFiLibrary[] res;

            // 1 field selecting.
            if (_idx2 == -1)
                res = _wifi.Select(_idx1, value1);

            else
                res = _wifi.Select(_idx1, value1, _idx2, value2);

            // If there are no data corresponding with user's value.
            if (res is null || res.Length <= 0)
                return null;
            Log.Information($"{nameof(SelectInterface)} {LogStates.start}");

            return res;

        }
        /// <summary>
        /// This method returns sorted data.
        /// </summary>
        /// <returns></returns>
        public WiFiLibrary[]? SortInterface()
        {

            return _wifi.Sort(_idx1);
        }
    }
}
