using Microsoft.AspNetCore.Identity;

namespace Chatting.Api.Domain.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public ICollection<ApplicationUser> Users { get; } = [];

}
