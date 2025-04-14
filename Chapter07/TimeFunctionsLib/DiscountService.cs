namespace TimeFunctionsLib
{
    public class DiscountService
    {
        private readonly TimeProvider _timeProvider;

        public DiscountService(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public decimal GetDiscount()
        {
            //var now = DateTime.UtcNow;

            var now = _timeProvider.GetUtcNow();

            return now.DayOfWeek switch
            {
                DayOfWeek.Saturday or DayOfWeek.Sunday => 0.2M,
                _ => 0M
            };
        }
    }
}
