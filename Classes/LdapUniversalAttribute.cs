using System;

namespace Flexinets.Ldap.Core
{
    public class LdapUniversalAttribute : LdapAttribute
    {
        public UniversalDataType DataType
        {
            get
            {
                return _tag.DataType;
            }
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        public LdapUniversalAttribute(UniversalDataType dataType, Boolean isConstructed)
        {
            _tag = new Tag(dataType, isConstructed);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapUniversalAttribute(UniversalDataType dataType, Boolean isConstructed, Object value)
        {
            _tag = new Tag(dataType, isConstructed);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create an attribute with tag
        /// </summary>
        /// <param name="tag"></param>
        protected LdapUniversalAttribute(Tag tag)
        {
            _tag = tag;
        }
    }
}
