namespace TravelRequests.Domain.Entities
{
    public class TravelRequest
    {
        public int Id { get; set; }
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string Status { get; set; } = "Pendiente"; // Pendiente, Aprobada, Rechazada

        // Relación
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
