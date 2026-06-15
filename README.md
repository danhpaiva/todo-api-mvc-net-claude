# Todo API

![.NET](https://img.shields.io/badge/.NET_9-512BD4?style=flat&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core_MVC-512BD4?style=flat&logo=dotnet&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=flat&logo=dotnet&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=flat&logo=jsonwebtokens&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=flat&logo=swagger&logoColor=black)
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=flat&logo=dotnet&logoColor=white)

REST API para gerenciamento de tarefas (to-do items) construída com .NET 9 e ASP.NET Core MVC. Serve como template de projeto com arquitetura em camadas, autenticação JWT, cache, paginação, health check e pipeline de qualidade via skills do Claude Code.

## Stack

| Tecnologia | Uso |
|---|---|
| **.NET 9** / ASP.NET Core MVC | Framework principal |
| **Entity Framework Core** (InMemory) | Persistência de dados |
| **JWT Bearer** | Autenticação e autorização |
| **IMemoryCache** | Cache de leitura com expiração |
| **Swashbuckle** (Swagger UI) | Documentação interativa com suporte a JWT |
| **xUnit** + **Moq** | Testes unitários e de integração |

## Arquitetura

O projeto segue uma arquitetura em 3 camadas:

```
Controllers  →  Services  →  Repositories  →  EF Core (AppDbContext)
```

```
TodoApi/
├── TodoApi/
│   ├── Controllers/
│   │   ├── AuthController.cs           # POST /api/auth/login
│   │   ├── TodoItemsController.cs      # CRUD /api/todoitems
│   │   └── HealthCheckEndpoints.cs     # GET /health
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── ITodoService.cs
│   │   │   └── IAuthService.cs
│   │   ├── TodoService.cs              # Lógica de negócio + cache
│   │   └── AuthService.cs             # Geração de token JWT
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   │   └── ITodoRepository.cs
│   │   └── TodoRepository.cs          # Acesso ao banco via EF Core
│   ├── Models/
│   │   ├── TodoItem.cs                # Entidade do banco
│   │   ├── TodoItemDTO.cs             # DTO de entrada/saída
│   │   ├── PagedResult.cs             # Modelo de resposta paginada
│   │   └── Login.cs                   # Modelo de login
│   ├── Context/
│   │   └── AppDbContext.cs            # DbContext (EF Core)
│   └── Program.cs                     # DI, middlewares e configuração
└── TodoApi.Tests/
    ├── Controllers/                    # Testes unitários (mock de serviços)
    ├── Repositories/                   # Testes unitários (banco InMemory)
    └── Integration/                    # Testes de integração (WebApplicationFactory)
```

## Endpoints

### Autenticação

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/auth/login` | Gera token JWT | ❌ |

**Body:**
```json
{
  "username": "admin",
  "password": "senhaforte"
}
```

**Resposta (200 OK):**
```json
{
  "token": "<jwt>"
}
```

### Todo Items

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/api/todoitems` | Lista itens com paginação | ❌ |
| GET | `/api/todoitems/{id}` | Retorna um item por ID | ❌ |
| POST | `/api/todoitems` | Cria um novo item | ❌ |
| PUT | `/api/todoitems/{id}` | Atualiza um item | ❌ |
| DELETE | `/api/todoitems/{id}` | Remove um item | ✅ Admin + CanDelete |

**Body (POST/PUT):**
```json
{
  "id": 0,
  "name": "Comprar leite",
  "isComplete": false
}
```

**Resposta do GET lista (paginada):**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**Query params:** `?page=1&pageSize=10` (pageSize máximo: 100)

