using System;

namespace Flexinets.Ldap.Core
{
    public class LdapResultAttribute : LdapAttribute
    {
        /// <summary>
        /// Create a new Ldap packet with message id
        /// </summary>
        /// <param name="messageId"></param>
        public LdapResultAttribute(LdapOperation operation, LdapResult result, String matchedDN = "", String diagnosticMessage = "") : base(operation)
        {
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.Enumerated, (Byte)result));
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, matchedDN));
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, diagnosticMessage));
            // todo add referral if needed
            // todo bindresponse can contain more child attributes...
        }
    }
}
