using Microsoft.WindowsAzure.Storage.Table;

namespace HMSPhase2Functions
{
    public class Portal : TableEntity
    {
        public string PortalName { get; set; }
        public string Administrator { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UrlSuffix { get; set; }
    }
}
