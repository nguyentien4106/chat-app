using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzyChat.Infrastructure.Configurations;

/// <summary>
/// Base configuration to apply common datetime column types for all entities
/// </summary>
public static class DateTimeConfigurationExtensions
{
    /// <summary>
    /// Configures all DateTime and DateTime? properties to use "timestamp without time zone"
    /// </summary>
    public static void ConfigureDateTimeColumns<TEntity>(this EntityTypeBuilder<TEntity> builder) 
        where TEntity : class
    {
        foreach (var property in builder.Metadata.GetProperties())
        {
            if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            {
                builder.Property(property.Name)
                    .HasColumnType("timestamp without time zone");
            }
        }
    }
}
