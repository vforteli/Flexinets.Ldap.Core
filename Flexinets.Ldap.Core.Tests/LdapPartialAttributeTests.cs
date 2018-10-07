using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flexinets.Ldap.Core.Tests
{
    public class LdapPartialAttributeTests
    {
        [TestCase]
        public void TestLdapPartialAttributeDescription()
        {
            var expected = "objectClass";
            var attribute = new LdapPartialAttribute(expected, "test");

            Assert.AreEqual(expected, attribute.Description);
        }


        [TestCase]
        public void TestLdapPartialAttributeSingleValue()
        {
            var expected = "test";
            var attribute = new LdapPartialAttribute("objectClass", expected);

            Assert.AreEqual(expected, attribute.Values.SingleOrDefault());
        }


        [TestCase]
        public void TestLdapPartialAttributeMultipleValues()
        {
            var expected = new List<String> { "test", "foo", "bar" };
            var attribute = new LdapPartialAttribute("objectClass", expected);

            var output = attribute.Values;
            CollectionAssert.AreEqual(expected, output);
        }
    }
}
