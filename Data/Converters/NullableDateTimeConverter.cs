using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Galaxon.Astronomy.Data.Converters;

public class NullableDateTimeConverter() : ValueConverter<DateTime?, DateTime?>(
    v => v,
    v => v == null ? null: new DateTime(v.Value.Ticks, DateTimeKind.Utc));
