namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
        public string Message { get; set; } = "Registration successful! Your account is pending admin approval. You will be able to login once an administrator approves your organization. Please wait for the approval email.";
        public bool RequiresApproval { get; set; } = true;
    }
}

