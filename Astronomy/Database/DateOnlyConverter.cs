using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Galaxon.Astronomy.Database;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        v => new DateTime(v.Year, v.Month, v.Day),
        v => new DateOnly(v.Year, v.Month, v.Day)
    ) { }
}
