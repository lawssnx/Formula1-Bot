using Newtonsoft.Json;

namespace RaceBT_
{
    public class DriverInfo
    {
        public class Driver
        {
            [JsonProperty("driverId")]
            public string DriverId { get; set; }

            [JsonProperty("permanentNumber")]
            public string PermanentNumber { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("givenName")]
            public string GivenName { get; set; }

            [JsonProperty("familyName")]
            public string FamilyName { get; set; }

            [JsonProperty("dateOfBirth")]
            public string DateOfBirth { get; set; }

            [JsonProperty("nationality")]
            public string Nationality { get; set; }
        }

        public class DriverData
        {
            [JsonProperty("MRData")]
            public MRData MRData { get; set; }
        }

        public class MRData
        {
            [JsonProperty("DriverTable")]
            public DriverTable DriverTable { get; set; }
        }

        public class DriverTable
        {
            [JsonProperty("Drivers")]
            public Driver[] Drivers { get; set; }
        }
    }
}