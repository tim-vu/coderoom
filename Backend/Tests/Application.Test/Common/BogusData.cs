using System.Collections.Generic;
using Bogus;
using Domain.Entities;
using Domain.Enums;

namespace Application.Test.Common
{
    public class BogusData
    {
        public static readonly Faker<User> User = new Faker<User>().Rules((faker, user) =>
        {
            user.NickName = faker.Internet.UserName();
            user.ConnectionId = faker.Random.Guid().ToString();
        });
        
        public static readonly Faker<Room> Room = new Faker<Room>().Rules((faker, room) =>
        {
            room.Id = faker.Random.Guid().ToString();
            room.Language = faker.Random.Enum<Language>();
            room.Text = faker.Lorem.Paragraph();
            room.Users = User.Generate(5);
        });
        
        public static readonly Faker<BinaryFile> File = new Faker<BinaryFile>().CustomInstantiator(f => new BinaryFile(f.System.FileName(), f.System.Random.Bytes(50))); 
    }
}