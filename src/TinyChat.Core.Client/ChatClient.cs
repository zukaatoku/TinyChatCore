using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using TinyChat.Core.Client.Command;
using TinyChat.Core.Client.Interfaces;
using TinyChat.Core.Client.Models;

namespace TinyChat.Core.Client
{
    public class ChatClient : IChatClient
    {
        private readonly UdpClient _udpClient;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly int _localPort;
        
        public ChatClient(int localPort, int serverPort, string serverIp)
        {
            _udpClient = new UdpClient(_localPort);
            _serverIp = serverIp;
            _serverPort = serverPort;
            _localPort = localPort;
        }
        
        public void CreateRoom(string roomName, string creatorName)
        {
            SendServerCommand(new ChatCommand()
            {
                Type = CommandType.CreateRoom,
                
                CreateRoomModel = new CreateRoomModel()
                {
                    CreatorName = creatorName,
                    RoomName = roomName
                }
            });
        }

        public void SendMessage(string roomName, string creatorName, string message)
        {
            SendServerCommand(new ChatCommand()
            {
                Type = CommandType.SendMessage,
                
                SendMessageModel = new SendMessageModel()
                {
                    Text = message,
                    RoomName = roomName,
                    CreatorName = creatorName
                }
            });
        }

        public List<Room> GetRooms()
        {
            SendServerCommand(new ChatCommand()
                {
                    Type = CommandType.GetRooms
                }
            );

            IPEndPoint addr = new IPEndPoint(IPAddress.Any, _localPort);
            var data = _udpClient.Receive(ref addr);
            var message = Encoding.UTF8.GetString(data);

            var rooms = JsonConvert.DeserializeObject<List<Room>>(message);
            return rooms;
        }

        public List<Message> GetMessages(string roomName)
        {
            throw new System.NotImplementedException();
        }

        private void SendServerCommand(ChatCommand command)
        {
            var json = JsonConvert.SerializeObject(command);
            SendJson(json);
        }

        private void SendJson(string json)
        {
            var data = Encoding.UTF8.GetBytes(json);
            _udpClient.Send(data, data.Length, _serverIp, _serverPort);
        }
    }
}