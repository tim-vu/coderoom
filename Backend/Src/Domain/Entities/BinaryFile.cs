namespace Domain.Entities
{
    //TODO: find a better name
    public class BinaryFile
    {
        public string Name { get; }
        
        public byte[] Content { get; }

        public BinaryFile(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }
    }
}