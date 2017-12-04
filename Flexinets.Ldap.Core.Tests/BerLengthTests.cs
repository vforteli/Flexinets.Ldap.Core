using NUnit.Framework;
using System;
using System.Collections;

namespace Flexinets.Ldap.Core.Tests
{
    public class BerLengthTests
    {
        [TestCase(1, "10000000")]
        [TestCase(127, "11111110")]
        [TestCase(128, "10000001 00000001")]
        [TestCase(256, "01000001 00000000 10000000")]
        [TestCase(65536, "11000001 00000000 00000000 10000000")]
        [TestCase(255, "10000001 11111111")]
        public void TestIntToBerLength(Int32 input, String expected)
        {
            var berlength = Utils.IntToBerLength(input);
            Assert.AreEqual(expected, Utils.BitsToString(new BitArray(berlength)));
        }


        [TestCase]
        public void TestBerLengthLongNotation5()
        {
            var bytes = Utils.StringToByteArray("300c02010161070a010004000400");
            var position = 1;
            var intLength = Utils.BerLengthToInt(bytes, 1, out position);

            Assert.AreEqual(12, intLength);
        }


        [TestCase]
        public void TestBerLengthLongNotation6()
        {
            var bytes = Utils.StringToByteArray("300c02010161070a010004000400");
            var position = 1;
            var intLength = Utils.BerLengthToInt(bytes, 3, out position);

            Assert.AreEqual(1, intLength);
        }
    }
}
