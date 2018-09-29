using NUnit.Framework;
using System;
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
