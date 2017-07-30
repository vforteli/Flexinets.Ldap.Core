using System;

namespace Flexinets.Ldap.Core
{
    public class LdapContextAttribute : LdapAttribute
    {
        public Byte ContextType
        {
            get
            {
                return _tag.ContextType;
            }
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        public LdapContextAttribute(Byte contextType, Boolean isConstructed)
        {
            _tag = new Tag(contextType, isConstructed);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapContextAttribute(Byte contextType, Boolean isConstructed, Object value)
        {
            _tag = new Tag(contextType, isConstructed);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create an attribute with tag
        /// </summary>
        /// <param name="tag"></param>
        protected LdapContextAttribute(Tag tag)
        {
            _tag = tag;
        }
    }
}
