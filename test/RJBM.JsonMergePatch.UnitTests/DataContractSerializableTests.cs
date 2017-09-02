using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace RJBM.JsonMergePatch.UnitTests
{
    public class DataContractSerializableTests
    {
        [DataContract]
        public class TestObject
        {
            [DataMember]
            public int Int1 { get; set; }

            [DataMember]
            public int? NullableInt1 { get; set; }

            [DataMember]
            public int? NullableInt2 { get; set; }

            [DataMember]
            public string String1 { get; set; }
        }

        [Fact]
        public void Serializes_and_deserializes_without_throwing_up()
        {
            var patchDoc = new JsonMergePatchDocument<TestObject>();

            DataContractSerializer s = new DataContractSerializer(patchDoc.GetType());
            Stream stream = new MemoryStream();
            s.WriteObject(stream, patchDoc);

            stream.Position = 0;
            var other = (JsonMergePatchDocument<TestObject>)s.ReadObject(stream);
        }

        [Fact]
        public void Deserializes_values()
        {
            var patchDoc = new JsonMergePatchDocument<TestObject>();
            patchDoc.Get(x => x.Int1).Value = 10;
            patchDoc.Get(x => x.String1).Value = "Initial1";
            patchDoc.Get(x => x.NullableInt2).Value = null;

            DataContractSerializer s = new DataContractSerializer(patchDoc.GetType());
            Stream stream = new MemoryStream();
            s.WriteObject(stream, patchDoc);

            stream.Position = 0;
            var other = (JsonMergePatchDocument<TestObject>)s.ReadObject(stream);

            Assert.Equal(10, other.Get(x => x.Int1).Value);
            Assert.False(other.Get(x => x.NullableInt1).IsDefined);
            Assert.True(other.Get(x => x.NullableInt2).IsDefined);
            Assert.Null(other.Get(x => x.NullableInt2).Value);
            Assert.Equal("Initial1", other.Get(x => x.String1).Value);
        }
    }
}
