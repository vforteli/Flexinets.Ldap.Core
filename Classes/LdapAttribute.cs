using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flexinets.Ldap.Core
{
    public class LdapAttribute
    {
        protected Tag _tag;
        protected Byte[] Value = new Byte[0];
        public List<LdapAttribute> ChildAttributes = new List<LdapAttribute>();

        public TagClass Class
        {
            get
            {
                return _tag.Class;
            }
        }

        public Boolean IsConstructed
        {
            get { return _tag.IsConstructed; }
        }


        /// <summary>
        /// Get the byte representation of the attribute and its children
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBytes()
        {
            var list = new List<Byte>();
            if (_tag.IsConstructed)
            {
                ChildAttributes.ForEach(o => list.AddRange(o.GetBytes()));
            }
            else
            {
                list.AddRange(Value);
            }

            var lengthBytes = Utils.IntToBerLength(list.Count);
            var attributeBytes = new byte[1 + lengthBytes.Length + list.Count];
            attributeBytes[0] = _tag.TagByte;
            Buffer.BlockCopy(lengthBytes, 0, attributeBytes, 1, lengthBytes.Length);
            Buffer.BlockCopy(list.ToArray(), 0, attributeBytes, 1 + lengthBytes.Length, list.Count);
            return attributeBytes;
        }


        /// <summary>
        /// Get a typed value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>()
        {
            return (T)Convert.ChangeType(GetValue(), typeof(T));
        }


        /// <summary>
        /// Get an object value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            if (_tag.Class == TagClass.Universal)
            {
                if (_tag.DataType == UniversalDataType.Boolean)
                {
                    return BitConverter.ToBoolean(Value, 0);
                }
                else if (_tag.DataType == UniversalDataType.Integer)
                {
                    var intbytes = new Byte[4];
                    Buffer.BlockCopy(Value, 0, intbytes, 4 - Value.Length, Value.Length);
                    return BitConverter.ToInt32(intbytes.Reverse().ToArray(), 0);
                }
                else
                {
                    return Encoding.UTF8.GetString(Value, 0, Value.Length);
                }
            }
            else
            {
                // todo add rest...
                return Encoding.UTF8.GetString(Value, 0, Value.Length);
            }
        }


        /// <summary>
        /// Convert the value to its byte form
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Byte[] GetBytes(Object value)
        {
            switch (value)
            {
                case String _value:
                    return Encoding.UTF8.GetBytes(_value);

                case Int32 _value:
                    return BitConverter.GetBytes(_value).Reverse().ToArray();

                case Boolean _value:
                    return BitConverter.GetBytes(_value);

                case Byte _value:
                    return new Byte[] { _value };

                case Byte[] _value:
                    return _value;

                default:
                    throw new InvalidOperationException($"Nothing found for {value.GetType()}");
            }
        }
    }
}
