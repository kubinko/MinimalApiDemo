using FluentValidation;
using MinimalApi.Database;

namespace MinimalApi.Validators
{
    public class AttendeeValidator : AbstractValidator<Attendee>
    {
        public AttendeeValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.YearOfBirth).GreaterThan(1900).LessThanOrEqualTo(DateTimeOffset.Now.Year);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(255).EmailAddress();
            RuleFor(x => x.Phone).MaximumLength(20);
        }
    }
}
