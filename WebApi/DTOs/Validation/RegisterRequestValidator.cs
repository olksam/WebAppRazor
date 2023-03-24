using System.Text.RegularExpressions;

using FluentValidation;
using WebApi.DTOs.Auth;

namespace WebApi.DTOs.Validation {

    public static class SharedValidators {
        public static bool BeValidPassword(string password) {
            return new Regex(@"\d").IsMatch(password)
                && new Regex(@"[a-z]").IsMatch(password)
                && new Regex(@"[A-Z]").IsMatch(password)
                && new Regex(@"[\.,';`]").IsMatch(password);
        }
    }

    public static class ValidationRulesExtensions {
        public static IRuleBuilderOptions<T, string> Password<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            bool mustContainLowerCase = true,
            bool mustContainUpperCase = true,
            bool mustContainDigit = true,
            bool mustContainNonAlphaNumeric = true) {

            IRuleBuilderOptions<T, string> options = null;

            if (mustContainLowerCase) {
                options = ruleBuilder.Must(password => new Regex(@"[a-z]").IsMatch(password));
            }
            if (mustContainUpperCase) {
                options = ruleBuilder.Must(password => new Regex(@"[A-Z]").IsMatch(password));
            }
            if (mustContainDigit) {

            }

            return options;
        }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest> {
        public RegisterRequestValidator()
        {
            RuleFor(e => e.Email).EmailAddress().NotEmpty();
           // RuleFor(e => e.Password).Password(mustContainDigit: false).NotEmpty();
            RuleFor(e => e.Password).Must(SharedValidators.BeValidPassword).NotEmpty();
        }

        //private bool BeValidPassword(string password) {
        //    return new Regex(@"\d").IsMatch(password)
        //        && new Regex(@"[a-z]").IsMatch(password)
        //        && new Regex(@"[A-Z]").IsMatch(password)
        //        && new Regex(@"[\.,';`]").IsMatch(password);
        //} 
    }


}
