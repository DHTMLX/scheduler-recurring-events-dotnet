using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHTMLX.Scheduler.RecurringEvents;

namespace DHTMLX.Scheduler.RecurringEvents.Tests
{
    [TestClass]
    public class RecurrenceRuleTest
    {

        [TestMethod]
        public void ParseMonthlySeriesFormat()
        {

            var parsedRule = RecurrenceRule.Parse("month_1_1_1_#5");

            Assert.AreEqual(RecurrenceType.Monthly, parsedRule.Type);
            Assert.AreEqual(1, parsedRule.Interval);
            Assert.AreEqual((int)DayOfWeek.Monday, parsedRule.WeekDayOfMonth);
            Assert.AreEqual(1, parsedRule.WeekDayOfMonthInterval);
            Assert.AreEqual(0, parsedRule.DaysOfWeek.Count);
            Assert.AreEqual(5, parsedRule.NumberOfInstances);
        }

        [TestMethod]
        public void ParseMonthlyNthDaySeriesFormat()
        {
            var parsedRule = RecurrenceRule.Parse("month_1_0_1_#no");
            Assert.AreEqual(RecurrenceType.Monthly, parsedRule.Type);
            Assert.AreEqual(1, parsedRule.Interval);
            Assert.AreEqual((int)DayOfWeek.Sunday, parsedRule.WeekDayOfMonth);
            Assert.AreEqual(1, parsedRule.WeekDayOfMonthInterval);
            Assert.AreEqual(0, parsedRule.DaysOfWeek.Count);
            Assert.AreEqual(-1, parsedRule.NumberOfInstances);
            Assert.AreEqual("month_1_0_1_#no", parsedRule.ToString());
        }

        [TestMethod]
        public void ParseWeeklySeriesFormat()
        {
            var parsedRule = RecurrenceRule.Parse("week_1___1,2,3,4,5#no");
            Assert.AreEqual(RecurrenceType.Weekly, parsedRule.Type);
            Assert.AreEqual(1, parsedRule.Interval);
            Assert.AreEqual(5, parsedRule.DaysOfWeek.Count);
            Assert.AreEqual(-1, parsedRule.NumberOfInstances);
        }

        [TestMethod]
        public void FormatWeeklySeries()
        {
            var rule = new RecurrenceRule
            {
                Type = RecurrenceType.Weekly,
                Interval = 1,
                DaysOfWeek = new List<int>
                {
                    (int)DayOfWeek.Monday,
                    (int)DayOfWeek.Tuesday,
                    (int)DayOfWeek.Wednesday,
                    (int)DayOfWeek.Thursday,
                    (int)DayOfWeek.Friday
                }
            };

            Assert.AreEqual("week_1___1,2,3,4,5#no", rule.ToString());
        }

    }
}
