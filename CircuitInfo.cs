using Newtonsoft.Json;
namespace RaceBT_
{
    public class CircuitInfo
    {
        public class CircuitData
        {
            [JsonProperty("MRData")]
            public MRData MRData { get; set; }
        }
        public class Location
        {
            [JsonProperty("lat")]
            public string Latitude { get; set; }

            [JsonProperty("long")]
            public string Longitude { get; set; }

            public string Locality { get; set; }

            public string Country { get; set; }
        }

        public class Circuit
        {
            [JsonProperty("circuitId")]
            public string CircuitId { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("circuitName")]
            public string CircuitName { get; set; }

            [JsonProperty("Location")]
            public Location Location { get; set; }
        }

        public class CircuitTable
        {
            [JsonProperty("constructorId")]
            public string ConstructorId { get; set; }

            [JsonProperty("driverId")]
            public string DriverId { get; set; }

            [JsonProperty("Circuits")]
            public List<Circuit> Circuits { get; set; }
        }

        public class MRData
        {
            [JsonProperty("xmlns")]
            public string Xmlns { get; set; }

            [JsonProperty("series")]
            public string Series { get; set; }

            [JsonProperty("limit")]
            public string Limit { get; set; }

            [JsonProperty("offset")]
            public string Offset { get; set; }

            [JsonProperty("total")]
            public string Total { get; set; }

            [JsonProperty("CircuitTable")]
            public CircuitTable CircuitTable { get; set; }
        }
    }
}

