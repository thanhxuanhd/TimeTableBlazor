using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using TimeTable.Blazor.Interfaces;
using TimeTable.Domain;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Services;

public class RoomService : IRoomService
{
    private readonly TimeTableDbContext _context;

    private readonly ILogger<RoomService> _logger;

    private const string ErrorMessage = "Error: {message}";

    public RoomService(TimeTableDbContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Tuple<bool, List<string>> CreateRoom(RoomDto room)
    {
        var success = true;
        var errors = new List<string>();
        try
        {
            if (!ValidationRoom(room, errors))
            {
                success = false;
            }

            if (IsDuplicate(room.Code, errors))
            {
                success &= false;
            }

            if (!success)
            {
                return Tuple.Create(success, errors);
            }

            var newRoom = new Room()
            {
                Id = Guid.NewGuid(),
                Code = room.Code?.Trim(),
                Location = room.Location?.Trim()
            };

            _context.Add(newRoom);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ErrorMessage, ex.Message);
            success = false;
        }

        return Tuple.Create(success, errors);
    }

    public Tuple<bool, List<string>> DeleteRoom(Guid id)
    {
        var success = true;
        var errors = new List<string>();
        string message;
        try
        {
            var room = GetRoom(id);

            if (room is null)
            {

                AddError(errors, $"Room doesn't exist with Id: [{id}]");
                success = false;
                return Tuple.Create(success, errors);
            }

            if (!CanRemoveRoom(room.Id))
            {
                AddError(errors, $"Room with: [{room.Code}] in-used.");
                success = false;
                return Tuple.Create(success, errors);
            }

            _context.Remove(room);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            message = ex.Message;
            errors.Add(message);
            _logger.LogError("Error: {message}", message);
            success = false;
        }

        return Tuple.Create(success, errors);
    }

    public RoomDto GetRoomById(Guid id)
    {
        var room = GetRoom(id);

        return room is not null ? new RoomDto()
        {
            Code = room.Code?.Trim(),
            Id = room.Id,
            Location = room.Location?.Trim()
        } : new RoomDto();
    }

    public List<RoomDto> GetRooms(LoadDataArgs args, out int count)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrEmpty(args.Filter))
        {
            // Filter via the Where method
            query = query.Where(args.Filter);
        }

        if (!string.IsNullOrEmpty(args.OrderBy))
        {
            // Sort via the OrderBy method
            query = query.OrderBy(args.OrderBy);
        }

        // Important!!! Make sure the Count property of RadzenDataGrid is set.
        count = query.Count();

        // Perform paging via Skip and Take.
        return query.Skip(args.Skip.Value).Take(args.Top.Value).Select(s => new RoomDto()
        {
            Code = s.Code,
            Location = s.Location,
            Id = s.Id
        }).AsNoTracking().ToList();
    }

    public List<RoomDto> GetRooms(LoadDataArgs args)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrEmpty(args.Filter))
        {
            query = query.Where(c => c.Code.ToLower().Contains(args.Filter.ToLower()) || c.Location.ToLower().Contains(args.Filter.ToLower()));
        }

        return query.Select(s => new RoomDto()
        {
            Code = s.Code,
            Location = s.Location,
            Id = s.Id
        }).AsNoTracking().ToList();
    }

    public Tuple<bool, List<string>> UpdateRoom(RoomDto room)
    {
        var success = true;
        var errors = new List<string>();
        try
        {
            if (!ValidationRoom(room, errors))
            {
                success = false;
            }

            if (IsDuplicate(room.Code, errors, room.Id))
            {
                success &= false;
            }

            if (!success)
            {
                return Tuple.Create(success, errors);
            }

            var updateRoom = _context.Rooms.FirstOrDefault(r => r.Id == room.Id);

            if (room is null)
            {
                success = false;
                AddError(errors, $"The room doesn't exist with Id: [{room.Id}]");
                return Tuple.Create(success, errors);
            }
            updateRoom.Location = room.Location?.Trim();
            updateRoom.Code = room.Code?.Trim();

            _context.Update(updateRoom);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            AddError(errors, ex.Message);
            success = false;
        }

        return Tuple.Create(success, errors);
    }

    private bool ValidationRoom(RoomDto room, List<string> errors)
    {
        var valid = true;

        if (string.IsNullOrEmpty(room.Code))
        {
            valid = false;
            AddError(errors, "Room Code is required.");

        }

        if (valid && room.Code?.Length > 20)
        {
            valid &= false;
            AddError(errors, "Room Code greater than 20 characters.");
        }

        return valid;
    }

    private bool IsDuplicate(string roomCode, List<string> errors, Guid? id = null)
    {
        var isDuplicate = false;
        Room roomExist;

        if (!id.HasValue)
        {
            roomExist = _context.Rooms.AsEnumerable().FirstOrDefault(s => s.Code.Equals(roomCode, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            roomExist = _context.Rooms.AsEnumerable().FirstOrDefault(s => s.Code.Equals(roomCode, StringComparison.OrdinalIgnoreCase) && s.Id != id);
        }

        isDuplicate = roomExist is not null;

        if (isDuplicate)
        {
            AddError(errors, $"Room is duplicate with Code [{roomCode}]");
        }
        return isDuplicate;
    }

    private Room GetRoom(Guid id)
    {
        return _context.Rooms.Include(s => s.Subjects).FirstOrDefault(r => r.Id == id);
    }

    private bool CanRemoveRoom(Guid roomId)
    {
        var session = _context.Sessions.Include(s => s.Room).FirstOrDefault(s => s.RoomId == roomId);
        return session is null;
    }

    private void AddError(List<string> errors, string message)
    {
        errors.Add(message);
        _logger.LogError(ErrorMessage, message);
    }
}