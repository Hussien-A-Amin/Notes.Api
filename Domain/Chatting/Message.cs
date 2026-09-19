using Chatting.Api.Domain.Base;
using Chatting.Api.Domain.Identity;

namespace Chatting.Api.Domain.Chatting;

public class Note : AuditableEntity
{

    public string Content { get; set; } = default!;

    public ApplicationUser User { get; }=default!;
    public Guid UserId { get; set; }
    
    

}
