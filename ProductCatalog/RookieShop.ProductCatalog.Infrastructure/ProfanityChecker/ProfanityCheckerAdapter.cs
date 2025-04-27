using ProfanityFilter.Interfaces;
using RookieShop.ProductCatalog.Application.Abstractions;

namespace RookieShop.ProductCatalog.Infrastructure.ProfanityChecker;

public class ProfanityCheckerAdapter : IProfanityChecker
{
    private readonly IProfanityFilter _profanityFilter;

    public ProfanityCheckerAdapter(IProfanityFilter profanityFilter)
    {
        _profanityFilter = profanityFilter;
    }

    public ValueTask<bool> CheckProfanityAsync(string text, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_profanityFilter.DetectAllProfanities(text).Count > 0);
    }
}