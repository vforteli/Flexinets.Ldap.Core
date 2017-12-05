using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace Flexinets.Ldap.Core
{
    public class LdapPacket : LdapAttribute
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LdapPacket));
        public Int32 MessageId => ChildAttributes[0].GetValue<Int32>();


        /// <summary>
        /// Create a new Ldap packet with message id
        /// </summary>
        /// <param name="messageId"></param>
        public LdapPacket(Int32 messageId) : base(UniversalDataType.Sequence)
        {
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.Integer, messageId));
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
            var tag = Tag.Parse(bytes[0]);
            var contentLength = Utils.BerLengthToInt(bytes, 1, out var lengthBytesCount);
            return (LdapPacket)ParseAttributes(bytes, 0, contentLength + lengthBytesCount + 1)[0];
        }


        /// <summary>
        /// Try parsing an ldap packet from a stream        
        /// </summary>      
        /// <param name="stream"></param>
        /// <param name="packet"></param>
        /// <returns>True if succesful. False if parsing fails or stream is empty</returns>
        public static Boolean TryParsePacket(Stream stream, out LdapPacket packet)
        {
            try
            {
                var tagByte = new Byte[1];
                var i = stream.Read(tagByte, 0, 1);
                if (i != 0)
                {
                    var tag = Tag.Parse(tagByte[0]);

                    var contentLength = Utils.BerLengthToInt(stream, out int n);
                    var contentBytes = new Byte[contentLength];
                    stream.Read(contentBytes, 0, contentLength);

                    packet = new LdapPacket(tag);
                    packet.ChildAttributes.AddRange(ParseAttributes(contentBytes, 0, contentLength));
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Could not parse packet from stream", ex);
            }

            packet = null;
            return false;
        }


        /// <summary>
        /// Parse the child attributes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="currentPosition"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static List<LdapAttribute> ParseAttributes(Byte[] bytes, Int32 currentPosition, Int32 length)
        {
            var list = new List<LdapAttribute>();
            while (currentPosition < length)
            {
                var tag = Tag.Parse(bytes[currentPosition]);
                currentPosition++;
                var attributeLength = Utils.BerLengthToInt(bytes, currentPosition, out int i);
                currentPosition += i;

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
