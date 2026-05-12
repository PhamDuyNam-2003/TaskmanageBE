using FluentValidation;
using BE.DTOs.Auth;

namespace BE.Validators.Auth
{
    public class LoginValidator
        : AbstractValidator<LoginRequestDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Username hoặc Email không được để trống");


            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Mật khẩu không được để trống");
        }
    }
}