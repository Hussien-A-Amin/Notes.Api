using Chatting.Api.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Chatting.Api.Application.Data;

public class BaseRepo<T>(ApplicationUnitOfWork dataUnit) where T :BaseEntity
{
   protected DbSet<T> _dtSet = dataUnit._context.Set<T>();
    public async Task<bool> DeleteById(Guid guid)
    {
      var rs=  await _dtSet
            .Where(p=>p.Id==guid)
            .ExecuteDeleteAsync();

        if( rs != 1)
        {
            dataUnit.Errors.Add("Inconsistent result");
        }
        return true;
    }
}
