﻿using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TinyChat.Core.Client.Command;
using TinyChat.Core.Client.Models;

namespace TinyChat.Core.Server
{
    internal partial class ChatServer
    {
        private void HandleCreateRoom(ChatCommand cmd)
        {
            _chat.CreateRoom(cmd.CreateRoomModel.CreatorName, cmd.SenderIdentifier, cmd.CreateRoomModel.RoomName);
        }

        private void HandleSendMessage(ChatCommand cmd)
        {
            _chat.SendMessage(cmd.SendMessageModel.RoomName, cmd.SendMessageModel.Text, cmd.SendMessageModel.CreatorName, cmd.SenderIdentifier);
        }

        private void HandleGetRooms(ChatCommand cmd)
        {
            var rooms = _chat.GetRooms();

            var json = JsonConvert.SerializeObject(rooms);
            byte[] data = Encoding.UTF8.GetBytes(json);

            var ip = cmd.SenderIdentifier.Split(':')[0];
            var port = Convert.ToInt32(cmd.SenderIdentifier.Split(':')[1]);
            
            _client.Send(data, data.Length, ip,  port);
        }

        private void HandleGetMessages(ChatCommand cmd)
        {
            var messages = _chat
                .GetMessages(cmd.GetMessagesModel.RoomName)
                .Select(t => new Message()
                {
                    Sender = t.SenderName,
                    Text = t.Text,
                    Time = t.CreatedDate.ToLongTimeString()
                }).ToList();

            var json = JsonConvert.SerializeObject(messages);
            byte[] data = Encoding.UTF8.GetBytes(json);

            var ip = cmd.SenderIdentifier.Split(':')[0];
            var port = Convert.ToInt32(cmd.SenderIdentifier.Split(':')[1]);

            _client.Send(data, data.Length, ip, port);
        }
    }
}
