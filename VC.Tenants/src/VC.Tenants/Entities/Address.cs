
namespace VC.Tenants.Entities;

public class Address : ValueObject
{
    public const int CountryMinLength = 5;
    public const int CountryMaxLength = 32;

    public const int CityMinLength = 4;
    public const int CityMaxLength = 32;

    public const int StreetMinLength = 5;
    public const int StreetMaxLength = 32;

    public const int HouseMinNum = 0;
    public const int HouseMaxNum = 9999;

    private Address(string country, string city, string street, int house)
    {
        Country = country;
        City = city;
        Street = street;
        House = house;
    }

    private Address() { }

    public string Country { get; private set; }

    public string City { get; private set; }

    public string Street { get; private set; }

    public int House { get; private set; }

    public static Address Create(string country, string city, string street, int house)
    {
        if (country.Length > CountryMaxLength || country.Length < CountryMinLength)
            throw new ArgumentException($"Country length {country.Length} must be greater than {CountryMinLength} or equals. Lowest than {CountryMaxLength}");

        if (city.Length > CityMaxLength || city.Length < CityMinLength)
            throw new ArgumentException($"City length {city.Length} must be greater than {CityMinLength} or equals. Lowest than {CityMaxLength} or equals.");

        if(street.Length > StreetMaxLength || street.Length < StreetMinLength)
            throw new ArgumentException($"Street length {street.Length} must be greater than {StreetMinLength} or equals. Lowest than {StreetMaxLength} or equals.");

        if(house > HouseMaxNum || house < HouseMinNum)
            throw new ArgumentException($"House number {house} must be greater than {HouseMinNum} or equals. Lowest than {HouseMaxNum} or equals.");

        return new Address(country, city, street, house);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Street;
        yield return House;
    }
}