using System.Collections.Concurrent;
using Chat.Contracts;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs;

public class ChatHub : Hub<IChatClient>, IChatHub
{
    private static ConcurrentDictionary<string, UserConnection> _connections = new();
    
    public async Task JoinChat(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatName);
        
        _connections[Context.ConnectionId] = connection;

        await Clients.Group(connection.ChatName).ReceiveMessage("System", $"{connection.UserName} joined the chat");
    }

    public async Task SendMessage(string message)
    {
        _connections.TryGetValue(Context.ConnectionId, out var connection);
        
        if (connection != null)
        {
            await Clients.Group(connection.ChatName)
                .ReceiveMessage(connection.UserName, message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connections.TryRemove(Context.ConnectionId, out var connection))
        {
            await Clients.Group(connection.ChatName)
                .ReceiveMessage("System", $"{connection.UserName} left the chat");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatName);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}