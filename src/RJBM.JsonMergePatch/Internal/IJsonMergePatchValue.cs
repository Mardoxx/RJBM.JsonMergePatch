using System;

namespace RJBM.JsonMergePatch.Internal
{
    public interface IJsonMergePatchValue
    {
        bool IsDefined { get; }
        object Value { get; set; }
    }
}
