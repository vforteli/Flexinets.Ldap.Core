using System;

namespace Flexinets.Ldap.Core
{
    public class LdapApplicationAttribute : LdapAttribute
    {
        public LdapOperation LdapOperation
        {
            get
            {
                return _tag.LdapOperation;
            }
        }


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        public LdapApplicationAttribute(LdapOperation operation, Boolean isConstructed)
        {
            _tag = new Tag(operation, isConstructed);
        }


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapApplicationAttribute(LdapOperation operation, Boolean isConstructed, Object value)
        {
            _tag = new Tag(operation, isConstructed);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create an attribute with tag
        /// </summary>
        /// <param name="tag"></param>
        protected LdapApplicationAttribute(Tag tag)
        {
            _tag = tag;
        }
    }
}
