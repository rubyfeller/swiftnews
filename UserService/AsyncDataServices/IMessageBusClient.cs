namespace UserService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void RemoveUser(string userId);
    }
}