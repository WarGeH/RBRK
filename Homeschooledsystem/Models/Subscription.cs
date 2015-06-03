using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public partial class Subscription
    {
        public int SubscriptionId { get; set; }
        [Display(Name = "Подписчик")]

        public ApplicationUser Subscriber { get; set; }

        public virtual Course Course { get; set; }        
    }
}