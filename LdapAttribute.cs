﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flexinets.Ldap.Core
{
    public class LdapAttribute
    {
        private Tag _tag;
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

        public LdapOperation? LdapOperation
        {
            get
            {
                if (_tag.Class == TagClass.Application)
                {
                    return _tag.LdapOperation;
                }
                return null;
            }
        }

        public UniversalDataType? DataType
        {
            get
            {
                if (_tag.Class == TagClass.Universal)
                {
                    return _tag.DataType;
                }
                return null;
            }
        }

        public Byte? ContextType
        {
            get
            {
                if (_tag.Class == TagClass.Context)
                {
                    return _tag.ContextType;
                }
                return null;
            }
        }


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(LdapOperation operation, Boolean isConstructed)
        {
            _tag = new Tag(operation, isConstructed);
        }


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(LdapOperation operation, Boolean isConstructed, Object value)
        {
            _tag = new Tag(operation, isConstructed);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(UniversalDataType dataType, Boolean isConstructed)
        {
            _tag = new Tag(dataType, isConstructed);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(UniversalDataType dataType, Boolean isConstructed, Object value)
        {
            _tag = new Tag(dataType, isConstructed);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(Byte contextType, Boolean isConstructed)
        {
            _tag = new Tag(contextType, isConstructed);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(Byte contextType, Boolean isConstructed, Object value)
        {
            _tag = new Tag(contextType, isConstructed);
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
        private Byte[] GetBytes(Object value)
        {
            if (value.GetType() == typeof(String))
            {
                return Encoding.UTF8.GetBytes((String)value);
            }
            else if (value.GetType() == typeof(Int32))
            {
                return BitConverter.GetBytes((Int32)value).Reverse().ToArray();
            }
            else if (value.GetType() == typeof(Boolean))
            {
                return BitConverter.GetBytes((Boolean)value);
            }
            else if (value.GetType() == typeof(Byte))
            {
                return new Byte[] { (Byte)value };
            }
            else if (value.GetType() == typeof(Byte[]))
            {
                return (Byte[])value;
            }
            throw new InvalidOperationException($"Nothing found for {value.GetType()}");
        }
    }
}