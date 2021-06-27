using System;
using Joker.Domain.Entities;
using Merchant.Domain.StoreAggregate.Rules;

namespace Merchant.Domain.StoreAggregate
{
    public class StoreBusinessHour : DomainEntity
    {
        public int DayOfWeek { get; private set; }
        public string DayName { get; private set; }
        public string StartTime { get; private set; }
        public string EndTime { get; private set; }
        public bool? IsTwentyFourHour { get; private set; }

        public StoreBusinessHour(int dayOfWeek,
            string startTime,
            string endTime,
            bool? isTwentyFourHour)
        {
            CheckRule(new DayOfWeekValidRule(dayOfWeek));

            DayName = Enum.GetName(typeof(DayOfWeek), dayOfWeek);
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            IsTwentyFourHour = isTwentyFourHour;
        }
    }
}