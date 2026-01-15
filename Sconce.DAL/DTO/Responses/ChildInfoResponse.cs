using System;
using System.Collections.Generic;

namespace Sconce.DAL.DTO.Responses
{
    public class ChildInfoResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RelationshipWithStudent { get; set; }
        public DateTime LinkedAt { get; set; }
        public ICollection<SimpleSectionResponse> Sections { get; set; } = new List<SimpleSectionResponse>();
    }

    public class SimpleSectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CourseName { get; set; }
    }
}
