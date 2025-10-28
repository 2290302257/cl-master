using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Alurax
{
    public static class BlackboardSerializeUtil
    {
        public static string Serialize(Blackboard bb)
        {
            if (bb == null) return string.Empty;
            var bbstr = JsonConvert.SerializeObject(bb.Variables,new BlackboardVariableJsonConverter(bb));
            return bbstr;
        }
        
        public static Blackboard Deserialize(string json,Object owner)
        {
            if (string.IsNullOrEmpty(json)) return null;
            
            Blackboard bb = new Blackboard(owner);
            JsonConvert.DeserializeObject<List<BlackboardVariableBase>>(json,new BlackboardVariableJsonConverter(bb));
            return bb;
        }
    }
}