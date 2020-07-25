namespace Domain.Entities
{
    public class User
    {
        public string ConnectionId { get; set; }
        
        public string NickName { get; set; }
        
        public bool InGroupCall { get; set; }
    }
}