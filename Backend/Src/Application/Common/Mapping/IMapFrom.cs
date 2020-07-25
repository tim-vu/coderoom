using AutoMapper;

namespace Application.Common.Mapping
{
    public interface IMapFrom<TSource>
    {
        void Mapping(Profile profile)
        {
            profile.CreateMap(typeof(TSource), GetType());
        }
    }
}