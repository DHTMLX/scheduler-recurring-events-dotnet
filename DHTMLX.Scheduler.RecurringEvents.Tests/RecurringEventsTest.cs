using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHTMLX.Scheduler.RecurringEvents;

namespace DHTMLX.Scheduler.RecurringEvents.Tests
{
    [TestClass]
    public class RecurringEventsTest
    {

        public List<SchedulerEvent> weeklyEveryThursday()
        {
            return new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every monday"
                    ,start_date=new DateTime(2011, 10, 03, 01,05,00)
                    ,end_date=new DateTime(9999, 01, 02)
                    ,event_length=8400
                    ,rec_type="week_1___4#no"
                    ,event_pid=null}
            };
        }

        public List<SchedulerEvent> getData()
        {
            return new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every monday"
                    ,start_date=new DateTime(2011, 09, 01, 10,30,00)
                    ,end_date=new DateTime(2012, 06, 02, 10,30,00)
                    ,event_length=6900
                    ,rec_type="month_1_1_1_#5"
                    ,event_pid=null}
                ,new SchedulerEvent(){
                    id="26"
                    ,text="every monday"
                    ,start_date=new DateTime(2011, 09, 01, 5,35,00)
                    ,end_date=new DateTime(9999, 01, 02)
                    ,event_length=12000
                    ,rec_type="month_1_1_1_#no"
                    ,event_pid=null}
                ,new SchedulerEvent(){
                    id="27"
                    ,text="New event"
                    ,start_date=new DateTime(2011, 10, 01, 13,20,00)
                    ,end_date=new DateTime(2012, 8, 3, 0,0,00)
                    ,event_length=7500
                    ,rec_type="month_1_1_1_#"
                    ,event_pid=null}
                 ,new SchedulerEvent(){
                    id="28"
                    ,text="every sunday"
                    ,start_date=new DateTime(2011, 10, 01, 7,50,00)
                    ,end_date=new DateTime(9999, 02, 01, 0,0,00)
                    ,event_length=3000
                    ,rec_type="month_1_0_1_#no"
                    ,event_pid=null}
                 ,new SchedulerEvent(){
                    id="29"
                    ,text="mon wedn sat"
                    ,start_date=new DateTime(2011, 09, 26, 2,05,00)
                    ,end_date=new DateTime(9999, 02, 01, 0,0,00)
                    ,event_length=12900
                    ,rec_type="week_2___1,3,6#no"
                    ,event_pid=null
                 }
                 ,new SchedulerEvent(){
                    id="30"
                    ,text="every 3 day"
                    ,start_date=new DateTime(2011, 09, 27, 7,40,00)
                    ,end_date=new DateTime(9999, 02, 01, 0,0,00)
                    ,event_length=300
                    ,rec_type="day_3___#no"
                    ,event_pid=null}
                 ,new SchedulerEvent(){
                    id="31"
                    ,text="single event"
                    ,start_date=new DateTime(2011, 09, 28, 7,10,00)
                    ,end_date=new DateTime(2011, 09, 28, 8,40,00)
                    ,event_length=0
                    ,rec_type=""
                    ,event_pid=""}
                };
        }




        [TestMethod]
        public void Weekly()
        {
            var data = new List<SchedulerEvent>
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every monday"
                    ,start_date=new DateTime(2011, 10, 03, 01,05,00)
                    ,end_date=new DateTime(9999, 01, 02)
                    ,event_length=8400
                    ,rec_type="week_1___4#no"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 9, 26), new DateTime(2011, 10, 10, 23, 59, 59));
            Assert.AreEqual(1, items.Count);

        }

        [TestMethod]
        public void WeeklyLimit()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every monday"
                    ,start_date=new DateTime(2011, 10, 03, 03,50,00)
                    ,end_date=new DateTime(9999, 10, 16)
                    ,event_length=8400
                    ,rec_type="month_1_1_1_#5"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 9, 26), new DateTime(2012, 10, 10, 23, 59, 59));
            Assert.AreEqual(5, items.Count);

        }

        [TestMethod]
        public void DailyEveryTwoDays()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every 2 days"
                    ,start_date=new DateTime(2011, 10, 03, 00,45,00)
                    ,end_date=new DateTime(9999, 01, 02)
                    ,event_length=7800
                    ,rec_type="day_2___#no"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 9, 26), new DateTime(2011, 10, 10, 23, 59, 59));
            Assert.AreEqual(4, items.Count);

        }




        [TestMethod]
        public void DeletedEvent()
        {

            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="35"
                    ,text="!DELETED DAY"
                    ,start_date=new DateTime(2011, 09, 26,0, 20,0)
                    ,end_date=new DateTime(9999, 2, 1)
                    ,event_length=5100
                    ,rec_type="week_1___0,1,2,3#no"
                    ,event_pid=null}

                ,new SchedulerEvent(){id="36"
                     ,text="!DELETED DAY"
                     ,start_date=new DateTime(2011, 09, 28,0, 20,0)
                     ,end_date=new DateTime(2011, 09, 28,1, 45,0)
                     ,event_length=1317158400
                     ,rec_type="none"
                     ,event_pid="35"}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 9, 26), new DateTime(2011, 10, 10, 23, 59, 59));
            Assert.AreEqual(8, items.Count);

        }

        [TestMethod]
        public void FirstMonday()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every monday"
                    ,start_date=new DateTime(2011, 09, 01, 10,30,00)
                    ,end_date=new DateTime(2012, 06, 02, 10,30,00)
                    ,event_length=6900
                    ,rec_type="month_1_1_1_#5"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 9, 1), new DateTime(2011, 10, 1, 23, 59, 59));
            Assert.AreEqual(1, items.Count);

        }

        [TestMethod]
        public void EveryTwoMonths()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="38"
                    ,text="Second tuesday"
                    ,start_date=new DateTime(2011, 10, 01, 11,50,00)
                    ,end_date=new DateTime(9999, 02, 01)
                    ,event_length=300
                    ,rec_type="month_2_2_2_#no"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 10, 1), new DateTime(2011, 11, 30, 23, 59, 59));
            Assert.AreEqual(1, items.Count);

        }

        [TestMethod]
        public void EveryTwoMonthsExactDay()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="38"
                    ,text="Second tuesday"
                    ,start_date=new DateTime(2011, 10, 01, 11,50,00)
                    ,end_date=new DateTime(9999, 02, 01)
                    ,event_length=300
                    ,rec_type="month_2_2_2_#no"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 10, 11), new DateTime(2011, 11, 12, 23, 59, 59));
            Assert.AreEqual(1, items.Count);

        }

        [TestMethod]
        public void Yearly()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="38"
                    ,text="yearly event"
                    ,start_date=new DateTime(2011, 01, 01, 14,45,00)
                    ,end_date=new DateTime(9999, 02, 01)
                    ,event_length=13500
                    ,rec_type="year_1_5_2_#no"
                    ,event_pid=null}
            };

            var helper = new RecurringEventsHelper();
            var items = helper.GetOccurrences(data, new DateTime(2011, 01, 1), new DateTime(2013, 1, 1, 23, 59, 59));
            Assert.AreEqual(2, items.Count);

        }


        [TestMethod]
        public void MonthlyStartDate()
        {
            var data = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="25"
                    ,text="Every monday"
                    ,start_date=new DateTime(2011, 09, 01, 10,30,00)
                    ,end_date=new DateTime(2012, 06, 02, 10,30,00)
                    ,event_length=6900
                    ,rec_type="month_1_1_1_#5"
                    ,event_pid=null}
            };
            var helper = new RecurringEventsHelper();

            var items = helper.GetOccurrences(data, new DateTime(2011, 09, 26), new DateTime(2011, 10, 28, 23, 59, 59));
            Assert.AreEqual(1, items.Count);
        }


        [TestMethod]
        public void AllTogether()
        {
            var helper = new RecurringEventsHelper();

            var items = helper.GetOccurrences(getData(), new DateTime(2011, 09, 26), new DateTime(2011, 10, 28, 23, 59, 59));
            Assert.AreEqual(24, items.Count);

        }
        [TestMethod]
        public void EveryWorkdaySelect()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                        id="26"
                        ,text = "New event"
                        ,start_date = new DateTime(2011, 10, 01, 12, 0, 0)
                        ,end_date = new DateTime(9999, 02, 01)
                        ,event_length = 5700
                        ,rec_type = "week_1___1,2,3,4,5#no"
                        ,event_pid = null
                }
            };
            var helper = new RecurringEventsHelper();

            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 01), new DateTime(2011, 10, 28));

            Assert.AreNotEqual(0, items.Count);
        }
        [TestMethod]
        public void EveryWorkday()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="26"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 10, 01, 12, 0, 0)
                    ,end_date = new DateTime(9999, 02, 01)
                    ,event_length = 5700
                    ,rec_type = "week_1___1,2,3,4,5#no"
                    ,event_pid = null
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 01), new DateTime(2011, 10, 28));

            Assert.AreEqual(
                0,
                items.Where(i => i.start_date.DayOfWeek == DayOfWeek.Saturday || i.start_date.DayOfWeek == DayOfWeek.Sunday).Count()
                );
        }

        [TestMethod]
        public void EveryWorkdayExactTimeUnspecified()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="26"
                    ,text = "New event"
                    ,start_date = DateTime.SpecifyKind(new DateTime(2011, 10, 01, 12, 0, 0), DateTimeKind.Unspecified)
                    ,end_date = new DateTime(9999, 02, 01)
                    ,event_length = 5700
                    ,rec_type = "week_1___1,2,3,4,5#no"
                    ,event_pid = null
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 01), new DateTime(2011, 10, 28));

            Assert.AreEqual(
                items.Count,
                items.Where(i => i.start_date.Hour == 12 && i.end_date.Hour == 13).Count()
                );
        }

        [TestMethod]
        public void EveryWorkdayExactTimeUTC()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="26"
                    ,text = "New event"
                    ,start_date = DateTime.SpecifyKind(new DateTime(2011, 10, 01, 12, 0, 0), DateTimeKind.Utc)
                    ,end_date = new DateTime(9999, 02, 01)
                    ,event_length = 5700
                    ,rec_type = "week_1___1,2,3,4,5#no"
                    ,event_pid = null
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 01), new DateTime(2011, 10, 28));

            Assert.AreEqual(
                items.Count,
                items.Where(i =>
                i.start_date.Hour == 12
                && i.end_date.Hour == 13).Count()
                );
        }
        [TestMethod]
        public void EveryWorkdayExactTimeLocal()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="26"
                    ,text = "New event"
                    ,start_date = DateTime.SpecifyKind(new DateTime(2011, 10, 01, 12, 0, 0), DateTimeKind.Local)
                    ,end_date = new DateTime(9999, 02, 01)
                    ,event_length = 5700
                    ,rec_type = "week_1___1,2,3,4,5#no"
                    ,event_pid = null
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 01), new DateTime(2011, 10, 28));

            Assert.AreEqual(
                items.Count,
                items.Where(i =>
                i.start_date.Hour == 12
                && i.end_date.Hour == 13).Count()
                );
        }
        [TestMethod]
        public void DeletedEventPos()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="27"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 27, 3, 05, 0)
                    ,end_date = new DateTime(2011, 09, 30, 3, 05, 0)
                    ,event_length = 10800
                    ,rec_type = "day_1___#3"
                    ,event_pid = null
                }
                ,new SchedulerEvent(){
                    id="28"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 28, 3, 05, 0)
                    ,end_date = new DateTime(2011, 09, 28, 6, 05, 0)
                    ,event_length = 1317168300
                    ,rec_type = "none"
                    ,event_pid = "27"
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 28), new DateTime(2011, 9, 29));

            Assert.AreEqual(
                0,
                items.Count);
        }
        [TestMethod]
        public void DeletedEventGetCount()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="27"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 27, 3, 05, 0)
                    ,end_date = new DateTime(2011, 09, 30, 3, 05, 0)
                    ,event_length = 10800
                    ,rec_type = "day_1___#3"
                    ,event_pid = null
                }
                ,new SchedulerEvent(){
                    id="28"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 28, 3, 05, 0)
                    ,end_date = new DateTime(2011, 09, 28, 6, 05, 0)
                    ,event_length = 1317168300
                    ,rec_type = "none"
                    ,event_pid = "27"
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 1), new DateTime(2011, 10, 1));

            Assert.AreEqual(
                2,
                items.Count);
        }
        [TestMethod]
        public void EditedEventGetCount()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="30"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 27, 2, 00, 0)
                    ,end_date = new DateTime(2011, 09, 30, 2, 00, 0)
                    ,event_length = 12300
                    ,rec_type = "day_1___#3"
                    ,event_pid = null
                }
                ,new SchedulerEvent(){
                    id="31"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 28, 6, 15, 0)
                    ,end_date = new DateTime(2011, 09, 28, 9, 40, 0)
                    ,event_length = 1317164400
                    ,rec_type = ""
                    ,event_pid = "30"
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 1), new DateTime(2011, 10, 1));

            Assert.AreEqual(
                2,
                items.Where(i => i.start_date.Hour == 2).Count());
        }

        [TestMethod]
        public void EditedEventGetModified()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id="30"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 27, 2, 00, 0)
                    ,end_date = new DateTime(2011, 09, 30, 2, 00, 0)
                    ,event_length = 12300
                    ,rec_type = "day_1___#3"
                    ,event_pid = null
                }
                ,new SchedulerEvent(){
                    id="31"
                    ,text = "New event"
                    ,start_date = new DateTime(2011, 09, 28, 6, 15, 0)
                    ,end_date = new DateTime(2011, 09, 28, 9, 40, 0)
                    ,event_length = 1317164400
                    ,rec_type = ""
                    ,event_pid = "30"
                }
            };
            var helper = new RecurringEventsHelper();


            var items = helper.GetOccurrences(evs, new DateTime(2011, 09, 1), new DateTime(2011, 10, 1));

            Assert.AreEqual(
                1,
                items.Where(i => i.start_date.Hour == 6).Count());
        }

        [TestMethod]
        public void EventsAtTheMoment()
        {
            var evs = new List<SchedulerEvent>()
            {
                new SchedulerEvent(){
                    id = "1",
                    text = "Sample Event",
                    start_date = new DateTime(2013, 05, 07, 08,00,00),
                    end_date = new DateTime(2013, 07, 14, 00,00,00),
                    event_pid = null,
                    event_length = 43200,
                    rec_type = "day_1___#"
                }
                ,new SchedulerEvent(){
                    id="2",
                    text = "Sample Event",
                    start_date = new DateTime(2013, 06, 07, 08,00,00),
                    end_date = new DateTime(2013, 07, 14, 00,00,00),
                    event_pid = null,
                    event_length = 43200,
                    rec_type = "day_1___#"
                }
                ,new SchedulerEvent(){
                    id="3",
                    text = "Sample Event",
                    start_date = new DateTime(2013, 05, 07, 2,00,00),
                    end_date = new DateTime(2013, 05, 07, 12,00,00),
                    event_pid = null,
                    event_length = 0,
                    rec_type = ""
                }
                ,new SchedulerEvent(){
                    id="4",
                    text = "Sample Event",
                    start_date = new DateTime(2013, 05, 07, 10,00,00),
                    end_date = new DateTime(2013, 05, 07, 15,00,00),
                    event_pid = null,
                    event_length = 0,
                    rec_type = ""
                }
            };
            var helper = new RecurringEventsHelper();

            var dat = new DateTime(2013, 05, 07, 09, 00, 00);
            var items = helper.GetOccurrences(evs, dat, dat.AddSeconds(10));

            Assert.AreEqual(
                2,
                items.Count());
        }

    }
}
