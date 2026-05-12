using FluentValidation;
using BE.DTOs.Auth;

namespace BE.Validators.Auth
{
    public class RegisterValidator
        : AbstractValidator<RegisterRequestDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username không được để trống")

                .MinimumLength(3)
                .WithMessage("Username phải có ít nhất 3 ký tự");


            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email không được để trống")

                .EmailAddress()
                .WithMessage("Email không hợp lệ");


            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Mật khẩu không được để trống")

                .MinimumLength(6)
                .WithMessage("Mật khẩu phải từ 6 ký tự");
        }
    }
}