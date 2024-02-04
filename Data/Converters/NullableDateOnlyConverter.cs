using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Galaxon.Astronomy.Data.Converters;

public class NullableDateOnlyConverter() : ValueConverter<DateOnly?, DateTime?>(
    v => v == null ? null : new DateTime(v.Value.Year, v.Value.Month, v.Value.Day),
    v => v == null ? null : new DateOnly(v.Value.Year, v.Value.Month, v.Value.Day));
