using AirBB.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirBB.Models.DataLayer.Configuration
{
    public class ClientConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            entity.HasData(
                new Client { UserId = 1, Name = "Admin Owner", Email = "owner@airbb.com", PhoneNumber = "123-456-7890", DOB = new DateTime(1980, 1, 1), UserType = "Owner" },
                new Client { UserId = 2, Name = "Test Client", Email = "client@airbb.com", PhoneNumber = "987-654-3210", DOB = new DateTime(1990, 5, 20), UserType = "Client" }
            );
        }
    }
}