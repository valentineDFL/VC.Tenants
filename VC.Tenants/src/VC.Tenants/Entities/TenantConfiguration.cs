
namespace VC.Tenants.Entities;

public class TenantConfiguration : ValueObject
{
    public const int AboutMinLength = 16;
    public const int AboutMaxLength = 256;

    public const int CurrencyMinLength = 3;
    public const int CurrencyMaxLength = 3;

    public const int LanguageMinLength = 2;
    public const int LanguageMaxLength = 3;

    public const int TimeZoneIdMinLength = 2;
    public const int TimeZoneIdMaxLength = 3;

    private TenantConfiguration(string about, string currency, string language, string timeZoneId)
    {
        About = about;
        Currency = currency;
        Language = language;
        TimeZoneId = timeZoneId;
    }

    private TenantConfiguration() { }

    public string About { get; private set; }

    public string Currency { get; private set; }

    public string Language { get; private set; }

    /// <remarks>https://en.wikipedia.org/wiki/List_of_tz_database_time_zones</remarks>
    public string TimeZoneId { get; private set; }

    public static TenantConfiguration Create(string about, string currency, string language, string timeZoneId)
    {
        if (about.Length > AboutMaxLength || about.Length < AboutMinLength)
            throw new ArgumentException($"About length must be greater than {AboutMinLength} or equals. Lower than {AboutMaxLength} or equals.");

        if(currency.Length > CurrencyMaxLength || currency.Length < CurrencyMinLength)
            throw new ArgumentException($"Currency length must be greater than {CurrencyMinLength} or equals. Lower than {CurrencyMaxLength} or equals.");

        if(language.Length > LanguageMaxLength || language.Length < LanguageMinLength)
            throw new ArgumentException($"Language length must be greater than {LanguageMinLength} or equals. Lower than {LanguageMaxLength} or equals.");

        if(timeZoneId.Length > TimeZoneIdMaxLength || timeZoneId.Length < TimeZoneIdMinLength)
            throw new ArgumentException($"TimeZoneId length must be greater than {TimeZoneIdMinLength} or equals. Lower than {TimeZoneIdMaxLength + 1}");

        return new TenantConfiguration(about, currency, language, timeZoneId);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return About;
        yield return Currency;
        yield return Language;
        yield return TimeZoneId;
    }
}