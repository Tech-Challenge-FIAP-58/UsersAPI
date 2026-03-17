using AutoMapper;
using FCG.Application.Inputs;
using FCG.Application.Mapping;
using FCG.Core.Models;
using Xunit;

namespace FCG.Test
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            _mapper = cfg.CreateMapper();
        }

        [Fact]
        public void UserRegisterDto_MapsTo_User()
        {
            var dto = new UserRegisterDto
            {
                Name = "Nome",
                Email = "email@x.com",
                Address = "End",
                Cpf = "12345678909",
                Password = "Abc123!45"
            };

            var user = _mapper.Map<User>(dto);

            Assert.Equal(dto.Name, user.Name);
            Assert.Equal(dto.Email, user.Email);
            Assert.Equal(dto.Address, user.Address);
            Assert.Equal(dto.Cpf, user.Cpf);
            Assert.False(user.IsAdmin);
        }

        [Fact]
        public void UserUpdateDto_MapTo_User_DoesNotOverwriteCpfOrPasswordWhenNull()
        {
            var user = new User
            {
                Name = "Old",
                Email = "old@x.com",
                Address = "OldAddr",
                Cpf = "11122233344",
                Password = "oldhash",
                IsAdmin = false
            };

            var update = new UserUpdateDto
            {
                Name = "NewName",
                Email = null,
                Address = null,
                Password = null
            };

            _mapper.Map(update, user);

            Assert.Equal("NewName", user.Name);
            Assert.Equal("11122233344", user.Cpf);      // não alterado
            Assert.Equal("oldhash", user.Password);     // não alterado
        }
    }
}