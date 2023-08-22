using Microsoft.AspNetCore.SignalR;

namespace Vihzin.Hubs
{
    public class VideoHub : Hub
    {
        private readonly ILogger<VideoHub> _logger;
        public VideoHub(ILogger<VideoHub> logger)
        {
            _logger = logger;
        }
        [HubMethodName("SendOffer")]
        public async Task SendOffer(string offer, string targetUserId)
        {
            // Enviar la oferta al usuario de destino.
            await Clients.Client(targetUserId).SendAsync("ReceiveOffer", offer, Context.ConnectionId);
        }

        // Método para enviar una respuesta a una oferta.
        public async Task SendAnswer(string answer, string senderConnectionId)
        {
            // Enviar la respuesta al usuario que hizo la oferta.
            await Clients.Client(senderConnectionId).SendAsync("ReceiveAnswer", answer);
        }

        // Método para enviar un candidato ICE a otro usuario.
        public async Task SendIceCandidate(string iceCandidate, string targetUserId)
        {
            // Enviar el candidato ICE al usuario de destino.
            await Clients.Client(targetUserId).SendAsync("ReceiveIceCandidate", iceCandidate);
        }

        public override Task OnConnectedAsync()
        {
            // Aquí puedes realizar acciones cuando un usuario se conecta.
            // Por ejemplo, notificar a otros usuarios o registrar el inicio de sesión.

            // Para notificar a todos los clientes conectados (excepto al cliente actual):
            Clients.Others.SendAsync("UserConnected", Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Aquí puedes realizar acciones cuando un usuario se desconecta.
            // Por ejemplo, notificar a otros usuarios o registrar el cierre de sesión.
            Clients.Others.SendAsync("UserDisconnected", Context.ConnectionId);
            // Realiza cualquier acción que necesites con el connectionId del cliente que se desconectó.

            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("EntryGroup")]
        public async Task EntryGroupById(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            _logger.LogDebug("Cliente {clientId} is connected in group {roomId}", Context.ConnectionId, roomId);
        }

        [HubMethodName("LeaveGroup")]
        public async Task LeaveGroupById(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            _logger.LogDebug("Cliente {clientId} left a group {roomId}", Context.ConnectionId, roomId);
        }

        [HubMethodName("ChatGroup")]
        public async Task ChatInGroupById(string roomId, string message)
        {
            await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync(message);
            _logger.LogDebug("Cliente {connectionId} send message to group {roomId}", Context.ConnectionId, roomId);
        }

        [HubMethodName("AudioRoomEntry")]
        public async Task AudioRoomEntry(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"AudioRoom{roomId}");
            _logger.LogDebug("Cliente {clientId} was connected in group {roomId}", Context.ConnectionId, roomId);
        }

        [HubMethodName("AudioRoomListen")]
        public async Task AudioRoomListen(string roomId, byte[] audioData)
        {
            await Clients.GroupExcept($"AudioRoom{roomId}", Context.ConnectionId).SendAsync("StreamAudio", audioData);
        }

        [HubMethodName("AudioRoomListen2")]
        public async Task AudioRoomListen2(string roomId, string ddd)
        {
            Console.WriteLine($"test {roomId} {ddd}");
        }
    }
}
