using FluentValidation;
using VC.Tenants.Api.Models.Request.Create;
using VC.Tenants.Entities;

namespace VC.Tenants.Api.Validation;

internal class CreateTenantValidation : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantValidation()
    {
        RuleFor(ctr => ctr)
            .NotNull();

        RuleFor(ctr => ctr.Name)
            .MinimumLength(Tenant.NameMinLenght)
            .MaximumLength(Tenant.NameMaxLenght)
            .NotEmpty();

        RuleFor(ctr => ctr.Config)
            .NotNull();

        RuleFor(ctr => ctr.Config)
            .ChildRules(tcd =>
            {
                tcd.RuleFor(tcd => tcd.About)
                    .MinimumLength(TenantConfiguration.AboutMinLength)
                    .MaximumLength(TenantConfiguration.AboutMaxLength)
                    .NotEmpty();

                tcd.RuleFor(tcd => tcd.Currency)
                    .MinimumLength(TenantConfiguration.CurrencyMinLength)
                    .MaximumLength(TenantConfiguration.CurrencyMaxLength)
                    .NotEmpty();

                tcd.RuleFor(tcd => tcd.Language)
                    .MinimumLength(TenantConfiguration.LanguageMinLength)
                    .MaximumLength(TenantConfiguration.LanguageMaxLength)
                    .NotEmpty();

                tcd.RuleFor(tcd => tcd.TimeZoneId)
                    .MinimumLength(TenantConfiguration.TimeZoneIdMinLength)
                    .MaximumLength(TenantConfiguration.TimeZoneIdMaxLength)
                    .NotEmpty();
            });

        RuleFor(ctr => ctr.Status)
            .Must(ctr => ctr != TenantStatus.None)
            .IsInEnum();

        RuleFor(ctr => ctr.ContactInfo)
            .ChildRules(con =>
            {
                con.RuleFor(ctr => ctr.Phone)
                .MinimumLength(ContactInfo.PhoneNumberMinLength)
                .MaximumLength(ContactInfo.PhoneNumberMaxLength)
                .NotEmpty();

                con.RuleFor(ctr => ctr.AddressDto)
                .NotNull()
                .ChildRules(add =>
                {
                    add.RuleFor(tn => tn.Country)
                    .MinimumLength(Address.CountryMinLength)
                    .MaximumLength(Address.CountryMaxLength)
                    .NotEmpty();

                    add.RuleFor(tn => tn.City)
                    .MinimumLength(Address.CityMinLength)
                    .MaximumLength(Address.CityMaxLength)
                    .NotEmpty();

                    add.RuleFor(tn => tn.Street)
                    .MinimumLength(Address.StreetMinLength)
                    .MaximumLength(Address.StreetMaxLength)
                    .NotEmpty();

                    add.RuleFor(tn => tn.House)
                    .Must(tn => tn >= Address.HouseMinNum && tn <= Address.HouseMaxNum);
                });

                con.RuleFor(ctr => ctr.EmailAddressDto)
                .ChildRules(ead =>
                {
                    ead.RuleFor(em => em.Email)
                    .NotEmpty()
                    .MaximumLength(EmailAddress.EmailAddressMaxLength)
                    .EmailAddress();
                });
            });

        RuleFor(ctr => ctr.WorkSchedule)
            .NotNull()
            .ChildRules(wc =>
            {
                wc.RuleFor(wc => wc.WeekSchedule)
                .NotNull()
                .Must(wk => wk.Count == Enum.GetValues(typeof(DayOfWeek)).Length)
                .Must(wk => wk.DistinctBy(wd => wd.Day).Count() == wk.Count)
                .Must(wk => wk.All(x => x.StartWork != x.EndWork && x.StartWork < x.EndWork))
                .Must(wk => wk.All(t => t.StartWork.Kind == DateTimeKind.Utc && t.EndWork.Kind == DateTimeKind.Utc))
                .WithMessage("Time Must be in UTC format");
            });
    }
}