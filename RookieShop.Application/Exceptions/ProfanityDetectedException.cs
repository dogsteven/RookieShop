namespace RookieShop.Application.Exceptions;

public class ProfanityDetectedException : Exception
{
    public ProfanityDetectedException() : base("Profanity detected.") {} 
}