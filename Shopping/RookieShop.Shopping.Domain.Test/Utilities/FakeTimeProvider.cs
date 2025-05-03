namespace RookieShop.Shopping.Domain.Test.Utilities;

public class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _fakeUtcNow;

    public FakeTimeProvider(DateTimeOffset fakeUtcNow)
    {
        _fakeUtcNow = fakeUtcNow;
    }
    
    public override DateTimeOffset GetUtcNow() => _fakeUtcNow;
}