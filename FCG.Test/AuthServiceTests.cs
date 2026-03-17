using AutoMapper;
using FCG.Application.Auth;
using FCG.Application.Inputs;
using FCG.Application.Producer;
using FCG.Core.Interfaces.Repository;
using FCG.Core.Interfaces.Utils;
using FCG.Core.Messages.Integration;
using FCG.Core.Models;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Test
{
    public class AuthServiceTests
    {
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<IAuthRepository> _repo = new();
        private readonly Mock<IConfiguration> _config = new();

        // mocks usados apenas para construir o UserProducer real e verificar chamadas ao IBus
        private readonly Mock<IBus> _busMock = new();
        private readonly Mock<ILogger<UserProducer>> _producerLoggerMock = new();

        private readonly UserProducer _producer;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _producer = new UserProducer(_producerLoggerMock.Object, _busMock.Object);
            _service = new AuthService(_mapper.Object, _hasher.Object, _repo.Object, _config.Object, _producer);
        }

        [Fact]
        public async Task Login_InvalidDto_ReturnsBadRequest()
        {
            var dto = new LoginDto { Email = "invalid", Password = "x" }; // invalid email
            var response = await _service.Login(dto);
            Assert.False(response.IsSuccess);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsUnauthorized()
        {
            var dto = new LoginDto { Email = "a@b.com", Password = "x" };
            _repo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            var response = await _service.Login(dto);

            Assert.False(response.IsSuccess);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var dto = new LoginDto { Email = "a@b.com", Password = "x" };
            var user = new User { Id = 1, Email = dto.Email, Password = "hash", Name = "N", Cpf = "123", Address = "A", IsAdmin = false };
            _repo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify(dto.Password, user.Password)).Returns(false);

            var response = await _service.Login(dto);

            Assert.False(response.IsSuccess);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_Valid_ReturnsToken()
        {
            var dto = new LoginDto { Email = "a@b.com", Password = "x" };
            var user = new User { Id = 1, Email = dto.Email, Password = "hash", Name = "N", Cpf = "123", Address = "A", IsAdmin = false };
            _repo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify(dto.Password, user.Password)).Returns(true);
            _repo.Setup(r => r.GenerateToken(It.IsAny<IConfiguration>(), user)).Returns("my-token");

            var response = await _service.Login(dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("my-token", response.ResultValue);
        }

        [Fact]
        public async Task Register_WhenEmailExists_ReturnsConflict()
        {
            var dto = new UserRegisterDto
            {
                Name = "Nome Completo",
                Email = "teste@email.com",
                Address = "Rua Teste 123",
                Cpf = "12345678909",
                Password = "Abc123!45"
            };
            _repo.Setup(r => r.ExistsByEmailAsync(dto.Email)).ReturnsAsync(true);

            var response = await _service.Register(dto);

            Assert.False(response.IsSuccess);
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);

            // Garante que o bus NÃO foi publicado quando há conflito de email
            _busMock.Verify(b => b.Publish(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Register_Success_CallsCreateAndPublishesEvent()
        {
            var dto = new UserRegisterDto
            {
                Name = "Nome Completo",
                Email = "teste@email.com",
                Address = "Rua Teste 123",
                Cpf = "12345678909",
                Password = "Abc123!45"
            };
            var mapped = new User { Email = dto.Email, Name = dto.Name, Cpf = dto.Cpf, Address = dto.Address, Password = "Abc123!45", IsAdmin = false };

            _repo.Setup(r => r.ExistsByEmailAsync(dto.Email)).ReturnsAsync(false);
            _repo.Setup(r => r.ExistsByCpfAsync(dto.Cpf)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<User>(dto)).Returns(mapped);
            _hasher.Setup(h => h.Hash(dto.Password)).Returns("hashed");
            _repo.Setup(r => r.Create(mapped)).ReturnsAsync(42);

            // Configura o IBus para aceitar Publish
            _busMock.Setup(b => b.Publish(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

            var response = await _service.Register(dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            _hasher.Verify(h => h.Hash(dto.Password), Times.Once);
            _repo.Verify(r => r.Create(mapped), Times.Once);

            _busMock.Verify(b => b.Publish(It.Is<UserCreatedEvent>(e => e.UserId == 42 && e.Email == dto.Email), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}