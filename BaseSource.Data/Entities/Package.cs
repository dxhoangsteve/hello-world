using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }

        public int MaxProjects { get; set; }
        public int MaxWallets { get; set; }
        public int MaxRequestsPerDay { get; set; }
        public int MaxRequestsPerMonth { get; set; }

        public ICollection<PackageOption> Options { get; set; }
        public ICollection<PackageSubscription> Subscriptions { get; set; }
    }

    public class PackageOption
    {
        public int Id { get; set; }
        public int DurationInMonths { get; set; }
        public decimal Price { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }
    }

    public class PackageSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserProfile User { get; set; }

        public int? PackageId { get; set; }
        public Package Package { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RefundedAmount { get; set; }
    }
}
