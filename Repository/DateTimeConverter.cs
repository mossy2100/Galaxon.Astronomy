using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AstroMultimedia.Astronomy.Repository;

public class DateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeConverter() : base(
        v => v,
        v => new DateTime(v.Ticks, DateTimeKind.Utc)
    )
    {
    }
}
