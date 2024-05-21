namespace PostService.AsyncDataServices;

public interface IMessage { }

public class Message : IMessage
{
    public string action { get; set; }
    public int postId { get; set; }
}

public class DeleteUserMessage : IMessage
{
    public string userId { get; set; }
}
