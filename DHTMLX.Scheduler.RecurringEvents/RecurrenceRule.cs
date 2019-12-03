using System;
using System.Collections.Generic;

namespace DHTMLX.Scheduler.RecurringEvents
{
    /// <summary>
    /// Parsed type of recurring event
    /// </summary>
    public class RecurrenceRule
    {
        public RecurrenceType Type { get; set; }
        public int Interval { get; set; }
        public int WeekDayOfMonth { get; set; }
        public int WeekDayOfMonthInterval { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public int NumberOfInstances { get; set; }

        public static RecurrenceRule Parse(string rec_type)
        {
            if (string.IsNullOrEmpty(rec_type))
            {
                return new RecurrenceRule() { Type = RecurrenceType.RegularEvent };
            }
            var typeInfoArray = rec_type.Split('_');

            if (typeInfoArray.Length < 5 && rec_type != RecurrenceTypeFormatter.Format(RecurrenceType.DeletedInstance))
            {
                return new RecurrenceRule() { Type = RecurrenceType.RegularEvent };
            }
            var typeInfo = new RecurrenceRule()
            {
                Type = RecurrenceTypeFormatter.Parse(typeInfoArray[0]),
                Interval = -1,
                WeekDayOfMonth = -1,
                WeekDayOfMonthInterval = -1,
                DaysOfWeek = new List<int>(),
                NumberOfInstances = -1
            };
            if (typeInfoArray.Length == 5)
            {
                int intValue;

                if (!string.IsNullOrEmpty(typeInfoArray[1]) && int.TryParse(typeInfoArray[1], out intValue))
                {
                    typeInfo.Interval = intValue;
                }
                if (!string.IsNullOrEmpty(typeInfoArray[2]) && int.TryParse(typeInfoArray[2], out intValue))
                {
                    typeInfo.WeekDayOfMonthInterval = intValue;
                }
                if (!string.IsNullOrEmpty(typeInfoArray[3]) && int.TryParse(typeInfoArray[3], out intValue))
                {
                    typeInfo.WeekDayOfMonth = intValue;

                }

                if (!string.IsNullOrEmpty(typeInfoArray[4]))
                {
                    var extra = typeInfoArray[4].Split('#');
                    if (!string.IsNullOrEmpty(extra[0]))
                    {
                        var days = extra[0].Split(',');
                        for (var i = 0; i < days.Length; i++)
                        {
                            if (int.TryParse(days[i], out intValue))
                            {
                                typeInfo.DaysOfWeek.Add(intValue);
                            }
                        }
                    }

                    if (extra.Length > 1)
                    {
                        int numberOfInstances = -1;
                        if (int.TryParse(extra[1], out numberOfInstances))
                        {
                            typeInfo.NumberOfInstances = numberOfInstances;
                        }
                    }
                }
            }
            return typeInfo;
        }

        public static string Format(RecurrenceRule rec_type)
        {
            if(rec_type.Type == RecurrenceType.DeletedInstance)
            {
                return "none";
            }else if(rec_type.Type == RecurrenceType.RegularEvent)
            {
                return "";
            }
            else
            {

                return String.Format("{0}_{1}_{2}_{3}_{4}#{5}", 
                    RecurrenceTypeFormatter.Format(rec_type.Type), 
                    rec_type.Interval <= 0 ? "" : rec_type.Interval.ToString(), 
                    rec_type.WeekDayOfMonth <= 0 ? "" : rec_type.WeekDayOfMonth.ToString(),
                    rec_type.WeekDayOfMonthInterval <= 0 ? "" : rec_type.WeekDayOfMonthInterval.ToString(),
                    String.Join(",", rec_type.DaysOfWeek),
                    rec_type.NumberOfInstances > 0 ? rec_type.NumberOfInstances.ToString() : "no"
                );
            }

        }

        public override string ToString()
        {
            return RecurrenceRule.Format(this);
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                var rule = (RecurrenceRule)obj;
                return rule.ToString() == this.ToString();
            }
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
