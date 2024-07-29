using Microsoft.EntityFrameworkCore;
using Plata.Models.Entities;

namespace Plata.Controllers
{
    public partial class PositionController
    {
        private Position? GetPositionForEdit(int id)
        {
            return dbContext.Positions
                .Where(p => p.Id == id && p.Company.UserAccountId == _authenticatedUserId)
                .SingleOrDefault();
        }

        private Position? GetPositionForDelete(int id)
        {
            return dbContext.Positions
                .Where(p => p.Id == id && p.Company.UserAccountId == _authenticatedUserId)
                .Include(p => p.EmploymentContracts)
                .SingleOrDefault();
        }
    }
}