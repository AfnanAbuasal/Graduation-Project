namespace Sconce.DAL.DTO.Responses
{
    public class StudentSectionResponse
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int LevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
    }
}
