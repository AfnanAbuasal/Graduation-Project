using Sconce.BLL.Services.Interfaces;
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
            var hasAccess = parentRelations.Any(sp => sp.StudentId == studentId);

            if (!hasAccess)
                return (false, "Parent does not have access to this student's data.");

            return (true, string.Empty);
        }
    }
}
