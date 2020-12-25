using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnusualActivity.Common
{

    public class UOActivityDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string USymbol { get; set; }
        public string OSymbol { get; set; }

        public string Strike { get; set; }
        public string StringExpirationDate { get; set; }
        public string ContractType { get; set; }
        public long ExpirationDate { get; set; }
        public string Description { get; set; }
        public long TotalVolume { get; set; }
        public long OpenInterest { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Theta { get; set; }
        public double Vega { get; set; }
        public double Rho { get; set; }
        public DateTime Expiry
        {
            get
            {
                try
                {
                    var dateExp = this.StringExpirationDate.Split(":")[0];
                    return Convert.ToDateTime(dateExp);
                }
                catch (Exception ex)
                {

                }
                return DateTime.MinValue;
            }
        }

        public DateTime LastRunDateTime { get; set; }
        public long VolumeAtLastRun { get; set; }
        
        public DateTime PreviousRunDateTime { get; set; }
        public long VolumeAtPreviousRun { get; set; }

        public long VolumeDelta { get; set; }

        public long PreviousOpenInterest { get; set; }
        public long PreviousOpenInterest1 { get; set; }
        public long PreviousOpenInterest2 { get; set; }
        public long PreviousOpenInterest3 { get; set; }


    }

    public class Security
    {
        [JsonProperty("symbol")]
        public string USymbol { get; set; }
        public double UnderlyingPrice { get; set; }
        [JsonProperty("putExpDateMap")]
        public Dictionary<string, Dictionary<string, List<OptionContract>>> Puts { get; set; }
        [JsonProperty("callExpDateMap")]
        public Dictionary<string, Dictionary<string, List<OptionContract>>> Calls { get; set; }

    }

    public class Expirations
    {
        public Dictionary<string, Object> Strikes { get; set; }
        //public Object Strikes { get; set; }

    }

    public class Strike
    {
        public Dictionary<string, OptionContract> ContractDetails { get; set; }
    }

    public class OptionContract
    {
        [JsonProperty("symbol")]
        public string OSymbol { get; set; }
        public long ExpirationDate { get; set; }
        public string Description { get; set; }
        public long TotalVolume { get; set; }
        public long OpenInterest { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Theta { get; set; }
        public double Vega { get; set; }
        public double Rho { get; set; }
    }
}
