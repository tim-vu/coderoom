using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.CodeExecution.Commands.CreateExecutionJob;
using Application.Rooms;
using Application.Rooms.Commands.ChangeLanguage;
using Application.Rooms.Commands.JoinGroupCall;
using Application.Rooms.Commands.JoinRoom;
using Application.Rooms.Commands.LeaveRoom;
using Application.Rooms.Commands.UpdateText;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace WebUI.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IMediator _mediator;

        public RoomHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RoomVm> JoinRoom(string roomId, string nickName)
        {
           var room = await _mediator.Send(new JoinRoom(roomId, Context.ConnectionId, nickName));
           Context.Items.Add("RoomId", room.Id);

           return room;
        }

        public Task LeaveRoom()
        {
            if (Context.Items.ContainsKey("RoomId"))
            {
                var roomId = (string) Context.Items["RoomId"];
                Context.Items.Clear();
                return _mediator.Send(new LeaveRoom(roomId, Context.ConnectionId));
            }

            return Task.CompletedTask;
        }

        public Task UpdateText(string text)
        {
            var roomId = (string)Context.Items["RoomId"];
            return _mediator.Send(new UpdateText(roomId, Context.ConnectionId, text));
        }

        
        public Task ChangeLanguage(Language language)
        {
            var roomId = (string) Context.Items["RoomId"];
            return _mediator.Send(new ChangeLanguage(roomId, Context.ConnectionId, language));
        }

        public Task StartCodeExecution()
        {
            var roomId = (string) Context.Items["RoomId"];
            return _mediator.Send(new CreateExecutionJob(Context.ConnectionId, roomId));
        }

        public Task JoinGroupCall()
        {
            var roomId = (string) Context.Items["RoomId"];
            return _mediator.Send(new JoinGroupCall(roomId, Context.ConnectionId));
        }
        
        public Task SendOfferData(string connectionId, string data)
        {
            return Clients.Client(connectionId).SendAsync("OnOfferDataReceived", Context.ConnectionId, data);
        }

        public Task SendAnswerData(string connectionId, string data)
        {
            return Clients.Client(connectionId).SendAsync("OnAnswerDataReceived", Context.ConnectionId, data);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.Items.ContainsKey("RoomId"))
            {
                await _mediator.Send(new LeaveRoom(Context.Items["RoomId"].ToString(), Context.ConnectionId));
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}