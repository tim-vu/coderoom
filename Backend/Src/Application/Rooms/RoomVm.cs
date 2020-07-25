using System;
using System.Collections.Generic;
using Application.Common.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Rooms
{
    public class RoomVm : IMapFrom<Room>
    {
        public string Id { get; set; }
        
        public string Text { get; set; }
        
        public List<User> Users { get; set; }
        
        public DateTime LastEdit { get; set; }
        
        public string TypingUserConnectionId { get; set; }
        
        public Language Language { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Room, RoomVm>();
        }
    }
}