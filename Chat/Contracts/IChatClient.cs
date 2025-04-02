namespace Chat.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(string userName, string message);
}