using Application.Common.Interfaces;
using Application.Common.Mapping;
using AutoMapper;
using Moq;

namespace Application.Test.Common
{
    public class TestBase
    {
        protected IMapper Mapper { get; }

        public TestBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = new Mapper(config);
        }
    }
}