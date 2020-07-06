namespace JwtAuthAspNetCoreApi.Models
{
    public class PasswordHashModel
    {
        public string HashText { get; set; }
        public string SaltText { get; set; }
        public int HashLength { get; set; }
        public int HashIteration { get; set; }
    }
}
