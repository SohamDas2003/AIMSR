using System;
using System.Collections.Generic;

namespace AIMSR.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }

    public class UserRolesViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<UserRolesViewModel> Roles { get; set; } = new List<UserRolesViewModel>();
    }

    public class AttendanceManagementViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    }

    public class MarksManagementViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string Course { get; set; } = string.Empty;
        public List<Marks> MarksRecords { get; set; } = new List<Marks>();
    }
} 