using Application.Common.Interfaces;

namespace Infrastructure.IdGenerator
{
    public class IdGenerator : IIdGenerator
    {
        private readonly IdGen.IdGenerator _generator = new IdGen.IdGenerator(0);
        
        public string GenerateId()
        {
            return _generator.CreateId().ToString();
        }
    }
}