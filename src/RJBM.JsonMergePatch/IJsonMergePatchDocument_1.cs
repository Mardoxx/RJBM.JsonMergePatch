using RJBM.JsonMergePatch.Internal;
using System;
using System.Linq.Expressions;

namespace RJBM.JsonMergePatch
{
    public interface IJsonMergePatchDocument<T> : IJsonMergePatchDocument
        where T : class
    {
        JsonMergePatchValue<TMember> Get<TMember>(Expression<Func<T, TMember>> expr);
    }
}
