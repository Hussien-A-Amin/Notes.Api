namespace Chatting.Api.Application.Data;

public sealed class ApplicationUnitOfWork
{

    public readonly ApplicationDbContext _context;
    public readonly ILogger<ApplicationUnitOfWork> logger;
    public List<string> Errors { get; internal set; }

    public ApplicationUnitOfWork(ApplicationDbContext context, ILogger<ApplicationUnitOfWork> logger)
    {
        _context = context;
        this.logger = logger;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }


    #region Repos


    public MessageRepo Messages => new(this);







    #endregion




}
