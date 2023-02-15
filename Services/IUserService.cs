namespace RabbitProcessHawk
{
    public interface IUserService
    {
        bool ValidateUser(string username, string password);
    }
}