using System;

namespace HospitalApp.Web.HospitalApi.Models
{
    public class SimpleDepartment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DoctorCount { get; set; }
    }
}
