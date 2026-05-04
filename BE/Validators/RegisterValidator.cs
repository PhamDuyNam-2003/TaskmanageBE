using FluentValidation;
using static BE.DTOs.AuthDto;

namespace BE.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator() {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username không được để trống")
                .MinimumLength(3).WithMessage("Username phải có ít nhất 3 ký tự");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải từ 6 ký tự trở lên");
        }

    }
}
