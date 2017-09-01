using RJBM.JsonMergePatch.Internal;
using System;
using System.Collections.Generic;

namespace RJBM.JsonMergePatch
{
    public interface IJsonMergePatchDocument
    {
        IReadOnlyDictionary<string, IJsonMergePatchValue> Members { get; }

        TTo ApplyTo<TTo>(TTo to)
            where TTo : class;

        IJsonMergePatchValue Get(string memberName);
    }
}
