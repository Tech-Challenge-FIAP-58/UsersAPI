using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FCG.Application.Inputs;
using FCG.Application.Services;
using FCG.Application.Web;
using FCG.Core.Interfaces.Repository;
using FCG.Core.Interfaces.Utils;
using FCG.Core.Models;
using Moq;
using Xunit;

namespace FCG.Test
{
    public class UserApplicationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly UserApplicationService _service;

        public UserApplicationServiceTests()
        {
            _service = new UserApplicationService(_passwordHasherMock.Object, _userRepositoryMock.Object, _mapperMock.Object);
        }

        private static User CreateUser(int id = 1)
            => new()
            {
                Id = id,
                Name = "User Name",
                Email = "user@example.com",
                Password = "oldhash",
                Cpf = "12345678901",
                Address = "Address",
                IsAdmin = false
            };

        [Fact]
        public async Task GetAll_ReturnsMappedUsersAndOk()
        {
            var users = new[] { CreateUser() };
            var mapped = new[] { new UserResponseDto(users[0].Id, users[0].Name, users[0].Email, users[0].Address, users[0].Cpf, users[0].CreatedAtUtc) };

            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(users.AsEnumerable());
            _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(It.IsAny<IEnumerable<User>>())).Returns(mapped);

            var response = await _service.GetAll();

            Assert.True(response.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.ResultValue);
            Assert.Equal(mapped, response.ResultValue);
        }

        [Fact]
        public async Task GetById_WhenFound_ReturnsOkWithDto()
        {
            var user = CreateUser(5);
            var dto = new UserResponseDto(user.Id, user.Name, user.Email, user.Address, user.Cpf, user.CreatedAtUtc);

            _userRepositoryMock.Setup(r => r.GetById(user.Id)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserResponseDto>(It.IsAny<User>())).Returns(dto);

            var response = await _service.GetById(user.Id);

            Assert.True(response.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.ResultValue);
            Assert.Equal(dto, response.ResultValue);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((User?)null);

            var response = await _service.GetById(10);

            Assert.False(response.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Null(response.ResultValue);
            Assert.Contains("não encontrado", response.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Update_WhenUserExists_WithPasswordProvided_HashesAndUpdates()
        {
            var user = CreateUser(2);
            User? capturedUser = null;

            var dto = new UserUpdateDto
            {
                Name = "Updated Name",
                Password = "P@ssw0rd123"
            };

            _userRepositoryMock.Setup(r => r.GetById(user.Id)).ReturnsAsync(user);

            // mapper.Map(dto, user) is expected to be called; allow any mapping call (we don't depend on AutoMapper behavior here)
            _mapperMock.Setup(m => m.Map(It.IsAny<UserUpdateDto>(), It.IsAny<User>()))
                       .Callback<UserUpdateDto, User>((s, d) =>
                       {
                           // mimic partial map behavior for test: apply Name if provided
                           if (s.Name != null) d.Name = s.Name;
                       });

            _passwordHasherMock.Setup(h => h.Hash(dto.Password)).Returns("hashed-password");

            _userRepositoryMock.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync(true);

            var response = await _service.Update(user.Id, dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.NotNull(capturedUser);
            Assert.Equal("Updated Name", capturedUser!.Name);
            Assert.Equal("hashed-password", capturedUser.Password);

            _mapperMock.Verify(m => m.Map(dto, user), Times.Once);
            _passwordHasherMock.Verify(h => h.Hash(dto.Password), Times.Once);
            _userRepositoryMock.Verify(r => r.Update(user.Id, It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenUserNotFound_ReturnsNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((User?)null);

            var response = await _service.Update(99, new UserUpdateDto { Name = "X" });

            Assert.False(response.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Remove_WhenUserExists_CallsRemoveAndReturnsNoContent()
        {
            var user = CreateUser(3);
            User? removedUser = null;

            _userRepositoryMock.Setup(r => r.GetById(user.Id)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.Remove(It.IsAny<User>()))
                .Callback<User>(u => removedUser = u)
                .ReturnsAsync(true);

            var response = await _service.Remove(user.Id);

            Assert.True(response.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(user, removedUser);

            _userRepositoryMock.Verify(r => r.Remove(user), Times.Once);
        }

        [Fact]
        public async Task Remove_WhenUserNotFound_ReturnsNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((User?)null);

            var response = await _service.Remove(404);

            Assert.False(response.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}