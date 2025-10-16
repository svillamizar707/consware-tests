namespace TravelRequests.Application.DTOs
{
    public class CreateTravelRequestDto
    {
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Justification { get; set; } = string.Empty;
    }
}
