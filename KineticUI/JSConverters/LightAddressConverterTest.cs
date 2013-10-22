using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using KineticControl;
using NUnit.Framework;
using Newtonsoft.Json;

namespace KineticUI.JSConverters
{
    [TestFixture]
    public class LightAddressConverterTest
    {

        private static readonly LightAddress TestAddress = new LightAddress(1,2,3);

        [Test]
        public void TestStreamSerialization()
        {
            Assert.Greater( Serializer.ToStream(TestAddress).Length, 20);
        }

        [Test]
        public void TestStringSerialization()
        {
            Assert.Greater( Serializer.ToString(TestAddress).Length, 20);
        }

        [Test]
        public void TestStreamDeserialization()
        {
            Stream serialized = Serializer.ToStream(TestAddress);
            LightAddress deserialized = Serializer.FromStream<LightAddress>(serialized);
            Assert.AreEqual(TestAddress, deserialized);
        }

        [Test]
        public void TestStringDeserialization()
        {
            string serialized = Serializer.ToString(TestAddress);
            var deserialized = Serializer.FromString<LightAddress>(serialized);
            Assert.AreEqual(TestAddress, deserialized);
        }

    }
}