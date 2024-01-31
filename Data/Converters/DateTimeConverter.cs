using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Galaxon.Astronomy.Data.Converters;

public class DateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeConverter() : base(
        v => v,
        v => new DateTime(v.Ticks, DateTimeKind.Utc)
    ) { }
}
