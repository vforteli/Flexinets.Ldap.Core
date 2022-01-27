namespace Flexinets.Ldap.Core
{
    public class LdapResultAttribute : LdapAttribute
    {
        /// <summary>
        /// Create an Ldap result attribute
        /// </summary>        
        public LdapResultAttribute(LdapOperation operation, LdapResult result, string matchedDN = "", string diagnosticMessage = "") : base(operation)
        {
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.Enumerated, (byte)result));
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, matchedDN));
            ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, diagnosticMessage));
            // todo add referral if needed
            // todo bindresponse can contain more child attributes...
        }
    }
}
