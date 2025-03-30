using Chat.Contracts;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(string userName, string message);
}

public class ChatHub : Hub<IChatClient>, IChatHub
{
    private List<UserConnection> _connections = new List<UserConnection>();
    
    public async Task JoinChat(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatName);

        await Clients.Group(connection.ChatName).ReceiveMessage("System", $"{connection.UserName} joined the chat");
    }

    public async Task SendMessage(string userName, string message)
    {
        var connection = _connections.FirstOrDefault(x => x.UserName == userName);
        
        if (connection != null)
        {
            await Clients.Group(connection.ChatName).ReceiveMessage("System", $"{connection.UserName} joined the chat");
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        
        return base.OnDisconnectedAsync(exception);
    }
}