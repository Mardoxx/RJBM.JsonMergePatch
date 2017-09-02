using System;
using System.Runtime.Serialization;

namespace RJBM.JsonMergePatch.Internal
{
    [DataContract]
    [KnownType(nameof(KnownTypes))]
    public class JsonMergePatchValue<T> : IJsonMergePatchValue<T>
    {
        [DataMember(Name = "Value", Order = 0)]
        private T _value;
        [DataMember(Name = "IsDefined", Order = 1)]
        private bool _isDefined;

        public JsonMergePatchValue()
        {
            _isDefined = false;
            _value = default(T);
        }

        public JsonMergePatchValue(T value) : this()
        {
            _value = value;
            _isDefined = true;
        }

        public bool IsDefined => _isDefined;

        public T Value
        {
            get
            {
                if (_isDefined)
                {
                    return _value;
                }

                return default(T);
            }

            set
            {
                _isDefined = true;
                _value = value;
            }
        }

        object IJsonMergePatchValue.Value
        {
            get => Value;
            set => Value = (T)value;
        }

        public override bool Equals(object obj)
        {
            if (!_isDefined)
            {
                return obj == null;
            }

            if (obj == null)
            {
                return false;
            }

            return _value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _isDefined ? _value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return _isDefined ? _value.ToString() : "";
        }

        private static Type[] KnownTypes()
        {
            return new Type[] { typeof(T) };
        }
    }
}