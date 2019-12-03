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

        protected SeriesInfo _GetOccurrences(SchedulerEvent ev, DateTime fromDate, DateTime toDate, int maxOccurrences)
        {
            var tmp = new SchedulerEvent(ev);

            var typeInfo = tmp._parsed_type;
            if (typeInfo.Type != RecurrenceType.RegularEvent && typeInfo.Type != RecurrenceType.DeletedInstance && tmp.event_length > 0)
            {

                var occurrences = new List<DateTime>();

                var currDate = tmp.start_date;

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
                        if (_IsInTimeFrame(tmp_date, occEndDate, fromDate, toDate))
                        {
                            occurrences.Add(tmp_date);
                        }
                    }
                    else if (typeInfo.DaysOfWeek.Count == 0 && _IsInTimeFrame(currDate, occEndDate, fromDate, toDate))
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
                            if (_IsInTimeFrame(occurrenceStartDate, occEndDate, fromDate, toDate) && occurrences.Count < maxOccurrences)
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
                            currDate = currDate.AddMonths(typeInfo.Interval);
                            break;
                        case RecurrenceType.Yearly:
                            currDate = currDate.AddYears(typeInfo.Interval);
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
