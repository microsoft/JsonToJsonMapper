using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToJsonMapper
{
    public static class ExtensionMethods
    {        
        public static void AddRange(this Dictionary<string, object> dictA, Dictionary<string, object> dictB)
        {            
            foreach (KeyValuePair<string, object> pair in dictB)
            {
                dictA[pair.Key]  = pair.Value;
            }
        }
    }
}
