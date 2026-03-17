using System;
using System.ComponentModel.DataAnnotations;
using FCG.Application.Inputs;
using FCG.Application.Validation;
using Xunit;

namespace FCG.Test
{
    public class DtoValidationTests
    {
        [Fact]
        public void UserRegisterDto_Valid_PassesValidation()
        {
            var dto = new UserRegisterDto
            {
                Name = "Fulano",
                Email = "fulano@example.com",
                Address = "Rua A, 123",
                Cpf = "12345678909", // coloque um CPF válido de teste conforme sua regra
                Password = "Abc1234!9"
            };

            Exception? ex = Record.Exception(() => DtoValidator.ValidateObject(dto));
            Assert.Null(ex);
        }

        [Fact]
        public void UserRegisterDto_Invalid_ThrowsValidationException()
        {
            var dto = new UserRegisterDto
            {
                Name = "A",
                Email = "not-an-email",
                Address = "A",
                Cpf = "invalid",
                Password = "short"
            };

            Assert.Throws<ValidationException>(() => DtoValidator.ValidateObject(dto));
        }

        [Fact]
        public void LoginDto_InvalidEmail_ThrowsValidationException()
        {
            var dto = new LoginDto
            {
                Email = "invalid-email",
                Password = "any"
            };

            Assert.Throws<ValidationException>(() => DtoValidator.ValidateObject(dto));
        }
    }
}