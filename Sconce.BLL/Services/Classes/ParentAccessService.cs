using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ParentAccessService : IParentAccessService
    {
        private readonly IStudentParentRepository _studentParentRepository;

        public ParentAccessService(IStudentParentRepository studentParentRepository)
        {
            _studentParentRepository = studentParentRepository;
        }

        public async Task<(bool HasAccess, string ErrorMessage)> ValidateParentAccessToStudentAsync(string parentId, string studentId)
        {
            // Check if parent has a StudentParent relationship with this student
            var parentRelations = await _studentParentRepository.GetByParentIdAsync(parentId);
            var hasAccess = parentRelations.Any(sp => sp.StudentId == studentId && sp.IsConfirmed);

            if (!hasAccess)
                return (false, "Parent does not have access to this student's data.");

            return (true, string.Empty);
        }

        public async Task<Response> GetChildrenAsync(string parentId)
        {
            try
            {
                var studentParentRelations = await _studentParentRepository.GetByParentIdAsync(parentId);

                // Filter only confirmed relationships
                var confirmedRelations = studentParentRelations.Where(sp => sp.IsConfirmed).ToList();

                var childrenList = new List<ChildInfoResponse>();

                foreach (var relation in confirmedRelations)
                {
                    var student = relation.Student;
                    
                    var childInfo = new ChildInfoResponse
                    {
                        Id = student.Id,
                        FullName = student.FullName,
                        Email = student.Email,
                        RelationshipWithStudent = relation.RelationshipWithStudent,
                        LinkedAt = relation.LinkedAt,
                        Sections = new List<SimpleSectionResponse>()
                    };

                    // Map student sections
                    foreach (var studentSection in student.StudentSections)
                    {
                        var sectionResponse = new SimpleSectionResponse
                        {
                            Id = studentSection.SectionId,
                            Name = studentSection.Section.Name,
                            CourseName = studentSection.Section.Course?.Name ?? "N/A"
                        };
                        childInfo.Sections.Add(sectionResponse);
                    }

                    childrenList.Add(childInfo);
                }

                return new SuccessResponse<IEnumerable<ChildInfoResponse>> { Data = childrenList };
            }
            catch (Exception ex)
            {
                return new ErrorResponse { Errors = [ex.Message] };
            }
        }
    }
}
