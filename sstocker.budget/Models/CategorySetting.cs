using sstocker.budget.Enums;

namespace sstocker.budget.Models
{
    public class CategorySetting
    {
        public bool IsActive;
        public bool IsCritical;
        public bool Unlimited;
        public long Amount = 1;
        public Duration Duration;
    }
}
