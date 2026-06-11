# Todo API

REST API construída com .NET 9 e ASP.NET Core MVC para gerenciamento de tarefas (to-do items). Inclui autenticação JWT, cache em memória, health check e tratamento global de erros.

## Stack

- **.NET 9** / ASP.NET Core MVC
- **Entity Framework Core** com banco InMemory
- **JWT Bearer** para autenticação e autorização
- **IMemoryCache** para cache de leitura
- **Swashbuckle** (Swagger UI)
- **xUnit** para testes unitários

## Estrutura do Projeto

```
TodoApi/
├── TodoApi/
│   ├── Controllers/
│   │   ├── AuthController.cs          # POST /api/auth/login
│   │   ├── TodoItemsController.cs     # CRUD /api/todoitems
│   │   └── HealthCheckEndpoints.cs    # GET /health
│   ├── Models/
│   │   ├── TodoItem.cs                # Entidade do banco
│   │   ├── TodoItemDTO.cs             # DTO de entrada/saída
│   │   └── Login.cs                  # Modelo de login
│   ├── Context/
│   │   └── AppDbContext.cs            # DbContext (EF Core)
│   └── Program.cs                     # Configuração da aplicação
└── TodoApi.Tests/
    └── Controllers/
        ├── TodoItemsControllerTests.cs
        └── AuthControllerTests.cs
```

## Endpoints

### Autenticação

| Método | Rota              | Descrição              | Auth |
|--------|-------------------|------------------------|------|
| POST   | /api/auth/login   | Gera token JWT         | ❌   |

**Body:**
```json
{
  "username": "admin",
  "password": "senhaforte"
}
```

**Resposta:**
```json
{
  "token": "<jwt>"
}
```

### Todo Items

| Método | Rota                  | Descrição                  | Auth          |
|--------|-----------------------|----------------------------|---------------|
| GET    | /api/todoitems        | Lista todos os itens       | ❌            |
| GET    | /api/todoitems/{id}   | Retorna um item por ID     | ❌            |
| POST   | /api/todoitems        | Cria um novo item          | ❌            |
| PUT    | /api/todoitems/{id}   | Atualiza um item           | ❌            |
| DELETE | /api/todoitems/{id}   | Remove um item             | ✅ Admin + CanDelete |

**Body (POST/PUT):**
```json
{
  "id": 0,
  "name": "Comprar leite",
  "isComplete": false
}
```

### Health Check

| Método | Rota     | Descrição                        |
|--------|----------|----------------------------------|
| GET    | /health  | Status da API e suas dependências |

**Resposta (200 OK):**
```json
{
  "status": "Healthy",
  "duration": "00:00:00.012",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.005"
    }
  ]
}
```

Retorna `503 Service Unavailable` quando o status não for `Healthy`.

## Autenticação e Autorização

A API usa **JWT Bearer**. Para acessar endpoints protegidos:

1. Faça POST em `/api/auth/login` com as credenciais.
2. Copie o `token` da resposta.
3. Inclua no header das requisições:
   ```
   Authorization: Bearer <token>
   ```

O endpoint `DELETE /api/todoitems/{id}` exige:
- Role `Admin`
- Claim `Permission: Delete`

Ambas são incluídas automaticamente no token gerado pelo login de `admin`.

## Cache

Leituras são armazenadas em `IMemoryCache` com:
- **Expiração absoluta:** 5 minutos
- **Expiração por sliding:** 2 minutos

O cache é invalidado automaticamente nas operações de escrita (POST, PUT, DELETE).

## Como Rodar

**Pré-requisitos:** .NET 9 SDK

```bash
# Clonar o repositório
git clone https://github.com/seu-usuario/todo-api-mvc-net-claude.git
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
    "Key": "sua_chave_secreta_aqui",
    "Issuer": "TodoApi",
    "Audience": "TodoApiUsers"
  }
}
```

## Testes

```bash
dotnet test TodoApi/TodoApi.Tests/TodoApi.Tests.csproj
```

Os testes usam banco InMemory isolado por teste e `NullLogger` — sem dependências externas ou mocks de framework.
