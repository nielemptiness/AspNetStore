using FluentValidation;

namespace Store.Core.Common
{
    public static class NameValidator
    {
        private static readonly string Message = "Name can not be empty!";
       
        public static IRuleBuilderOptions<T, string> ValidateName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Name can not be empty!");
        }
    }
}