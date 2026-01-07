# UserService

## Visão Geral

O **UserService** é um microsserviço responsável pelo gerenciamento de usuários e autenticação, desenvolvido em **ASP.NET 8** seguindo princípios de **Clean Architecture**, **DDD** e **separação de responsabilidades**. O serviço foi projetado para ser facilmente escalável, testável e pronto para execução em ambientes locais, Docker e orquestradores como Kubernetes.

---

## Arquitetura

O projeto está organizado em camadas bem definidas:

```
src/
 ├── UserService.Core          # Domínio (entidades, regras de negócio, exceções)
 ├── UserService.Application   # Casos de uso, serviços de aplicação, contratos
 ├── UserService.Infra         # Persistência, integrações externas, implementações
 └── UserService.WebApi        # API HTTP, controllers, middlewares e configuração
```

### Responsabilidades por Camada

* **Core**
  * Contém o coração do domínio: entidades, agregados, regras de negócio e validações. Não depende de nenhuma outra camada.

* **Application**
  * Implementa os casos de uso do sistema. Orquestra o domínio e define contratos (interfaces) para infraestrutura e serviços externos.

* **Infra**
  * Implementa detalhes técnicos como acesso a dados, autenticação, hashing, persistência e integrações.

* **WebApi**
  * Exposição HTTP do serviço. Contém controllers, middlewares, configuração de DI, autenticação e pipeline da aplicação.

### Visibilidade entre Camadas


---

## Tecnologias Utilizadas

* **.NET 8**
* **ASP.NET Core Web API**
* **JWT para autenticação**
* **Docker / Dockerfile multi-stage**
* **SQL Server**
* **Clean Architecture / DDD**

---

## Configuração

### Arquivos de Configuração

Os principais arquivos de configuração estão em:

* `appsettings.json`
* `appsettings.Development.json`

Exemplo de configuração de conexão presente em `appsettings.Development.example.json`

> ⚠️ **Nunca versionar secrets reais**. Utilize variáveis de ambiente em produção.

---

## Execução Local

### Pré-requisitos

* .NET SDK 8+
* Docker (opcional)
* Banco de dados configurado

### Executar via CLI

```bash
dotnet restore
dotnet build
dotnet run --project src/UserService.WebApi
```

A API estará disponível em:

```
https://localhost:5001
http://localhost:5000
```

---

## Docker

O projeto possui um **Dockerfile multi-stage** localizado em:

```
src/UserService.WebApi/Dockerfile
```

### Build da imagem

```bash
docker build -t userservice -f src/UserService.WebApi/Dockerfile .
```

### Executar o container

```bash
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="..." \
  -e Jwt__SecretKey="..." \
  userservice
```

---

## Kubernetes

🏗Em andamento...

---

## Endpoints

### Autenticação

* `POST api/Auth/Login`
* `POST api/Auth/Register`
* 🔐 `POST api/Auth/RegisterAdmin`

### Usuários

* 🔐 `GET api/User/GetById/{id}`
* 🔐 `GET api/User/GetAll`
* 🔐 `PUT api/User/Update/{id}`
* 🔐 `DELETE api/User/Delete/{id}`

> Endpoints protegidos exigem **Bearer Token (JWT)**.

---

## Segurança

* Autenticação baseada em **JWT**
* Hash de senha
* Separação clara entre domínio e infraestrutura
* Middlewares dedicados para tratamento de erros

---

## Boas Práticas Adotadas

* Clean Architecture
* Inversão de Dependência
* Domínio isolado
* Configuração por ambiente
* Containers prontos para CI/CD

---

## Observações Finais

Este microsserviço foi estruturado para ser reutilizável em um ecossistema de microsserviços, facilitando evolução, testes e manutenção. Toda regra de negócio relevante encontra-se protegida no domínio, mantendo a API fina e coesa.

---
