using NUnit.Framework;
using System;

namespace Flexinets.Ldap.Core.Tests
{
    public class LdapAttributeTests
    {
        [TestCase]
        public void TestLdapAttributeGetBytes()
        {
            var attribute = new LdapAttribute(UniversalDataType.Integer, false, (Byte)1);
            Assert.AreEqual("020101", Utils.ByteArrayToString(attribute.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeGetBytes2()
        {
            var attribute = new LdapAttribute(UniversalDataType.Integer, false, (Byte)2);
            Assert.AreEqual("020102", Utils.ByteArrayToString(attribute.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeGetBytesMaxInt()
        {
            var attribute = new LdapAttribute(UniversalDataType.Integer, false, Int32.MaxValue);
            Assert.AreEqual(Int32.MaxValue, attribute.GetValue<Int32>());
        }


        [TestCase]
        public void TestLdapAttributeGetBytesBoolean()
        {
            var attribute = new LdapAttribute(UniversalDataType.Boolean, false, true);
            Assert.AreEqual("010101", Utils.ByteArrayToString(attribute.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeGetBytesBoolean2()
        {
            var attribute = new LdapAttribute(UniversalDataType.Boolean, false, false);
            Assert.AreEqual("010100", Utils.ByteArrayToString(attribute.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeGetBytesString()
        {
            var attribute = new LdapAttribute(UniversalDataType.OctetString, false, "dc=karakorum,dc=net");
            Assert.AreEqual("041364633d6b6172616b6f72756d2c64633d6e6574", Utils.ByteArrayToString(attribute.GetBytes()));
        }
   

        [TestCase]
        public void TestAttributeClass()
        {
            var attribute = new LdapAttribute(LdapOperation.BindRequest, true);
            Assert.IsNull(attribute.DataType);
        }

        [TestCase]
        public void TestAttributeClass2()
        {
            var attribute = new LdapAttribute(LdapOperation.BindRequest, true);
            Assert.AreEqual(LdapOperation.BindRequest, attribute.LdapOperation);
        }
    }
}
