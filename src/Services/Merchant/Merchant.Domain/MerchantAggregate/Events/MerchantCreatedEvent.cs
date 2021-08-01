using System;
using Joker.Domain.DomainEvent;

namespace Merchant.Domain.MerchantAggregate.Events
{
    public class MerchantCreatedEvent : DomainEvent
    {
        private MerchantCreatedEvent()
        {
        }

        public MerchantCreatedEvent(Guid id, string name, Guid pricingPlanId, string pricingPlanName, Guid userId)
        {
            Id = id;
            Name = name;
            PricingPlanId = pricingPlanId;
            PricingPlanName = pricingPlanName;
            UserId = userId;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid PricingPlanId { get; private set; }
        public string PricingPlanName { get; private set; }
        public Guid UserId { get; private set; }
    }
}