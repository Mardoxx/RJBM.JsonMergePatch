using System;

namespace RJBM.JsonMergePatch.Internal
{
    public interface IJsonMergePatchValue<T> : IJsonMergePatchValue
    {
        new T Value { get; set; }
    }
}
