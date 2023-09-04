using Radzen;
using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface IRoomService
{
    List<RoomDto> GetRooms(LoadDataArgs args, out int count);

    List<RoomDto> GetRooms(LoadDataArgs args);

    Tuple<bool, List<string>> CreateRoom(RoomDto room);

    Tuple<bool, List<string>> UpdateRoom(RoomDto room);

    Tuple<bool, List<string>> DeleteRoom(Guid id);

    RoomDto GetRoomById(Guid id);
}