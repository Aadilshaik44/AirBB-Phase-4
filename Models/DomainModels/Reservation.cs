using System.ComponentModel.DataAnnotations;

namespace AirBB.Models.DomainModels
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReservationStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReservationEndDate { get; set; }

        public int ResidenceId { get; set; }
        public Residence? Residence { get; set; } 
    }
}