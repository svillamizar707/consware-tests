namespace TravelRequests.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Solicitante"; // 'Solicitante' o 'Aprobador'

        // Relación
        public ICollection<TravelRequest> TravelRequests { get; set; } = new List<TravelRequest>();
    }
}
