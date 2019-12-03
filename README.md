# Recurring Events Helper for dhtmlxScheduler on ASP.NET/ASP.NET Core backends

A helper library for parsing recurring events of [dhtmlxScheduler](https://docs.dhtmlx.com/scheduler/) on ASP.NET/ASP.NET Core backends.

## Supported Frameworks

- .NET 4.5+
- .NET Core 1.0+

## Install

### Package Manager Console

```
PM> Install-Package DHTMLX.Scheduler.RecurringEvents
```

### .NET CLI Console

```
> dotnet add package DHTMLX.Scheduler.RecurringEvents
```

## Usage

### Overview

The main class is **RecurringEventsHelper** which is defined in **DHTMLX.Scheduler.RecurringEvents** namespace.

The only public method **GetOccurrences** takes a list of event entries in the same form they are stored in the database, and outputs the list of processed records:

- entries that represent recurring series will be replaced with individual occurrences of the series. Each occurrence will have its own start and end dates
- single events won't be modified
- records that represent deleted occurrences will be removed from the result


### Basic usage

Add the namespace:

```
using DHTMLX.Scheduler.RecurringEvents;
```

And use:

```

// SchedulerEvent is defined in DHTMLX.Scheduler.RecurringEvents namespace
var data = new List<SchedulerEvent>
{
    new SchedulerEvent(){
        id = "1",
        text = "Every Thursday",
        start_date = new DateTime(2019, 10, 03, 01, 05, 00),
        end_date = new DateTime(9999, 01, 02),
        event_length = 8400,
        rec_type = "week_1___4#no",
        event_pid = null
	}
};

var helper = new RecurringEventsHelper();
var items = helper.GetOccurrences(data, new DateTime(2019, 11, 1), new DateTime(2019, 11, 15));
// items:
// [ 
//   { id = 1, text = Every Thursday, start_date = 2019-11-07 01:05, end_date = 2019-11-07 03:25, },
//	 { id = 1, text = Every Thursday, start_date = 2019-11-14 01:05, end_date = 2019-11-14 03:25, }
// ]
```

In order to use the helper with custom model classes, you'll need to manually convert them to **DHTMLX.Scheduler.RecurringEvents.SchedulerEvent** for **GetOccurrences** method:

```
// get events scheduled for next three days
var currentDate = DateTime.Now;
var upUntilDate = currentDate.AddDays(3);

var rawEvents = _context.RecurringEvents.Where(e => e.StartDate < upUntilDate && e.EndDate > currentDate);
var helper = new RecurringEventsHelper();

var upcomingEvents = helper.GetOccurrences(
    rawEvents.Select(e => new SchedulerEvent
    {
        id = e.Id.ToString(),
        text = e.Name,
        start_date = e.StartDate,
        end_date = e.EndDate,
        event_length = e.EventLength,
        event_pid = e.EventPID.ToString(),
        rec_type = e.RecType
    }).ToList(),
    currentDate,
    upUntilDate
);
```

### UTC Dates

If you use [occurrence_timestamp_in_utc](https://docs.dhtmlx.com/scheduler/api__scheduler_occurrence_timestamp_in_utc_config.html) config on the client, you'll need to specify it on the backend as well:

```
var helper = new RecurringEventsHelper
{
    OccurrenceTimestampInUtc = true
};
```

## License

Copyright (c) 2019 XB Software Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
OR OTHER DEALINGS IN THE SOFTWARE.


## Useful links

- [dhtmlxScheduler product page](https://dhtmlx.com/docs/products/dhtmlxScheduler)
- [dhtmlxScheduler documentation](https://docs.dhtmlx.com/scheduler/)
- [Support forum](https://forum.dhtmlx.com/c/scheduler-all)