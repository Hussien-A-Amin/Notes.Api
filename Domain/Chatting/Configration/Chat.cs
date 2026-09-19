using Chatting.Api.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Chatting.Api.Domain.Chatting;


public sealed class MessageConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> b)
    {
        b.ToTable("Notes");


        b.HasOne(c => c.User)
               .WithMany(c => c.Notes)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

       

    }
}