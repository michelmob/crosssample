namespace Gravity.Manager.Service.Ldap
{
    public class LdapConnectionOptions
    {
        public string HostAddress { get; set; }

        public int Port { get; set; }

        public bool UseSecureConnection { get; set; }
    }
}