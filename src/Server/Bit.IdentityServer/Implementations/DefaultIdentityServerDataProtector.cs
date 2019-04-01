using Microsoft.Owin.Security.DataProtection;

namespace Bit.IdentityServer.Implementations
{
    public class DefaultIdentityServerDataProtector : IdentityServer3.Core.Configuration.IDataProtector
    {
        public virtual IDataProtectionProvider DataProtectionProvider { get; set; }

        public virtual byte[] Protect(byte[] data, string entropy = "")
        {
            IDataProtector protector = DataProtectionProvider.Create(entropy);
            return protector.Protect(data);
        }

        public virtual byte[] Unprotect(byte[] data, string entropy = "")
        {
            IDataProtector protector = DataProtectionProvider.Create(entropy);
            return protector.Unprotect(data);
        }
    }
}
