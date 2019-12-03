using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHTMLX.Scheduler.RecurringEvents;

namespace DHTMLX.Scheduler.RecurringEvents.Tests
{
    [TestClass]
    public class SingleEventsTest
    {

        private SchedulerEvent _getTestEvent()
        {
            return new SchedulerEvent
            {
                id = "1",
                text = "1",
                start_date = new DateTime(2019, 12, 1, 14, 30, 0),
                end_date = new DateTime(2019, 12, 1, 15, 30, 0)
            };
        }

        private List<SchedulerEvent> _getTestEventArray()
        {
            return new List<SchedulerEvent>{
                new SchedulerEvent
                {
                    id = "1",
                    text = "1",
                    start_date = new DateTime(2019, 10, 1, 14, 30, 0),
                    end_date = new DateTime(2019, 10, 1, 15, 30, 0)
                },
                new SchedulerEvent
                {
                    id = "2",
                    text = "2",
                    start_date = new DateTime(2019, 11, 1, 14, 30, 0),
                    end_date = new DateTime(2019, 12, 1, 15, 30, 0)
                },
                new SchedulerEvent
                {
                    id = "3",
                    text = "3",
                    start_date = new DateTime(2019, 12, 1, 14, 30, 0),
                    end_date = new DateTime(2019, 12, 1, 15, 30, 0)
                },

            };
        }

        [TestMethod]
        public void ParseSingleEvent()
        {

            var helper = new RecurringEventsHelper();

            var parseSingleEvent = helper.GetOccurrences(_getTestEvent(), new DateTime(2019, 1, 1), new DateTime(2020, 1, 1));

            Assert.AreEqual(parseSingleEvent.Count, 1);
            Assert.AreEqual(_getTestEvent(), parseSingleEvent.First());
        }

        [TestMethod]
        public void ParseSingleEventArray()
        {

            var helper = new RecurringEventsHelper();
            var parseSingleEvent = helper.GetOccurrences(_getTestEventArray(), new DateTime(2019, 1, 1), new DateTime(2020, 1, 1));

            var expected = _getTestEventArray();
            Assert.AreEqual(parseSingleEvent.Count, expected.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], parseSingleEvent[i]);
            }
        }

        [TestMethod]
        public void ParseOutOfRangeEvent()
        {

            var helper = new RecurringEventsHelper();
            var parseSingleEvent = helper.GetOccurrences(new List<SchedulerEvent> { this._getTestEvent() }, new DateTime(2019, 10, 1), new DateTime(2019, 11, 1));

            Assert.AreEqual(parseSingleEvent.Count, 0);
        }

        [TestMethod]
        public void ParseOutOfRangePartial()
        {

            var helper = new RecurringEventsHelper();
            var parseSingleEvent = helper.GetOccurrences(_getTestEventArray(), new DateTime(2019, 10, 1), new DateTime(2019, 11, 1));
            var expected = _getTestEventArray();
            Assert.AreEqual(parseSingleEvent.Count, 1);
            Assert.AreEqual(expected.First(), parseSingleEvent.First());
        }
    }
}
