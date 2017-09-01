using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace RJBM.JsonMergePatch.UnitTests
{
    public class ChildObject
    {
        public Guid Guid1 { get; set; }
    }
    public class ParentObject
    {
        public int Int1 { get; set; }
        public int Int2 { get; set; }
        public int? NullableInt1 { get; set; }
        public string String1 { get; set; }
    }

    public class SomeDTO
    {
        public int Int1 { get; set; }
        public int Int2 { get; set; }
        public string String1 { get; set; }
        public string String2 { get; set; }
    }

    public class UnitTests
    {
        [Fact]
        public void Excludes_Undefined_Values_In_Serialization()
        {
            var patchDocument = new JsonMergePatchDocument<ParentObject>();
            patchDocument.Get(x => x.Int1);

            var serialized = JsonConvert.SerializeObject(patchDocument, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            });

            Assert.DoesNotContain(serialized, nameof(ParentObject.String1), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Defines_Members_When_Deserializing()
        {
            var serialized = @"{Int1: 12}";

            var deserialized = JsonConvert.DeserializeObject<JsonMergePatchDocument<ParentObject>>(serialized);

            Assert.True(deserialized.Get(x => x.Int1).IsDefined);
            Assert.Equal(1, deserialized.Members.Count(x => x.Value.IsDefined));
        }

        [Fact]
        public void Defines_Null_Members_When_Deserializing()
        {
            var serialized = @"{Int1: 12, NullableInt1: null}";

            var deserialized = JsonConvert.DeserializeObject<JsonMergePatchDocument<ParentObject>>(serialized);

            Assert.True(deserialized.Get(x => x.Int1).IsDefined);
            Assert.True(deserialized.Get(x => x.NullableInt1).IsDefined);
            Assert.Equal(2, deserialized.Members.Count(x => x.Value.IsDefined));
            Assert.Null(deserialized.Get(x => x.NullableInt1).Value);
        }

        [Fact]
        public void Does_Not_Define_Undefined_Values_When_Deseralizing()
        {
            var serialized = @"{Int1: 12}";

            var deserialized = JsonConvert.DeserializeObject<JsonMergePatchDocument<ParentObject>>(serialized);

            Assert.False(deserialized.Get(x => x.Int2).IsDefined);
        }

        [Fact]
        public void Only_Maps_Set_Members()
        {
            var dto = new SomeDTO()
            {
                Int1 = 1,
                Int2 = 2,
                String1 = "Initial1",
                String2 = "Initial2"
            };

            var patchDocument = new JsonMergePatchDocument<ParentObject>();
            patchDocument.Get(x => x.Int1).Value = 1337;
            patchDocument.Get(x => x.String1).Value = null;

            var result = patchDocument.ApplyTo(dto);

            Assert.Equal(1337, dto.Int1);
            Assert.Equal(2, dto.Int2);
            Assert.Null(dto.String1);
            Assert.Equal("Initial2", dto.String2);
        }
    }
}
