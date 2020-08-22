using Domain.Enums;

namespace Application.Rooms.RoomService
{
    public interface ITemplateService
    {
        string GetLanguageTemplate(Language language);
    }
}