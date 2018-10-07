using System;
using System.Collections.Generic;
using System.Linq;

namespace Flexinets.Ldap.Core
{
    /// <summary>
    /// Convenience class for creating PartialAttributes
    /// </summary>
    public class LdapPartialAttribute : LdapAttribute
    {
        /// <summary>
        /// Partial attribute description
        /// </summary>
        public String Description => (String)ChildAttributes.FirstOrDefault().GetValue();


        /// <summary>
        /// Partial attribute values
        /// </summary>
        public List<String> Values => ChildAttributes[1].ChildAttributes.Select(o => (String)o.GetValue()).ToList();


        /// <summary>
        /// Create a partial Attribute from list of values
        /// </summary>
        /// <param name="attributeDescription"></param>
        /// <param name="attributeValues"></param>
        public LdapPartialAttribute(String attributeDescription, IEnumerable<String> attributeValues) : base(UniversalDataType.Sequence)
        {
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, attributeDescription));
            var values = new LdapAttribute(UniversalDataType.Set);
            values.ChildAttributes.AddRange(attributeValues.Select(o => new LdapAttribute(UniversalDataType.OctetString, o)));
            ChildAttributes.Add(values);
        }


        /// <summary>
        /// Create a partial attribute with a single value
        /// </summary>
        /// <param name="attributeDescription"></param>
        /// <param name="attributeValue"></param>
        public LdapPartialAttribute(String attributeDescription, String attributeValue) : this(attributeDescription, new List<String> { attributeValue })
        {

        }
    }
}
