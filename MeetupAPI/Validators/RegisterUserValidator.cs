using FluentValidation;
using MeetupAPI.Entities;
using MeetupAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MeetupAPI.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        //konfiguracja dla FluentValidation
        public RegisterUserValidator(MeetupContext meetupContext)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);

            RuleFor(x => x.Email).Custom((value, context) =>
              {
                  var userAlreadyExist = meetupContext.Users.Any(u => u.Email == value);
                  if (userAlreadyExist)
                  {
                      context.AddFailure("Email", "The email adress is taken");
                  }
              });

        }
    }
}
