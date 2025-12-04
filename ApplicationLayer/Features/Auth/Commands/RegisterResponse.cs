namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
        public string Message { get; set; } = "Registration successful. Please wait for admin approval.";
    }
}

