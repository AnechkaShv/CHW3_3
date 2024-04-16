using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FileWorking
{
    [Serializable]
    public class WiFiLibrary
    {
        private int _id, _numberOfAccessPoints, _coverageArea;
        long _globalId;
        private double _latitudeWGS84, _longitudeWGS84;
        private string? _libraryName, _admArea, _district, _address, _wifiName, _functionFlag, _accessFlag, _password, _geodata, _geoarea;
        [JsonPropertyName("ID")]
        public int Id { get { return _id; } set { _id = value; } }
        public string District { get { return _district; } set { _district = value; } }
        public string Address { get { return _address; } set {  _address = value; } }
        public int NumberOfAccessPoints { get { return _numberOfAccessPoints; } set { _numberOfAccessPoints = value; } }
        public string AdmArea { get { return _admArea; } set { _admArea = value; } }
        public string WiFiName { get { return _wifiName;  } set { _wifiName = value; } }
        public string FunctionFlag { get { return _functionFlag; } set { _functionFlag = value; } }
        public string AccessFlag { get { return _accessFlag; }set { _accessFlag = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        [JsonPropertyName("Latitude_WGS84")]
        public double Latitude { get { return _latitudeWGS84; }set { _latitudeWGS84 = value; } }
        [JsonPropertyName("Longtitude_WGS84")]
        public double Longtitude { get { return _longitudeWGS84; } set { _longitudeWGS84 = value; } }
        public string LibraryName { get { return _libraryName; } set { _libraryName = value; } }
        public int CoverageArea { get { return _coverageArea; } set { _coverageArea = value; } }
        [JsonPropertyName("global_id")]
        public long GlobalId { get { return _globalId; } set { _globalId = value; } }
        [JsonPropertyName("geodata_center")]
        public string GeoData { get { return _geodata; } set {  _geodata = value; } }
        [JsonPropertyName("geoarea")]
        public string GeoArea { get { return _geoarea; } set { _geoarea = value; } }

        public WiFiLibrary() { }
        /// <summary>
        /// This constructor initializes all fileds.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libraryName"></param>
        /// <param name="admArea"></param>
        /// <param name="district"></param>
        /// <param name="address"></param>
        /// <param name="numberOfAccessPoints"></param>
        /// <param name="wifiName"></param>
        /// <param name="coverageArea"></param>
        /// <param name="functionFlag"></param>
        /// <param name="accessFlag"></param>
        /// <param name="password"></param>
        /// <param name="latitude"></param>
        /// <param name="longtitude"></param>
        /// <param name="globalId"></param>
        /// <param name="geodata"></param>
        /// <param name="geoarea"></param>
        public WiFiLibrary(int id,string libraryName, string admArea, string district, string address, int numberOfAccessPoints, string wifiName, int coverageArea, string functionFlag, string accessFlag, string password, double latitude, double longtitude, long globalId, string geodata, string geoarea)
        {
            _id = id;
            _libraryName = libraryName;
            _admArea = admArea;
            _district = district;
            _address = address;
            _wifiName = wifiName;
            _coverageArea = coverageArea;
            _functionFlag = functionFlag;
            _accessFlag = accessFlag;
            _password = password;
            _numberOfAccessPoints = numberOfAccessPoints;
            _geodata = geodata;
            _geoarea = geoarea;
            _latitudeWGS84 = latitude;
            _longitudeWGS84 = longtitude;
            _globalId = globalId;
        }
        /// <summary>
        /// This methods creates a correct string with all fields data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string[] res = new string[] {$"{_id}", $"{_libraryName}",
                $"{_admArea}", $"{_district}", $"{_address}",
                $"{_numberOfAccessPoints}", $"{_wifiName}",
                $"{_coverageArea}", $"{_functionFlag}", $"{_accessFlag}",
                $"{_password}", $"{_latitudeWGS84}", $"{_longitudeWGS84}", 
                $"{_globalId}", $"{_geodata}", $"{_geoarea}"};
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = $"\"{res[i]}\"";
            }
            return string.Join(';', res) + ";";
        }
        /// <summary>
        /// This method converts object's fields to json format.
        /// </summary>
        /// <returns></returns>
        public string ToJSON()
        {
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            string[] names = new string[] { "ID", "LibraryName", "AdmArea", "District", "Address",
                "NumberOfAccessPoints", "WiFiName", "CoverageArea", "FunctionFlag", "AccessFlag",
                "Password", "Latitude_WGS84", "Longtitude_WGS84", "global_id", "geodata_center", "geoarea" };
            object[] objects = new object[] { Id, LibraryName, AdmArea, District, Address, 
                NumberOfAccessPoints, WiFiName, CoverageArea, FunctionFlag, AccessFlag,
                Password, Latitude, Longtitude, GlobalId, GeoData, GeoArea};

            // Returning string.
            StringBuilder sb = new StringBuilder();

            // Beginning an object string with {. 
            sb.Append("  {\n");

            // Writing formatted wtring with an object's fields.
            for (int i = 0; i < names.Length; i++)
            {
                if (i != names.Length - 1)
                {
                    if (objects[i] is string && objects[i] != null)
                        sb.Append($"    \"{names[i]}\": {JsonSerializer.Serialize(objects[i], options)},\n");
                    else
                        sb.Append($"    \"{names[i]}\": {JsonSerializer.Serialize(objects[i], options)},\n");
                }

                else
                {
                    if (objects[i] is string && objects[i] != null)
                        sb.Append($"    \"{names[i]}\": \"{objects[i]}\"\n");
                    else
                        sb.Append($"    \"{names[i]}\": {JsonSerializer.Serialize(objects[i])}\n");
                }
            }
            sb.Append("  }");
            return sb.ToString();
        }
    }
}
