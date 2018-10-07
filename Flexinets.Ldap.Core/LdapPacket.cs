using System;
using System.Diagnostics;
using System.IO;

namespace Flexinets.Ldap.Core
{
    public class LdapPacket : LdapAttribute
    {        
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
            var packet = new LdapPacket(Tag.Parse(bytes[0]));
            var contentLength = Utils.BerLengthToInt(bytes, 1, out var lengthBytesCount);
            packet.ChildAttributes.AddRange(ParseAttributes(bytes, 1 + lengthBytesCount, contentLength));
            return packet;
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
                    var contentLength = Utils.BerLengthToInt(stream, out int n);
                    var contentBytes = new Byte[contentLength];
                    stream.Read(contentBytes, 0, contentLength);

                    packet = new LdapPacket(Tag.Parse(tagByte[0]));
                    packet.ChildAttributes.AddRange(ParseAttributes(contentBytes, 0, contentLength));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not parse packet from stream {ex.Message}");                
            }

            packet = null;
            return false;
        }
    }
}
