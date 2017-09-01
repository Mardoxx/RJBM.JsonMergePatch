using Newtonsoft.Json;
using RJBM.JsonMergePatch.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RJBM.JsonMergePatch
{
    [JsonConverter(typeof(Converters.JsonMergePatchDocumentJsonConverter))]
    public class JsonMergePatchDocument<T> : IJsonMergePatchDocument<T>
        where T : class
    {
        private readonly ReadOnlyDictionary<string, IJsonMergePatchValue> _members;

        public JsonMergePatchDocument()
        {
            PropertyInfo[] typeProperties = typeof(T).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            var members = new Dictionary<string, IJsonMergePatchValue>();

            for (int i = 0; i < typeProperties.Length; ++i)
            {
                PropertyInfo propInfo = typeProperties[i];

                // T
                Type mergePatchValueTypeArg = propInfo.PropertyType;
                // JsonMergePatchValue<T>;
                Type mergePatchValueType = typeof(JsonMergePatchValue<>).MakeGenericType(mergePatchValueTypeArg);
                // (IJsonMergePatchValue) new JsonMergePatchValue<T>();
                IJsonMergePatchValue mergePatchValue = (IJsonMergePatchValue)mergePatchValueType.GetConstructor(Type.EmptyTypes).Invoke(null);

                members.Add(propInfo.Name, mergePatchValue);
            }

            _members = new ReadOnlyDictionary<string, IJsonMergePatchValue>(members);
        }

        public IReadOnlyDictionary<string, IJsonMergePatchValue> Members => _members;

        public IJsonMergePatchValue Get(string memberName)
        {
            return Members.First(x => x.Key == memberName).Value;
        }

        public TTo ApplyTo<TTo>(TTo to)
            where TTo : class
        {
            TypeInfo typeInfo = typeof(TTo).GetTypeInfo();

            PropertyInfo[] typeProperties = typeInfo.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            string[] typePropertyNames = typeProperties.Select(x => x.Name).ToArray();

            var validMemberNames = _members.Keys.Where(x => typePropertyNames.Contains(x)).ToArray();

            for (int i = 0; i < validMemberNames.Length; ++i)
            {
                string memberName = validMemberNames[i];
                var member = Get(memberName);
                if (member.IsDefined)
                {
                    typeInfo.GetProperty(memberName).SetValue(to, member.Value);
                }
            }

            return to;
        }

        public JsonMergePatchValue<TMember> Get<TMember>(Expression<Func<T, TMember>> expr)
        {
            MemberExpression memberExpression = expr.Body as MemberExpression ?? throw new ArgumentException($"Expression '{expr}' refers to a method, not a property.");

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo ?? throw new ArgumentException($"Expression '{expr}' refers to a field, not a property.");

            return (JsonMergePatchValue<TMember>)Get(propertyInfo.Name);
        }
    }
}
