using VC.Tenants.Application.Contracts;
using VC.Tenants.Entities;

namespace VC.Tenants.Infrastructure.Implementations;

internal class ByDateCodeGenerator : IEmailVerifyCodeGenerator
{
    public string GenerateCode()
    {
        int hashCode = DateTime.UtcNow.GetHashCode();
        string stringHashCode = hashCode > 0 ? hashCode.ToString() : (-hashCode).ToString();

        if(stringHashCode.Length > EmailVerification.CodeMaxLenght)
            stringHashCode = stringHashCode.Substring(0, EmailVerification.CodeMaxLenght - 1);

        return stringHashCode;
    }
}