using System.Runtime.Serialization;

namespace sstocker.core.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, decimal y, decimal percentage)
        {
            Label = label;
            Y = y;
            Percentage = percentage;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "percentage")]
        public decimal? Percentage = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public decimal? Y = null;
    }
}
