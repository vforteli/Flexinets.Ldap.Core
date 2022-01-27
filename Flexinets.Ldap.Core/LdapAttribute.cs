using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flexinets.Ldap.Core
{
    public class LdapAttribute
    {
        private Tag _tag;
        protected byte[] Value = new byte[0];
        public List<LdapAttribute> ChildAttributes = new List<LdapAttribute>();

        public TagClass Class => _tag.Class;
        public bool IsConstructed => _tag.IsConstructed || ChildAttributes.Any();
        public LdapOperation? LdapOperation => _tag.LdapOperation;
        public UniversalDataType? DataType => _tag.DataType;
        public byte? ContextType => _tag.ContextType;


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(LdapOperation operation)
        {
            _tag = new Tag(operation);
        }


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(LdapOperation operation, object value)
        {
            _tag = new Tag(operation);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(UniversalDataType dataType)
        {
            _tag = new Tag(dataType);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(UniversalDataType dataType, object value)
        {
            _tag = new Tag(dataType);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(byte contextType)
        {
            _tag = new Tag(contextType);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(byte contextType, object value)
        {
            _tag = new Tag(contextType);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create an attribute with tag
        /// </summary>
        /// <param name="tag"></param>
        protected LdapAttribute(Tag tag)
        {
            _tag = tag;
        }


        /// <summary>
        /// Get the byte representation of the attribute and its children
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            var attributeBytes = new List<byte>();
            BuildAttribute(attributeBytes);
            return attributeBytes.ToArray();
        }


        /// <summary>
        /// Recursively build the attribute
        /// </summary>
        /// <param name="attributeBytes"></param>
        private void BuildAttribute(List<byte> attributeBytes)
        {
            var contentBytes = new List<byte>();
            if (ChildAttributes.Any())
            {
                _tag.IsConstructed = true;
                ChildAttributes.ForEach(o => contentBytes.AddRange(o.GetBytes()));
            }
            else
            {
                contentBytes.AddRange(Value);
            }

            attributeBytes.Add(_tag.TagByte);
            attributeBytes.AddRange(Utils.IntToBerLength(contentBytes.Count));
            attributeBytes.AddRange(contentBytes);
        }


        /// <summary>
        /// Get a typed value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() => (T)Convert.ChangeType(GetValue(), typeof(T));


        /// <summary>
        /// Get an object value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            if (_tag.Class == TagClass.Universal)
            {
                switch (_tag.DataType)
                {
                    case UniversalDataType.Boolean:
                        return BitConverter.ToBoolean(Value, 0);

                    case UniversalDataType.Integer:
                        var intbytes = new byte[4];
                        Buffer.BlockCopy(Value, 0, intbytes, 4 - Value.Length, Value.Length);
                        Array.Reverse(intbytes);
                        return BitConverter.ToInt32(intbytes, 0);

                    default:
                        return Encoding.UTF8.GetString(Value, 0, Value.Length);
                }
            }

            // todo add rest if needed
            return Encoding.UTF8.GetString(Value, 0, Value.Length);
        }


        /// <summary>
        /// Convert the value to its byte form
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] GetBytes(object value) => value switch
        {
            string v => Encoding.UTF8.GetBytes(v),
            int v => BitConverter.GetBytes(v).Reverse().ToArray(),
            bool v => BitConverter.GetBytes(v),
            byte v => new byte[] { v },
            byte[] v => v,
            _ => throw new ArgumentException($"No conversion configured for {value.GetType()}"),
        };


        /// <summary>
        /// Parse the child attributes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="currentPosition"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected static List<LdapAttribute> ParseAttributes(byte[] bytes, int currentPosition, int length)
        {
            var list = new List<LdapAttribute>();
            while (currentPosition < length)
            {
                var tag = Tag.Parse(bytes[currentPosition]);
                currentPosition++;
                var attributeLength = Utils.BerLengthToInt(bytes, currentPosition, out int i);
                currentPosition += i;

                var attribute = new LdapAttribute(tag);
                if (tag.IsConstructed && attributeLength > 0)
                {
                    attribute.ChildAttributes = ParseAttributes(bytes, currentPosition, currentPosition + attributeLength);
                }
                else
                {
                    attribute.Value = new byte[attributeLength];
                    Buffer.BlockCopy(bytes, currentPosition, attribute.Value, 0, attributeLength);
                }
                list.Add(attribute);

                currentPosition += attributeLength;
            }
            return list;
        }
    }
}
