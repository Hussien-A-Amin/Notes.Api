using System.ComponentModel.DataAnnotations;

namespace Chatting.Api.Domain.Base;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
