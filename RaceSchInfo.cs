using Newtonsoft.Json;
namespace RaceBT_
{
    public class RaceSchInfo
    {
        public class RaceData
        {
            [JsonProperty("MRData")]
            public MRData MRData { get; set; }
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

            [JsonProperty("RaceTable")]
            public RaceTable RaceTable { get; set; }
        }
        public class RaceTable
        {
            [JsonProperty("season")]
            public string Season { get; set; }

            [JsonProperty("Races")]
            public List<Race> Races { get; set; }
        }
        public class Race
        {
            [JsonProperty("season")]
            public string Season { get; set; }

            [JsonProperty("round")]
            public string Round { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("raceName")]
            public string RaceName { get; set; }

            [JsonProperty("Circuit")]
            public Circuit Circuit { get; set; }

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }

            [JsonProperty("FirstPractice")]
            public Practice FirstPractice { get; set; }

            [JsonProperty("SecondPractice")]
            public Practice SecondPractice { get; set; }

            [JsonProperty("ThirdPractice")]
            public Practice ThirdPractice { get; set; }

            [JsonProperty("Qualifying")]
            public Practice Qualifying { get; set; }
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
        public class Location
        {
            [JsonProperty("lat")]
            public string Lat { get; set; }

            [JsonProperty("long")]
            public string Long { get; set; }

            [JsonProperty("locality")]
            public string Locality { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }
        }
        public class Practice
        {
            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }
        }
    }
}
