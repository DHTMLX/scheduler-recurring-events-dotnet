using System;
using System.Collections.Generic;
using System.Text;

namespace DHTMLX.Scheduler.RecurringEvents
{
    public class SchedulerEvent
    {
        public SchedulerEvent() {}

        public SchedulerEvent(SchedulerEvent obj):this()
        {
            this.id = obj.id;
            this.text = obj.text;
            this.start_date = obj.start_date;
            this.end_date = obj.end_date;
            this.rec_type = obj.rec_type;
            this.event_length = obj.event_length;
            this.event_pid = obj.event_pid;

            if (string.IsNullOrEmpty(this.id))
            {
                this.id = "";
            }
            if (string.IsNullOrEmpty(this.text))
            {
                this.text = "";
            }
            if (string.IsNullOrEmpty(this.rec_type))
            {
                this.rec_type = "";
            }
            if (string.IsNullOrEmpty(this.event_pid))
            {
                this.event_pid = "";
            }

            if (this.event_length == default(long))
            {
                this.event_length = 0;
            }
        }

        public string id { get; set; }
        public string text { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string rec_type { get; set; }

        internal RecurrenceRule _parsed_type { 
            get 
            { 
                return RecurrenceRule.Parse(this.rec_type); 
            } 
        }
        public long event_length { get; set; }
        public string event_pid { get; set; }


        public override string ToString()
        {
            return string.Join(Environment.NewLine, new List<string>
            {
                Environment.NewLine,
                "id = " + id,
                "text = " + text,
                "start_date = " + start_date.ToString("yyyy-MM-dd HH:mm"),
                "end_date = " + end_date.ToString("yyyy-MM-dd HH:mm"),
                "rec_type = " + rec_type,
                "event_length = " + event_length.ToString(),
                "event_pid = " + event_pid,
                Environment.NewLine
            });
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
                var rule = (SchedulerEvent)obj;
                return rule.ToString() == this.ToString();
            }
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
