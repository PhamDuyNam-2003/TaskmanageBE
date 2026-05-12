using FluentValidation;
using BE.DTOs.Tasks;

namespace BE.Validators.Tasks
{
    public class CreateTaskValidator
        : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề task không được để trống")

                .MaximumLength(200)
                .WithMessage("Tiêu đề tối đa 200 ký tự");


            RuleFor(x => x.AssignedToId)
                .NotEmpty()
                .WithMessage("Người được giao task không hợp lệ");


            RuleFor(x => x.Progress)
                .InclusiveBetween(0, 100)
                .WithMessage("Progress phải từ 0 -> 100");
        }
    }
}