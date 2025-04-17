using ProfanityFilter.Interfaces;
using RookieShop.ProductReview.Application.Abstractions;

namespace RookieShop.ProductReview.Infrastructure.ProfanityChecker;

public class ProfanityCheckerAdapter : IProfanityChecker
{
    private readonly IProfanityFilter _profanityFilter;

    public ProfanityCheckerAdapter(IProfanityFilter profanityFilter)
    {
        _profanityFilter = profanityFilter;
    }

    public ValueTask<bool> CheckProfanityAsync(string text, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_profanityFilter.IsProfanity(text));
    }
}