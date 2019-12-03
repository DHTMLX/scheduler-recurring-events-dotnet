namespace DHTMLX.Scheduler.RecurringEvents
{
    public enum RecurrenceType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
        DeletedInstance,
        RegularEvent
    }

    internal static class RecurrenceTypeFormatter
    {
        public static string Format(RecurrenceType type)
        {
            switch (type)
            {
                case RecurrenceType.Daily:
                    return "day";
                case RecurrenceType.Weekly:
                    return "week";
                case RecurrenceType.Monthly:
                    return "month";
                case RecurrenceType.Yearly:
                    return "year";
                case RecurrenceType.DeletedInstance:
                    return "none";
                default:
                    return "";
            }
        }
        public static RecurrenceType Parse(string value)
        {
            switch (value)
            {
                case "day":
                    return RecurrenceType.Daily;
                case "week":
                    return RecurrenceType.Weekly;
                case "month":
                    return RecurrenceType.Monthly;
                case "year":
                    return RecurrenceType.Yearly;
                case "none":
                    return RecurrenceType.DeletedInstance;
                default:
                    return RecurrenceType.RegularEvent;
            }
        }
    }

}
