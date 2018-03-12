using System;
using Gravity.Diagnostics;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace Gravity.Manager.Service.Ldap
{
    public class LdapAuthenticationProvider : IExternalAuthenticationProvider
    {
        private readonly ILogger _logger;
        private readonly LdapConnectionOptions _ldapConnectionOptions;

        public LdapAuthenticationProvider(ILogger logger, IOptions<LdapConnectionOptions> ldapConnectionOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ldapConnectionOptions = ldapConnectionOptions?.Value ?? throw new ArgumentNullException(nameof(ldapConnectionOptions.Value));
        }

        public bool Authenticate(string logonName, string logonPassword)
        {
            if (string.IsNullOrWhiteSpace(logonName)) throw new ArgumentNullException(nameof(logonName));
            if (string.IsNullOrWhiteSpace(logonPassword)) throw new ArgumentNullException(nameof(logonPassword));
            using (var ldapConnection = new LdapConnection())
            {
                try
                {
                    ldapConnection.SecureSocketLayer = _ldapConnectionOptions.UseSecureConnection;
                    ldapConnection.Connect(_ldapConnectionOptions.HostAddress, _ldapConnectionOptions.Port);
                    return ldapConnection.Bound;
                }
                catch (Exception ex)
                {
                    _logger.Error("LDAP Authentication Failure", ex);
                    return false;
                }
                finally
                {
                    ldapConnection.Disconnect();
                }
            }
        }
        
        
        
        
    }
}