using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AstroMultimedia.Astronomy.Repository;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        v => new DateTime(v.Year, v.Month, v.Day),
        v => new DateOnly(v.Year, v.Month, v.Day)
    )
    {
    }
}
