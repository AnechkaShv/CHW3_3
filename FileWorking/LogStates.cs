using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWorking
{
    public class LogStates
    {
        // States for logging.
        public const string start = "beginning method";
        public const string end = "closing method";
        public const string selectionAdm = "AdmArea selection";
        public const string selectionWifi = "WiFi_Name selection";
        public const string selectionFlag = "FunctionFlag and AccessFlag selection";
        public const string sortLib = "LibraryName sorting";
        public const string sortArea = "CoverageArea sorting";
        public const string uploadCsv = "uploading csv file";
        public const string uploadJson = "uploading json file";
        public const string wrongMessage = "wrong message";
        public const string error = "error while working with file";

    }
}
