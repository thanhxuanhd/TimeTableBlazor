﻿using System.ComponentModel.DataAnnotations;

namespace TimeTable.Domain.Dtos;

public class StudentDto
{
    public Guid? Id { get; set; }

    public string Code { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}

public class StudentImportDto
{
    public string Code { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}