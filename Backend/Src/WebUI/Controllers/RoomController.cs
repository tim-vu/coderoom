using System.Threading.Tasks;
using Application.Rooms;
using Application.Rooms.Commands.CreateRoom;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class RoomController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<RoomVm>> CreateRoom()
        {
            return await Mediator.Send(new CreateRoom());
        }
    }
}