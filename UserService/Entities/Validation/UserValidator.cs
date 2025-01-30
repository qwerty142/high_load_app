using FluentValidation;

namespace UserService.Entities.Validation;

public class UserValidator : AbstractValidator<IUser>
{
    public UserValidator()
    {
        RuleFor(user => user.Login).NotNull().NotEmpty();
        RuleFor(user => user.Password).NotNull().NotEmpty();
        RuleFor(user => user.Name).NotNull().NotEmpty();
        RuleFor(user => user.Surname).NotNull().NotEmpty();
        RuleFor(user => user.Age).NotNull().GreaterThan(0).LessThanOrEqualTo(99);
    }
}