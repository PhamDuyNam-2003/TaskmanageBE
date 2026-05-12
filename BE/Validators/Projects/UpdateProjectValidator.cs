using FluentValidation;
using BE.DTOs.Projects;

namespace BE.Validators.Projects
{
    public class UpdateProjectValidator
        : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Tên project không được để trống")

                .MaximumLength(100)
                .WithMessage("Tên project tối đa 100 ký tự");
        }
    }
}