using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RJBM.JsonMergePatch.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RJBM.JsonMergePatch.Converters
{
    public class JsonMergePatchDocumentJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // (JsonMergePatchDocument)(new JsonMergePatchDocument<T>());
            IJsonMergePatchDocument mergePatch = (IJsonMergePatchDocument)(objectType.GetConstructor(Type.EmptyTypes).Invoke(null));
            // T
            Type mergePatchTypeArg = mergePatch.GetType().GetTypeInfo().GenericTypeArguments[0];

            JObject jObject = JObject.Load(reader);

            PropertyInfo[] mergePatchProps = mergePatchTypeArg.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            for (int i = 0; i < mergePatchProps.Length; ++i)
            {
                PropertyInfo prop = mergePatchProps[i];

                JToken val;
                if (!jObject.TryGetValue(prop.Name, StringComparison.Ordinal, out val)
                    && !jObject.TryGetValue(prop.Name, StringComparison.OrdinalIgnoreCase, out val))
                {
                    continue;
                };

                mergePatch.Get(prop.Name).Value = val.ToObject(prop.PropertyType);
            }

            return mergePatch;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jObject = new JObject();

            KeyValuePair<string, IJsonMergePatchValue>[] setMembers = ((IJsonMergePatchDocument)value).Members.Where(x => x.Value.IsDefined).ToArray();

            for (int i = 0; i < setMembers.Length; ++i)
            {
                KeyValuePair<string, IJsonMergePatchValue> member = setMembers[i];

                if (member.Value.Value == null)
                {
                    jObject.Add(member.Key, null);
                }
                else
                {
                    jObject.Add(member.Key, JToken.FromObject(member.Value.Value, serializer));
                }
            }

            jObject.WriteTo(writer);
        }
    }
}
