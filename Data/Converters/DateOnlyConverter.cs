using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Galaxon.Astronomy.Data.Converters;

public class DateOnlyConverter() : ValueConverter<DateOnly, DateTime>(
    v => new DateTime(v.Year, v.Month, v.Day),
    v => new DateOnly(v.Year, v.Month, v.Day));
