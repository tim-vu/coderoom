using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Room
    {
        public string Id { get; set; }
        
        public string CurrentExecutionJobId { get; set; }

        public Language Language { get; set; } = Language.Java;
        
        public string Text { get; set; } = string.Empty;

        public DateTime LastEdit { get; set; } = DateTime.MinValue;
        
        public string TypingUserConnectionId { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        
        public List<string> Output = new List<string>();
    }
}