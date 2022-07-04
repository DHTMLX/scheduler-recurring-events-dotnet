using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DHTMLX.Scheduler.RecurringEvents
{
    public class RecurringEventsHelper
    {
        protected class SeriesInfo
        {
            public SchedulerEvent SchedulerEvent { get; set; }
            public List<DateTime> Occurrences { get; set; }
        }

        public bool OccurrenceTimestampInUtc = false;
        public OverflowInstancesRule OverflowInstances = OverflowInstancesRule.Skip;

        /// <summary>
        /// Parses raw scheduler event records into individual occurrences within specified date range
        /// </summary>
        /// <param name="items">Collection of calendar events as they retreived from the data source</param>
        /// <param name="from">Minimal occurrence date</param>
        /// <param name="to">Maximal occurrence date</param>
        /// <param name="maxOccurrences">Max number of occurrences per series</param>
        /// <returns></returns>
        public List<SchedulerEvent> GetOccurrences(IEnumerable<SchedulerEvent> items, DateTime from, DateTime to, int maxOccurrences = 300)
        {

            var output = new List<SchedulerEvent>();
    
            var parsedSeries = new List<SeriesInfo>();
   
            foreach (var item in items)
            {
                // split recurring series into individual occurrences
                // single events are interpreted as series with a single occurrence
                var occurrences = _GetOccurrences(item, from, to, maxOccurrences);
                if (occurrences != null)
                    parsedSeries.Add(occurrences);
            }

            var timezoneOffset = OccurrenceTimestampInUtc ? (new TimeSpan(0)) : TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

            // apply modified or deleted occurrences to the series
            foreach (var eventSet in parsedSeries)
            {
                if (eventSet.SchedulerEvent._parsed_type.Type == RecurrenceType.RegularEvent || eventSet.SchedulerEvent._parsed_type.Type == RecurrenceType.DeletedInstance)
                {
                    var parentSeries = parsedSeries.SingleOrDefault(set => set.SchedulerEvent.id == eventSet.SchedulerEvent.event_pid);
                    if (parentSeries != null)
                    {
                        var occurrenceTimestamp = new DateTime(1970, 1, 1).AddSeconds((long)eventSet.SchedulerEvent.event_length) + timezoneOffset;
                        parentSeries.Occurrences.Remove(occurrenceTimestamp);
                    }
                }
            }

            // remove deleted instances from the result dataset
            parsedSeries = parsedSeries.Where(e => e.SchedulerEvent._parsed_type.Type != RecurrenceType.DeletedInstance).ToList();

            foreach (var eventSet in parsedSeries)
            {
                output.AddRange(_ToEventList(eventSet));
                
              //  if (((eventSet.SchedulerEvent._parsed_type == null && (string.IsNullOrEmpty(eventSet.SchedulerEvent.rec_type))) || (eventSet.SchedulerEvent._parsed_type != null && (eventSet.SchedulerEvent._parsed_type.Type == RecurrenceType.NoRepeat || eventSet.SchedulerEvent._parsed_type.Type != RecurrenceType.DeletedInstance))))
              //  {
              //      output.AddRange(_ProcessEvData(eventSet));
              //  }
            }
            return output;
        }

        public List<SchedulerEvent> GetOccurrences(SchedulerEvent item, DateTime from, DateTime to, int maxOccurrences = 300)
        {
            return this.GetOccurrences(new List<SchedulerEvent> { item }, from, to, maxOccurrences);
        }

        ///<summary>Gets the DateTime of the next specified week day following a specified date (i.e. get next Monday after December 1st, 2019).</summary>
        ///<param name="date">The date.</param>
        ///<param name="dayOfWeek">The day of week to return.</param>
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
        protected DateTime _Next(DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }

        /// <summary>
        /// Returns the DateTime of the specified week day that occurs on a specified week after a provided date (i.e. get second Tuesday of starting from December 1st, 2019)
        /// </summary>
        /// <param name="date">Start date</param>
        /// <param name="nthWeek">Week number</param>
        /// <param name="dayOfWeek">Day of week to return</param>
        /// <returns></returns>
        private DateTime _GetNthWeekOfMonth(DateTime date, int nthWeek, DayOfWeek dayOfWeek)
        {
            return _Next(date, dayOfWeek).AddDays((nthWeek - 1) * 7);
        }
        private List<SchedulerEvent> _ToEventList(SeriesInfo data)
        {
            var results = new List<SchedulerEvent>();

            if(data.SchedulerEvent._parsed_type.Type == RecurrenceType.RegularEvent)
            {
                results.Add(data.SchedulerEvent);
            }
            else
            {
                var secondsLength = data.SchedulerEvent.event_length;

                foreach (DateTime currentDate in data.Occurrences)
                {
                    results.Add(new SchedulerEvent
                    {
                        id = data.SchedulerEvent.id,
                        text = data.SchedulerEvent.text,
                        start_date = currentDate,
                        end_date = currentDate.AddSeconds(secondsLength)
                    });
                }
            }
            return results;

        }

        protected bool _IsInTimeFrame(DateTime evStart, DateTime evEnd, DateTime From, DateTime To)
        {
            return (evStart < To && evEnd > From && evStart < evEnd);
        }

        protected int _getSeriesStep(RecurrenceType type)
        {
            switch (type)
            {
                case RecurrenceType.Daily:
                    return 1;
                case RecurrenceType.Weekly:
                    return 7;
                case RecurrenceType.Monthly:
                    return 1;
                case RecurrenceType.Yearly:
                    return 12;
            }
            return 1;
        }

        protected DateTime _CorrectSeriesStartDate(DateTime eventStart, DateTime rangeStart, SchedulerEvent ev)
        {
            var resultDate = eventStart;
            var rule = ev._parsed_type;
            switch (rule.Type) {
                case RecurrenceType.Monthly:
                case RecurrenceType.Yearly:
                    var step = _getSeriesStep(rule.Type);
                    var delta = (int)Math.Ceiling((decimal)((rangeStart.Year * 12 + rangeStart.Month) - (eventStart.Year * 12 + eventStart.Month)) / (step));
                    if(delta > 0)
                    {
                        resultDate = new DateTime(resultDate.Year, resultDate.Month, 1, resultDate.Hour, resultDate.Minute, resultDate.Second);
                        resultDate = resultDate.AddMonths(delta * rule.Interval);
                    }
                    return _GoToFirstMonthYearInstance(resultDate, 0, ev);
            }
            return resultDate;
        }


        public enum OverflowInstancesRule
        {
            Default,
            LastDay,
            Skip
        }
        protected DateTime _GoToFirstMonthYearInstance(DateTime startDate, int increment, SchedulerEvent seriesInstance, int currentCount = 0)
        {
            var overflowRule = OverflowInstancesRule.LastDay;
            if(currentCount == 0)
            {
                currentCount = 1;
            }
            else
            {
                currentCount++;
            }

            var typeInfo = seriesInstance._parsed_type;

            var maxCount = 12;

            if(currentCount > maxCount)
            {
                return default(DateTime);
            }

            var resultDate = new DateTime(
                startDate.Year, 
                startDate.Month, 
                1, 
                startDate.Hour, 
                startDate.Minute,
                startDate.Second
            );
            resultDate = resultDate.AddMonths(increment * _getSeriesStep(typeInfo.Type));


            var originalDate = resultDate;

            if (typeInfo.WeekDayOfMonthInterval != -1 && typeInfo.WeekDayOfMonth != -1)
            {
                resultDate = _GetNthWeekOfMonth(resultDate, typeInfo.WeekDayOfMonthInterval, (DayOfWeek)typeInfo.WeekDayOfMonth);

                if(resultDate.Month != originalDate.Month && overflowRule != OverflowInstancesRule.Default)
                {
                    if(overflowRule == OverflowInstancesRule.LastDay)
                    {
                        resultDate = originalDate.AddMonths(1);
                        resultDate = resultDate.AddDays(-1);
                    }
                    else
                    {
                        resultDate = _GoToFirstMonthYearInstance(originalDate.AddMonths(1), increment, seriesInstance, currentCount);
                    }
                }
            }

            return resultDate;

        }

        protected SeriesInfo _GetOccurrences(SchedulerEvent ev, DateTime fromDate, DateTime toDate, int maxOccurrences)
        {
            var tmp = new SchedulerEvent(ev);

            var typeInfo = tmp._parsed_type;
            if (typeInfo.Type != RecurrenceType.RegularEvent && typeInfo.Type != RecurrenceType.DeletedInstance && tmp.event_length > 0)
            {

                var occurrences = new List<DateTime>();

                var seriesStart = tmp.start_date;

                var currDate = _CorrectSeriesStartDate(tmp.start_date, fromDate, ev);

                var end_date = tmp.end_date;
                if(typeInfo.NumberOfInstances > -1 && typeInfo.NumberOfInstances < maxOccurrences)
                {
                    maxOccurrences = typeInfo.NumberOfInstances;
                }
                while (currDate < end_date && currDate < toDate && occurrences.Count < maxOccurrences)
                {
                    var occEndDate = currDate.AddSeconds((double)tmp.event_length);
                    if (typeInfo.WeekDayOfMonthInterval != -1 && typeInfo.WeekDayOfMonth != -1)
                    {

                        var tmp_date = _GetNthWeekOfMonth(currDate, typeInfo.WeekDayOfMonthInterval, (DayOfWeek)typeInfo.WeekDayOfMonth);
                        occEndDate = tmp_date.AddSeconds((double)tmp.event_length);
                        if (_IsInTimeFrame(tmp_date, occEndDate, fromDate, toDate) && tmp_date >= seriesStart)
                        {
                            occurrences.Add(tmp_date);
                        }
                    }
                    else if (typeInfo.DaysOfWeek.Count == 0 && _IsInTimeFrame(currDate, occEndDate, fromDate, toDate) && currDate >= seriesStart)
                    {
                        occurrences.Add(currDate);
                    }
                    else if (typeInfo.DaysOfWeek.Count > 0)
                    {
                        foreach (var occurrenceWeekDay in typeInfo.DaysOfWeek)
                        {

                            int currentWeekDay = (int)currDate.DayOfWeek;
                            var diff = (7 - currentWeekDay + occurrenceWeekDay) % 7;
                            var occurrenceStartDate = currDate.AddDays(diff);

                            occEndDate = occurrenceStartDate.AddSeconds((double)tmp.event_length);
                            if (_IsInTimeFrame(occurrenceStartDate, occEndDate, fromDate, toDate) && occurrences.Count < maxOccurrences && occurrenceStartDate >= seriesStart)
                                occurrences.Add(occurrenceStartDate);
                        }
                    }
                    switch (typeInfo.Type)
                    {
                        case RecurrenceType.Daily:
                            currDate = currDate.AddDays(typeInfo.Interval);
                            break;
                        case RecurrenceType.Weekly:
                            currDate = currDate.AddDays(7 * typeInfo.Interval);
                            break;
                        case RecurrenceType.Monthly:
                            currDate = _GoToFirstMonthYearInstance(currDate, typeInfo.Interval, ev);
                            //currDate = currDate.AddMonths(typeInfo.Interval);
                            break;
                        case RecurrenceType.Yearly:
                            currDate = _GoToFirstMonthYearInstance(currDate, typeInfo.Interval, ev);
                            //currDate = currDate.AddYears(typeInfo.Interval);
                            break;
                        default:
                            break;
                    }
                }

                return new SeriesInfo { SchedulerEvent = tmp, Occurrences = occurrences };

            }
            else
            {
                if (tmp.start_date < toDate
                    && tmp.end_date > fromDate)
                    return new SeriesInfo { SchedulerEvent = tmp, Occurrences = new List<DateTime>() { tmp.start_date } };
                else
                    return null;
            }

        }

    }
}
