using System;
using System.Collections.Generic;
using System.IO;

namespace Flexinets.Ldap.Core
{
    public class LdapPacket : LdapAttribute
    {
        public Int32 MessageId
        {
            get
            {
                return ChildAttributes[0].GetValue<Int32>();
            }
        }


        /// <summary>
        /// Create a new Ldap packet with message id
        /// </summary>
        /// <param name="messageId"></param>
        public LdapPacket(Int32 messageId) : base(UniversalDataType.Sequence, true)
        {
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.Integer, false, messageId));
        }


        /// <summary>
        /// Create a packet with tag
        /// </summary>
        /// <param name="tag"></param>
        private LdapPacket(Tag tag) : base(tag)
        {
        }


        /// <summary>
        /// Parse an ldap packet from a byte array. 
        /// Must be the complete packet
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static LdapPacket ParsePacket(Byte[] bytes)
        {
            return (LdapPacket)ParseAttributes(bytes, 0, null)[0];
        }


        /// <summary>
        /// Try parsing an ldap packet from a stream        
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Boolean TryParsePacket(Stream stream, out LdapPacket packet)
        {
            var tagByte = new Byte[1];
            stream.Read(tagByte, 0, 1);
            var tag = Tag.Parse(tagByte[0]);

            var contentLength = Utils.BerLengthToInt(stream, out int i);
            var contentBytes = new Byte[contentLength];
            stream.Read(contentBytes, 0, contentLength);

            packet = new LdapPacket(tag);
            packet.ChildAttributes.AddRange(ParseAttributes(contentBytes, 0, contentLength));
            return true;
        }


        /// <summary>
        /// Parse the child attributes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="currentPosition"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static List<LdapAttribute> ParseAttributes(Byte[] bytes, Int32 currentPosition, Int32? length)
        {
            var list = new List<LdapAttribute>();
            while (!length.HasValue || (currentPosition < length))
            {
                var tag = Tag.Parse(bytes[currentPosition]);
                currentPosition++;
                var attributeLength = Utils.BerLengthToInt(bytes, currentPosition, out int i);
                currentPosition += i;

                // This is for the first pass, ie the packet itself when the length is unknown
                if (!length.HasValue)
                {
                    length = attributeLength + currentPosition;
                }

                var attribute = new LdapPacket(tag);
                if (tag.IsConstructed && attributeLength > 0)
                {
                    attribute.ChildAttributes = ParseAttributes(bytes, currentPosition, currentPosition + attributeLength);
                }
                else
                {
                    attribute.Value = new Byte[attributeLength];
                    Buffer.BlockCopy(bytes, currentPosition, attribute.Value, 0, attributeLength);
                }
                list.Add(attribute);

                currentPosition += attributeLength;
            }
            return list;
        }
    }
}
