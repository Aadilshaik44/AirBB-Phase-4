using AirBB.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirBB.Models.DataLayer.Configuration
{
    public class LocationConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> entity)
        {
            entity.HasData(
                new Location { LocationId = 1, Name = "Chicago" },
                new Location { LocationId = 2, Name = "New York" },
                new Location { LocationId = 3, Name = "Boston" },
                new Location { LocationId = 4, Name = "Miami" }
            );
        }
    }
}