### Health Check

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/health` | Status da API e dependências |

**Resposta (200 OK):**
```json
{
  "status": "Healthy",
  "duration": "00:00:00.012",
  "checks": [
    { "name": "database", "status": "Healthy", "duration": "00:00:00.005" }
  ]
}
```

Retorna `503 Service Unavailable` quando o status não for `Healthy`.

## Autenticação e Autorização

A API usa **JWT Bearer**. Para acessar endpoints protegidos:

1. `POST /api/auth/login` com as credenciais configuradas em `appsettings.json`
2. Copie o `token` da resposta
3. Inclua no header: `Authorization: Bearer <token>`

O endpoint `DELETE /api/todoitems/{id}` exige:
- Role `Admin`
- Claim `Permission: Delete`

Ambas são incluídas automaticamente no token gerado pelo login padrão.

No Swagger UI, clique no botão **Authorize** (cadeado) e informe `Bearer <token>` para testar endpoints protegidos diretamente pela interface.

## Cache

Leituras são armazenadas em `IMemoryCache` com:
- **Expiração absoluta:** 5 minutos
- **Expiração sliding:** 2 minutos

O cache é invalidado automaticamente em toda operação de escrita (POST, PUT, DELETE).

## Como Rodar

**Pré-requisitos:** .NET 9 SDK

```bash
# Clonar o repositório
git clone https://github.com/danhpaiva/todo-api-mvc-net-claude.git
cd todo-api-mvc-net-claude

# Rodar a API
dotnet run --project TodoApi/TodoApi

# Acessar o Swagger
# http://localhost:<porta>/swagger
```

## Configuração (appsettings.json)

```json
{
  "Jwt": {
    "Key": "sua_chave_secreta_aqui_minimo_32_chars",
    "Issuer": "TodoApi",
    "Audience": "TodoApiUsers"
  },
  "Auth": {
    "Username": "admin",
    "Password": "sua_senha_aqui"
  }
}
```

> **Atenção:** Em produção, use variáveis de ambiente ou `dotnet user-secrets` para `Jwt:Key`, `Auth:Username` e `Auth:Password`. Nunca commite valores reais dessas chaves.

## Testes

```bash
dotnet test TodoApi/TodoApi.Tests/TodoApi.Tests.csproj
```

| Tipo | Localização | Descrição |
|------|-------------|-----------|
| Unitários — controllers | `TodoApi.Tests/Controllers/` | Mockam `I*Service` via Moq; sem banco nem HTTP |
| Unitários — repositórios | `TodoApi.Tests/Repositories/` | Banco InMemory isolado por teste |
| Integração | `TodoApi.Tests/Integration/` | Pipeline HTTP completo via `WebApplicationFactory` |

## Skills do Claude Code

Este projeto inclui um conjunto de skills em `.claude/skills/` para uso com o Claude Code. Cada skill automatiza uma tarefa recorrente do ciclo de desenvolvimento:

| Skill | Comando | O que faz |
|-------|---------|-----------|
| `new-feature` | `/new-feature <Nome>` | Cria entidade, DTO, DbSet e controller CRUD completo |
| `new-entity` | `/new-entity <Nome>` | Cria entidade + DTO em `Models/` e registra no DbContext |
| `new-endpoint` | `/new-endpoint <Controller> <Verbo> <Ação>` | Adiciona um endpoint a um controller existente |
| `generate-tests` | `/generate-tests <Controller>` | Gera testes unitários cobrindo todos os endpoints |
| `check-architecture` | `/check-architecture` | Verifica violações de arquitetura em camadas |
| `review-code` | `/review-code` | Revisa o diff atual com foco nos padrões do projeto |
| `check-security` | `/check-security` | Analisa o diff em busca de vulnerabilidades |
| `safe-commit` | `/safe-commit` | Pipeline completo: arquitetura + revisão + segurança + testes + commit + push |
| `describe-commit` | `/describe-commit` | Gera mensagem de commit padronizada (Conventional Commits) e faz push |
| `describe-pr` | `/describe-pr` | Gera descrição completa para pull request |
| `update-readme` | `/update-readme` | Atualiza o README com base nas mudanças recentes |
| `setup-project` | `/setup-project <Nome>` | Guia a configuração inicial ao usar este projeto como template |

### Usando como template

Para iniciar um novo projeto a partir deste template:

```bash
git clone https://github.com/danhpaiva/todo-api-mvc-net-claude.git meu-projeto
cd meu-projeto
```

Em seguida, abra o Claude Code e execute `/setup-project <NomeDoProjeto>` — a skill irá guiar os passos de renomeação, configuração do JWT, troca do banco e verificação inicial.

---

Desenvolvido por **Daniel Paiva** — [linkedin.com/in/danhpaiva](https://www.linkedin.com/in/danhpaiva/)
