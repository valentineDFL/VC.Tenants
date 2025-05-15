using VC.Shared.MailkitIntegration;
using VC.Tenants.Application.Contracts;

namespace VC.Tenants.Application;

internal class TenantEmailVerifyMessagesFactory : ITenantEmailVerificationMessagesFactory
{
    public Message CreateAfterRegistration(string code, string receiverName, string receiverMail)
    {
        var subject = "Регистрация на сайте";
        var header = "Спасибо за регистрацию на сайте!";

        var text = $"Вы зарегистрировались на нашем сайте, код подтверждения почты: {code}";

        return new Message(subject, text, receiverName, receiverMail, header);
    }

    public Message CreateMessageForVerify(string code, string receiverName, string receiverMail)
    {
        var subject = "Подтверждение почты";
        var header = "Вы не подтвердили почту!";

        var text = $"Код подтверждения почты: {code}";

        return new Message(subject, text, receiverName, receiverMail, header);
    }
}