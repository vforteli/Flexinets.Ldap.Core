using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Flexinets.Ldap.Core.Tests
{
    public class LdapPacketTests
    {
        [TestCase]
        public void TestLdapAttributeSequenceGetBytesString()
        {
            var packet = new LdapPacket(1);

            var bindrequest = new LdapAttribute(LdapOperation.BindRequest);
            bindrequest.ChildAttributes.Add(new LdapAttribute(UniversalDataType.Integer, (Byte)3));
            bindrequest.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "cn=bindUser,cn=Users,dc=dev,dc=company,dc=com"));
            bindrequest.ChildAttributes.Add(new LdapAttribute((byte)0, "bindUserPassword"));

            packet.ChildAttributes.Add(bindrequest);

            var expected = "304c0204000000016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264"; // "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeSequenceGetBytes2()
        {
            var packet = new LdapPacket(1);

            var bindresponse = new LdapAttribute(LdapOperation.BindResponse);

            var resultCode = new LdapAttribute(UniversalDataType.Enumerated, (Byte)LdapResult.success);
            bindresponse.ChildAttributes.Add(resultCode);

            var matchedDn = new LdapAttribute(UniversalDataType.OctetString);
            var diagnosticMessage = new LdapAttribute(UniversalDataType.OctetString);

            bindresponse.ChildAttributes.Add(matchedDn);
            bindresponse.ChildAttributes.Add(diagnosticMessage);

            packet.ChildAttributes.Add(bindresponse);

            var expected = "300f02040000000161070a010004000400"; // "300c02010161070a010004000400";
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeSequenceGetBytesShortcut()
        {
            var packet = new LdapPacket(1);
            var bindresponse = new LdapResultAttribute(LdapOperation.BindResponse, LdapResult.success);
            packet.ChildAttributes.Add(bindresponse);

            var expected = "300f02040000000161070a010004000400"; // "300c02010161070a010004000400";
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParse()
        {
            var expected = "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            var packetBytes = Utils.StringToByteArray(expected);
            var packet = LdapPacket.ParsePacket(packetBytes);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParse3()
        {
            var expected = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e32";
            var packetBytes = Utils.StringToByteArray(expected);
            var packet = LdapPacket.ParsePacket(packetBytes);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParse4()
        {
            var bytes = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e3200000000";
            var expected = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e32";
            var packetBytes = Utils.StringToByteArray(bytes);
            var packet = LdapPacket.ParsePacket(packetBytes);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParseFromStream4()
        {
            var bytes = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e3200000000";
            var expected = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e32";
            var packetBytes = Utils.StringToByteArray(bytes);
            var stream = new MemoryStream(packetBytes);
            LdapPacket.TryParsePacket(stream, out var packet);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestPacketMessageId()
        {
            var packet = new LdapPacket(Int32.MaxValue);
            Assert.AreEqual(Int32.MaxValue, packet.MessageId);
        }


        [TestCase]
        public void TestPacketParsingBindRequest()
        {
            var bytes = "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            var expected = "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            var packetBytes = Utils.StringToByteArray(bytes);
            var stream = new MemoryStream(packetBytes);
            LdapPacket.TryParsePacket(stream, out var packet);
            RecurseAttributes(packet);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestPacketStuff()
        {
            var expected = "3084000000800204000000016478042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d3047301804037569643111040f75736572756964676f657368657265302b040b6f626a656374436c617373311c040c616161616161616161616161040c626262626262626262626262";
            var responseEntryPacket = new LdapPacket(1);
            var searchResultEntry = new LdapAttribute(LdapOperation.SearchResultEntry);
            searchResultEntry.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "cn=bindUser,cn=Users,dc=dev,dc=company,dc=com"));   //  objectName

            var partialAttributeList = new LdapAttribute(UniversalDataType.Sequence);



            var partialAttributeUid = new LdapAttribute(UniversalDataType.Sequence);
            partialAttributeUid.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "uid"));   // type
            var partialAttributeUidValues = new LdapAttribute(UniversalDataType.Set);
            partialAttributeUidValues.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "useruidgoeshere"));
            partialAttributeUid.ChildAttributes.Add(partialAttributeUidValues);
            partialAttributeList.ChildAttributes.Add(partialAttributeUid);

            var partialAttributeObjectClass = new LdapAttribute(UniversalDataType.Sequence);
            partialAttributeObjectClass.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "objectClass"));   // type
            var partialAttributeObjectClassValues = new LdapAttribute(UniversalDataType.Set);
            partialAttributeObjectClassValues.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "aaaaaaaaaaaa"));
            partialAttributeObjectClassValues.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "bbbbbbbbbbbb"));
            partialAttributeObjectClass.ChildAttributes.Add(partialAttributeObjectClassValues);
            partialAttributeList.ChildAttributes.Add(partialAttributeObjectClass);

            searchResultEntry.ChildAttributes.Add(partialAttributeList);
            responseEntryPacket.ChildAttributes.Add(searchResultEntry);
            var responsEntryBytes = responseEntryPacket.GetBytes();

            Console.WriteLine(Utils.ByteArrayToString(responsEntryBytes));


            var packet = LdapPacket.ParsePacket(responsEntryBytes);
            RecurseAttributes(packet);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestPacketPartialAttribute()
        {
            var expected = "3084000000800204000000016478042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d3047301804037569643111040f75736572756964676f657368657265302b040b6f626a656374436c617373311c040c616161616161616161616161040c626262626262626262626262";
            var responseEntryPacket = new LdapPacket(1);
            var searchResultEntry = new LdapAttribute(LdapOperation.SearchResultEntry);
            searchResultEntry.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "cn=bindUser,cn=Users,dc=dev,dc=company,dc=com"));   //  objectName

            var partialAttributeList = new LdapAttribute(UniversalDataType.Sequence);



            partialAttributeList.ChildAttributes.Add(new LdapPartialAttribute("uid", "useruidgoeshere"));
            partialAttributeList.ChildAttributes.Add(new LdapPartialAttribute("objectClass", new List<String> { "aaaaaaaaaaaa", "bbbbbbbbbbbb" }));

            searchResultEntry.ChildAttributes.Add(partialAttributeList);
            responseEntryPacket.ChildAttributes.Add(searchResultEntry);
            var responsEntryBytes = responseEntryPacket.GetBytes();

            Console.WriteLine(Utils.ByteArrayToString(responsEntryBytes));


            var packet = LdapPacket.ParsePacket(responsEntryBytes);
            RecurseAttributes(packet);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        /// <summary>
        /// Example from https://wiki.wireshark.org/SampleCaptures?action=AttachFile&do=get&target=ldap-krb5-sign-seal-01.cap
        /// Parse and rebuild a packet
        /// </summary>
        [TestCase]
        public void TestPacketParsingmorethan255()
        {
            var bytes = "308400000159020200d563840000014f04000a01000a0100020100020178010100870b6f626a656374636c61737330840000012b0411737562736368656d61537562656e747279040d6473536572766963654e616d65040e6e616d696e67436f6e7465787473041464656661756c744e616d696e67436f6e746578740413736368656d614e616d696e67436f6e74657874041a636f6e66696775726174696f6e4e616d696e67436f6e746578740417726f6f74446f6d61696e4e616d696e67436f6e746578740410737570706f72746564436f6e74726f6c0414737570706f727465644c44415056657273696f6e0415737570706f727465644c444150506f6c69636965730417737570706f727465645341534c4d656368616e69736d73040b646e73486f73744e616d65040f6c646170536572766963654e616d65040a7365727665724e616d650415737570706f727465644361706162696c6974696573";
            var expected = bytes;
            var packetBytes = Utils.StringToByteArray(bytes);
            var stream = new MemoryStream(packetBytes);
            LdapPacket.TryParsePacket(stream, out var packet);
            RecurseAttributes(packet);
            var output = Utils.ByteArrayToString(packet.GetBytes());
            Console.WriteLine(bytes);
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }


        /// <summary>
        /// Example from https://wiki.wireshark.org/SampleCaptures?action=AttachFile&do=get&target=ldap-krb5-sign-seal-01.cap
        /// Parse and rebuild a packet
        /// </summary>
        [TestCase]
        public void TestPacketThunderbirdSearch()
        {
            var bytes = "30840000048e020102638400000485041164633d6578616d706c652c64633d636f6d0a01020a0100020164020100010100a08400000105a184000000ffa40d0402636e30078105686d6d6d6da4140409676976656e4e616d6530078105686d6d6d6da40d0402736e30078105686d6d6d6da41a040f6d6f7a696c6c614e69636b6e616d6530078105686d6d6d6da40f04046d61696c30078105686d6d6d6da41d04126d6f7a696c6c615365636f6e64456d61696c30078105686d6d6d6da018a416040b6465736372697074696f6e30078105686d6d6d6da40c04016f30078105686d6d6d6da40d04026f7530078105686d6d6d6da41004057469746c6530078105686d6d6d6da419040e6d6f7a696c6c61576f726b55726c30078105686d6d6d6da419040e6d6f7a696c6c61486f6d6555726c30078105686d6d6d6d30840000035204057469746c650402736e04077375726e616d6504176d6f7a696c6c61486f6d654c6f63616c6974794e616d650402636e040a636f6d6d6f6e6e616d650409676976656e4e616d6504106d6f7a696c6c61486f6d65537461746504046d61696c04016f0407636f6d70616e7904126d6f7a696c6c61486f6d6553747265657432040f6d6f7a696c6c614e69636b6e616d650410786d6f7a696c6c616e69636b6e616d6504066d6f62696c65040963656c6c70686f6e65040863617270686f6e65040f6d6f6469667974696d657374616d7004076e7341494d696404116e73637061696d73637265656e6e616d65040f74656c6570686f6e654e756d6265720409626972746879656172040163040b636f756e7472796e616d6504116d6f7a696c6c61486f6d65537472656574040a706f7374616c436f646504037a6970040e6d6f7a696c6c61437573746f6d310407637573746f6d3104166d6f7a696c6c61486f6d65436f756e7472794e616d65040273740406726567696f6e040e6d6f7a696c6c61437573746f6d320407637573746f6d32040e6d6f7a696c6c61486f6d6555726c0407686f6d6575726c04126d6f7a696c6c61576f726b5374726565743204126d6f7a696c6c615365636f6e64456d61696c0413786d6f7a696c6c617365636f6e64656d61696c041866616373696d696c6574656c6570686f6e656e756d6265720403666178040b6465736372697074696f6e04056e6f746573040e6d6f7a696c6c61437573746f6d330407637573746f6d330409686f6d6550686f6e6504126d6f7a696c6c6155736548746d6c4d61696c0413786d6f7a696c6c6175736568746d6c6d61696c040862697274686461790406737472656574040d73747265657461646472657373040d706f73744f6666696365426f78040e6d6f7a696c6c61437573746f6d340407637573746f6d3404016c04086c6f63616c69747904057061676572040a706167657270686f6e6504026f75040a6465706172746d656e7404106465706172746d656e746e756d62657204076f7267756e6974040a62697274686d6f6e7468040e6d6f7a696c6c61576f726b55726c0407776f726b75726c040a6c6162656c656455524904156d6f7a696c6c61486f6d65506f7374616c436f6465040b6f626a656374436c617373";
            var expected = bytes;
            var packetBytes = Utils.StringToByteArray(bytes);
            var stream = new MemoryStream(packetBytes);
            LdapPacket.TryParsePacket(stream, out var packet);
            RecurseAttributes(packet);
            var output = Utils.ByteArrayToString(packet.GetBytes());
            Console.WriteLine(bytes);
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }






        private void RecurseAttributes(LdapAttribute attribute, Int32 depth = 1)
        {
            if (attribute != null)
            {
                Console.WriteLine($"{Utils.Repeat(">", depth)} {attribute.Class}:{attribute.DataType}{attribute.LdapOperation}{attribute.ContextType} - Type: {attribute.GetValue().GetType()} - {attribute.GetValue()}");

                if (attribute.IsConstructed)
                {
                    foreach (var attr in attribute.ChildAttributes)
                    {
                        RecurseAttributes(attr, depth + 1);
                    }
                }
            }
        }

    }
}
