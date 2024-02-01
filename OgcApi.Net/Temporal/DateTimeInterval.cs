using System;
using System.Globalization;

namespace OgcApi.Net.Temporal;

public class DateTimeInterval(DateTime? start, DateTime? end)
{
    public DateTime? Start { get; set; } = start;

    public DateTime? End { get; set; } = end;

    public static DateTimeInterval Parse(string dateTime)
    {
        if (dateTime is null)
        {
            return new DateTimeInterval(null, null);
        }

        DateTime? startDate, endDate;
        if (!dateTime.Contains('/'))
        {
            startDate = DateTime.Parse(dateTime, CultureInfo.InvariantCulture);
            endDate = startDate;
        }
        else
        {
            var tokens = dateTime.Split('/');

            if (tokens[0] == "..")
            {
                startDate = null;
            }
            else
            {
                startDate = DateTime.Parse(tokens[0], CultureInfo.InvariantCulture);
            }

            if (tokens[1] == "..")
            {
                endDate = null;
            }
            else
            {
                endDate = DateTime.Parse(tokens[1], CultureInfo.InvariantCulture);
            }
        }

        return new DateTimeInterval(startDate, endDate);
    }
}