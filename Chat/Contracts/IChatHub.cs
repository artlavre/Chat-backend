using Chat.Models;

namespace Chat.Contracts;

public interface IChatHub
{
    public Task JoinChat(UserConnection connection);
}