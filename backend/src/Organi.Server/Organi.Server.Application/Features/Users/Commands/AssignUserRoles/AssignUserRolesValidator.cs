using FluentValidation;

namespace Organi.Server.Application.Features.Users.Commands.AssignUserRoles;

public sealed class AssignUserRolesValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesValidator()
    {
        RuleFor(x => x.RoleNames)
            .NotEmpty().WithMessage("At least one role is required.");

        RuleForEach(x => x.RoleNames)
            .NotEmpty().WithMessage("Role name must not be empty.")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");
    }
}
