using Chatting.Api.Domain.Chatting;

namespace Chatting.Api.Application.Data;

public class MessageRepo(ApplicationUnitOfWork dataUnit): BaseRepo<Note>(dataUnit)
{
    public async Task<Note> AddAsync (Note mesage)
    {
        await _dtSet.AddAsync(mesage);

        await dataUnit.SaveChangesAsync();

        return mesage;
    }








}