using Facets.Core.Security.Entities;
using Facets.Core.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Security;

internal sealed class OTPRepository : BaseRepository, IOTPRepository
{
    private readonly DbSet<OTPQueue> _table;

    public OTPRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = dbContext.Set<OTPQueue>();
    }


    public OTPQueue Add(OTPQueue otp)
    {
        _table.Add(otp);

        return otp;
    }

    public async Task<OTPQueue?> GetOTPToVerify(string identityNumber)
    {
        var otp = await _table.AsTracking()
                              .Where(w => w.IdentityNumber == identityNumber)
                              .OrderByDescending(w => w.CreatedOn)
                              .FirstOrDefaultAsync();

        return otp;
    }

    public async Task<int> CountSentSince(string identityNumber, DateTimeOffset since)
    {
        return await _table.CountAsync(w => w.IdentityNumber == identityNumber && w.CreatedOn >= since);
    }
}
