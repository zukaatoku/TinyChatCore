using System;
using System.Linq;
using TinyChat.Core.Domain.Interfaces;
using Xunit;

namespace TinyChat.Core.Domain.Tests
{
    public class TinyChatDomainTest
    {
        private const string DefaultRoom = "Default";
        
        [Fact]
        public void Chat_Can_Send_Message()
        {
            var chat = GetChatWithDefaultRoom();
            var fakeUserName = "testUser";
            var fakeUserId = "testId";
            var fakeMessage = "testMessage";
            var sendingTime = DateTime.Now.ToLongTimeString();
            
            chat.SendMessage(DefaultRoom, fakeMessage, fakeUserName, fakeUserId);

            var message = chat.GetMessages(DefaultRoom).FirstOrDefault();
            Assert.NotNull(message);
            var messageTime = message.CreatedDate.ToLongTimeString();
            Assert.Equal(fakeUserName, message.SenderName);
            Assert.Equal(fakeUserId, message.SenderId);
            Assert.Equal(fakeMessage, message.Text);
            Assert.Equal(sendingTime, messageTime);
        }

        [Fact]
        public void Chat_Can_Create_Room_And_Send_Message()
        {
            var chat = GetEmptyChat();
            var fakeUserName = "testUser";
            var fakeUserId = "testId";
            var fakeMessage = "testMessage";
            var fakeRoomName = "testRoom";
            
            chat.CreateRoom(fakeUserName, fakeUserId, fakeRoomName);
            
            chat.SendMessage(fakeRoomName, fakeMessage, fakeUserName, fakeUserId);
        }
        
        [Fact]
        public void Chat_Can_Return_Rooms()
        {
            var chat = GetChatWithDefaultRoom();

            var rooms = chat.GetRooms();

            Assert.NotEmpty(rooms);
        }

        [Fact]
        public void Chat_Can_Return_Messages()
        {
            var chat = GetChatWithDefaultRoom_AndMessages();

            var messages = chat.GetMessages(DefaultRoom);

            Assert.NotEmpty(messages);
        }
        
        [Fact]
        public void Chat_Should_Create_Room()
        {
            var chat = GetEmptyChat();
            var fakeUserName = "testUser";
            var fakeUserId = "testId";
            var fakeRoomName = "testRoom";
            
            chat.CreateRoom(fakeUserName, fakeUserId, fakeRoomName);

            var createdRoom = chat.GetRooms().FirstOrDefault(r => r.Name == fakeRoomName);
            Assert.NotNull(createdRoom);
            Assert.Equal(fakeUserName, createdRoom.Creator);
            Assert.Equal(fakeUserId, createdRoom.CreatorId);
            Assert.Equal(fakeRoomName, createdRoom.Name);
        }

        private IChat GetChatWithDefaultRoom_AndMessages()
        {
            Chat chat = new Chat();
            chat.CreateRoom("fake", "fake", DefaultRoom);
            chat.SendMessage(DefaultRoom, "fake", "fake", "fake");
            return chat;
        }
        
        private IChat GetChatWithDefaultRoom()
        {
            Chat chat = new Chat();
            chat.CreateRoom("fake", "fake", DefaultRoom);
            return chat;
        }
        
        private IChat GetEmptyChat()
        {
            return new Chat();
        }
    }
}