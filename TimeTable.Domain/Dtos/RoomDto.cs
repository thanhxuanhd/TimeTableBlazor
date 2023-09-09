namespace TimeTable.Domain.Dtos;

public class RoomDto
{
    public Guid? Id { get; set; }

    public string Code { get; set; }

    public string Location { get; set; }

    public string Name => $"{Code} - {Location}";
}

public class RoomImportDto
{
    public string Code { get; set; }

    public string Location { get; set; }
}