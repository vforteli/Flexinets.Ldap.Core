using System;
using System.Collections;

namespace Flexinets.Ldap.Core
{
    public class Tag
    {
        /// <summary>
        /// Tag in byte form
        /// </summary>
        public Byte TagByte { get; internal set; }


        public Boolean IsConstructed
        {
            get
            {
                return new BitArray(new byte[] { TagByte }).Get(5);
            }
        }


        public TagClass Class
        {
            get
            {
                return (TagClass)(TagByte >> 6);
            }
        }


        public UniversalDataType DataType
        {
            get
            {
                return (UniversalDataType)(TagByte & 31);
            }
        }


        public LdapOperation LdapOperation
        {
            get
            {
                return (LdapOperation)(TagByte & 31);
            }
        }


        public Byte ContextType
        {
            get
            {
                return (byte)(TagByte & 31);
            }
        }


        /// <summary>
        /// Create an application tag
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isSequence"></param>
        public Tag(LdapOperation operation, Boolean isSequence)
        {
            TagByte = (byte)((byte)operation + (Convert.ToByte(isSequence) << 5) + ((byte)TagClass.Application << 6));
        }


        /// <summary>
        /// Create a universal tag
        /// </summary>
        /// <param name="isSequence"></param>
        /// <param name="operation"></param>
        public Tag(UniversalDataType dataType, Boolean isSequence)
        {
            TagByte = (byte)((byte)dataType + (Convert.ToByte(isSequence) << 5) + ((byte)TagClass.Universal << 6));
        }


        /// <summary>
        /// Create a context tag
        /// </summary>
        /// <param name="isSequence"></param>
        /// <param name="operation"></param>
        public Tag(Byte context, Boolean isSequence)
        {
            TagByte = (byte)((byte)context + (Convert.ToByte(isSequence) << 5) + ((byte)TagClass.Context << 6));
        }


        /// <summary>
        /// Parses a raw tag byte
        /// </summary>
        /// <param name="tagByte"></param>
        /// <returns></returns>
        public static Tag Parse(Byte tagByte)
        {
            return new Tag(tagByte);
        }


        private Tag(Byte tagByte)
        {
            TagByte = tagByte;
        }
    }
}
