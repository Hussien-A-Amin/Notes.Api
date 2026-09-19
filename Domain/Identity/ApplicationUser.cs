using Chatting.Api.Domain.Chatting;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace Chatting.Api.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? Address { get; set; }
    public ICollection<ApplicationRole> Roles { get; } = [];
    
    
    public List<Note> Notes { get; } = [];

}
