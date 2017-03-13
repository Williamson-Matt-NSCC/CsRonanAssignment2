using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NamedayDemo
{
    public static class Repository
    {
        private static List<NamedayModel> allNamedaysCache;

        public static async Task<List<NamedayModel>> GetAllNamedaysAsync()
        {
            if (allNamedaysCache != null)
            {
                return allNamedaysCache;
            }
            var hc = new HttpClient();
            var stream = await hc.GetStreamAsync(
                "http://www.response.hu/namedays_hu.json");
            var serializer = new DataContractJsonSerializer(
                typeof(List<NamedayModel>));
            allNamedaysCache = 
                (List<NamedayModel>)serializer.ReadObject(stream); 
            return allNamedaysCache;
        }
    }
}
