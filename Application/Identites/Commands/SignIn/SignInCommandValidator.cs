using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Identites.Commands.SignIn
{
    public class SignInCommandValidator : AbstractValidator<SignInCommand>
    {
        public SignInCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Tên đăng nhập là bắt buộc.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Mật khẩu là bắt buộc.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Vai trò là bắt buộc.");
        }
    }
}