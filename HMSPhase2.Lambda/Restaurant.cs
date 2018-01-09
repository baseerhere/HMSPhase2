using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMSPhase2.Lambda
{
    public class Restaurant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Administrator { get; set; }
        public string EmailAddress { get; set; }
        public string UniqueId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
