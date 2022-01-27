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
        public string Description => (string)ChildAttributes.FirstOrDefault().GetValue();


        /// <summary>
        /// Partial attribute values
        /// </summary>
        public List<string> Values => ChildAttributes[1].ChildAttributes.Select(o => (string)o.GetValue()).ToList();


        /// <summary>
        /// Create a partial Attribute from list of values
        /// </summary>
        /// <param name="attributeDescription"></param>
        /// <param name="attributeValues"></param>
        public LdapPartialAttribute(string attributeDescription, IEnumerable<string> attributeValues) : base(UniversalDataType.Sequence)
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
        public LdapPartialAttribute(string attributeDescription, string attributeValue) : this(attributeDescription, new List<string> { attributeValue })
        {
        }
    }
}
