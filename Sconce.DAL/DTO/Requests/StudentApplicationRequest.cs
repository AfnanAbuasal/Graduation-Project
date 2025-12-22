using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Requests
{
    public class StudentApplicationRequest
    {
        [Required] public string Email { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required] public DateOnly DateOfBirth { get; set; }
        [Required] public Gender Gender { get; set; }
        [Required] public string Country { get; set; }
        [Required] public string City { get; set; }
        public string? Street { get; set; }
        [Required] public IFormFile Document { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianEmail { get; set; }
        [Required]
        [EnumDataType(typeof(LevelOfProficiency))]
        [Range((int)LevelOfProficiency.None, (int)LevelOfProficiency.C2, ErrorMessage = "Level of proficiency must be a valid option.")]
        public LevelOfProficiency LevelOfProficiency { get; set; }
    }
}
