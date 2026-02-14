namespace DotnetAPI.Dtos
{
    public partial class UserForLoginConfirmationDTO
    {
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        UserForLoginConfirmationDTO()
        {
            if (passwordHash == null) passwordHash = new byte[0];
            if (passwordSalt == null) passwordSalt = new byte[0];
        }
    }
}