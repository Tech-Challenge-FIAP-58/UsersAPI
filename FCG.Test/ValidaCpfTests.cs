using System.ComponentModel.DataAnnotations;
using FCG.Application.Validation;
using Xunit;

namespace FCG.Test
{
    public class ValidaCpfTests
    {
        [Theory]
        [InlineData("11111111111", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        // Substitua o CPF abaixo por um CPF válido conforme sua implementação de teste
        [InlineData("12345678909", true)]
        public void IsCpf_VariousCases_ReturnsExpected(string? cpf, bool expected)
        {
            var actual = ValidaCpf.IsCpf(cpf);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ValidarCpf_ReturnsValidationResult()
        {
            var valid = ValidaCpf.ValidarCpf("12345678909", null);
            Assert.Equal(ValidationResult.Success, valid);

            var invalid = ValidaCpf.ValidarCpf("invalid", null);
            Assert.NotNull(invalid);
            Assert.Equal("CPF inválido.", invalid?.ErrorMessage);
        }
    }
}