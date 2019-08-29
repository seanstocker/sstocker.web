using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.core.Models
{
    public class ChartModel
    {
        public string Title;
        public List<DataPoint> DataPoints;
        public string DataPointsString { get { return JsonConvert.SerializeObject(DataPoints); } }
        public string DisplayChart { get { return DataPoints.Any().ToString().ToLowerInvariant(); } }

        public ChartModel()
        {
            DataPoints = new List<DataPoint>();
        }
    }
}
