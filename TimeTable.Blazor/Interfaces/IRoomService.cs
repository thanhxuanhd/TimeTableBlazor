using Radzen;
using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface IRoomService
{
    List<RoomDto> GetRooms(LoadDataArgs args, out int count);
    List<RoomDto> GetRooms(LoadDataArgs args);
    void CreateRoom(RoomDto room);
}