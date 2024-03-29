﻿using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using TimeTable.Blazor.Interfaces;
using TimeTable.Domain;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly TimeTableDbContext _context;

        private readonly ILogger<TeacherService> _logger;

        private const string ErrorMessage = "Error: {message}";

        public TeacherService(TimeTableDbContext context, ILogger<TeacherService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Tuple<bool, List<string>> CreateTeacher(TeacherDto teacherDto)
        {
            var success = true;
            var errors = new List<string>();
            try
            {
                if (!ValidateTeacher(teacherDto, errors))
                {
                    success = false;
                    return Tuple.Create(success, errors);
                }

                if (IsDuplicate(teacherDto.Code, errors))
                {
                    success = false;
                    return Tuple.Create(success, errors);
                }

                var teacher = new Teacher()
                {
                    Id = Guid.NewGuid(),
                    Code = teacherDto?.Code.Trim(),
                    Email = teacherDto?.Email.Trim(),
                    FirstName = teacherDto?.FirstName.Trim(),
                    LastName = teacherDto?.LastName.Trim(),
                };

                _context.Add(teacher);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                AddError(errors, ex.Message);
                success = false;
            }

            return Tuple.Create(success, errors);
        }

        public Tuple<bool, List<string>> DeleteTeacher(Guid id)
        {
            var success = true;
            var errors = new List<string>();

            try
            {
                var teacher = GetTeacher(id);

                if (teacher is null)
                {
                    AddError(errors, "Teacher doesn't exist.");
                    success = false;
                    return Tuple.Create(success, errors);
                }

                if (!CanRemoveTeacher(teacher.Id))
                {
                    AddError(errors, $"Teacher [{teacher.Code}] in used.");
                    success = false;
                    return Tuple.Create(success, errors);
                }

                _context.Remove(teacher);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                AddError(errors, ex.Message);
                success = false;
            }

            return Tuple.Create(success, errors);
        }

        public TeacherDto GetTeacherById(Guid id)
        {
            var teacher = GetTeacher(id);

            return teacher is null ? new TeacherDto() : new TeacherDto()
            {
                Code = teacher.Code?.Trim(),
                FirstName = teacher.FirstName?.Trim(),
                LastName = teacher.LastName?.Trim(),
                Id = teacher.Id,
                Email = teacher.Email?.Trim(),
            };
        }

        public List<TeacherDto> GetTeachers(LoadDataArgs args, out int count)
        {
            var query = _context.Teachers.AsQueryable();

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
            return query.Skip(args.Skip.Value).Take(args.Top.Value).Select(s => new TeacherDto()
            {
                Code = s.Code,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Id = s.Id,
                Email = s.Email
            }).AsNoTracking().ToList();
        }

        public List<TeacherDto> GetTeachers(LoadDataArgs args)
        {
            var query = _context.Teachers.AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(c => c.Code.ToLower().Contains(args.Filter.ToLower()) || c.FirstName.ToLower().Contains(args.Filter.ToLower()));
            }

            return query.Select(s => new TeacherDto()
            {
                Code = s.Code,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Id = s.Id,
                Email = s.Email
            }).AsNoTracking().ToList();
        }

        public Tuple<bool, List<string>> UpdateTeacher(TeacherDto teacherDto)
        {
            var success = true;
            var errors = new List<string>();
            try
            {
                if (!ValidateTeacher(teacherDto, errors))
                {
                    success = false;
                    return Tuple.Create(success, errors);
                }

                if (IsDuplicate(teacherDto.Code, errors, teacherDto.Id.Value))
                {
                    success = false;
                    return Tuple.Create(success, errors);
                }

                var teacher = GetTeacher(teacherDto.Id.Value);

                if (teacher is null)
                {
                    AddError(errors, "Teacher doesn't exist.");
                    success = false;
                    return Tuple.Create(success, errors);
                }

                teacher.Code = teacherDto?.Code.Trim();
                teacher.FirstName = teacherDto.FirstName?.Trim();
                teacher.LastName = teacherDto.LastName?.Trim();
                teacher.Email = teacherDto.Email?.Trim();

                _context.Update(teacher);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                success = false;
                AddError(errors, ex.Message);
            }

            return Tuple.Create(success, errors);
        }

        public bool ValidateTeacher(TeacherDto teacher, List<string> errors)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(teacher.Code))
            {
                isValid = false;
                AddError(errors, "Teacher Code is required.");
            }

            if (string.IsNullOrEmpty(teacher.FirstName))
            {
                isValid &= false;
                AddError(errors, "Teacher First Name is required.");
            }

            if (isValid && teacher.Code?.Length > 20)
            {
                isValid &= false;
                AddError(errors, "Teacher Code greater than  greater than 20 characters.");
            }

            return isValid;
        }

        private bool IsDuplicate(string code, List<string> errors, Guid? id = null)
        {
            Teacher teacher = null;

            if (!id.HasValue)
            {
                teacher = _context.Teachers.AsEnumerable().FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                teacher = _context.Teachers.AsEnumerable().FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && s.Id != id);
            }

            var isDuplicate = teacher is not null;
            if (isDuplicate)
            {
                AddError(errors, $"Teacher is duplicate with [{code}]");
            }

            return isDuplicate;
        }

        private Teacher GetTeacher(Guid id)
        {
            return _context.Teachers.FirstOrDefault(t => t.Id == id);
        }

        private void AddError(List<string> errors, string message)
        {
            errors.Add(message);
            _logger.LogError(ErrorMessage, message);
        }

        private bool CanRemoveTeacher(Guid teacherId)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.TeacherId == teacherId);
            return subject is null;
        }
    }
}