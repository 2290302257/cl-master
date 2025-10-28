using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Alurax;
using Object = System.Object;

public class BlackboardVariableJsonConverter : JsonConverter
{
    private Blackboard _blackboard;
    
    public BlackboardVariableJsonConverter(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var variable = value as BlackboardVariableBase;
        var curValue = variable.GetSerializedValue();
        
        JToken valueStr;
        bool isUnity = curValue.GetType().Namespace == "UnityEngine";
        if (isUnity) valueStr = JsonUtility.ToJson(curValue);
        else valueStr = JToken.FromObject(curValue, serializer);
        
        JObject jObject = new JObject
        {
            { "Type", variable.GetType().Name },
            { "Name", variable.Name },
            { "Value", valueStr },
            { "ValueType", curValue.GetType().AssemblyQualifiedName},
            { "IsUnity", isUnity }
        };
        jObject.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        var variableTypeName = jObject["Type"].Value<string>();
        string fullTypeName = $"Alurax.{variableTypeName}, Alurax";
        var valueTypeName = jObject["ValueType"].Value<string>();

        Type variableType = Type.GetType(fullTypeName);
        Type valueType = Type.GetType(valueTypeName);
        
        Object value;
        var isUnity = jObject["IsUnity"].Value<bool>();
        if (isUnity) value = JsonUtility.FromJson(jObject["Value"].Value<string>(), valueType);
        else value = jObject["Value"].ToObject(valueType, serializer);

        string name = jObject["Name"].Value<string>();
        var variable = _blackboard.SetVariable(name, variableType, value);
        
        if (variableType == null || valueType == null)
        {
            Log.E($"Failed to load {name} with type {variableTypeName} or value type {valueTypeName}");
        }
        
        return variable;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(BlackboardVariableBase).IsAssignableFrom(objectType);
    }
    
}
