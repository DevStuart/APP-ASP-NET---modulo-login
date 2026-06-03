# AutenticacaoJWT — Módulo de Login 100%

**Copyright © Elias Ferreira. Todos os direitos reservados.**

API ASP.NET Core 8 com JWT, refresh token, admin seed, painel web, Docker, testes e CI.

> Autor e titular: **Elias Ferreira** — ver [LICENSE](LICENSE).

## Recursos

- Login, cadastro, refresh token, logout
- `GET /api/Login/me` — usuário autenticado
- CRUD de usuários com policy **AdminOnly**
- Hash PBKDF2 (HMAC-SHA256), e-mail único no banco
- FluentValidation (senha forte no cadastro)
- Rate limiting em rotas de auth
- Health check: `GET /health`
- Seed automático de admin (Development)
- Painel: `/modulo-login/`
- Docker Compose (API + PostgreSQL)
- Testes com xUnit + `WebApplicationFactory`

---

## Pré-requisitos

| Ferramenta | Versão sugerida | Verificar |
|------------|-----------------|-----------|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0+ | `dotnet --version` |
| [PostgreSQL](https://www.postgresql.org/download/) | 16+ | serviço rodando na porta `5432` |
| [dotnet-ef](https://learn.microsoft.com/ef/core/cli/dotnet) | global | `dotnet ef --version` |
| (Opcional) Docker Desktop | recente | `docker --version` |

---

## Guia completo — do zero até a API no ar

Todos os comandos abaixo assumem a pasta do projeto:

```cmd
cd c:\Users\user\Desktop\autenticacao-jwt-master
```

> **CMD e PowerShell:** os comandos `dotnet` são iguais nos dois.

---

### Passo 1 — Instalar a ferramenta EF (só na primeira vez)

Se `dotnet ef` não for reconhecido:

```cmd
dotnet tool install --global dotnet-ef
```

Atualizar se já existir:

```cmd
dotnet tool update --global dotnet-ef
```

Fechar e abrir o terminal, depois conferir:

```cmd
dotnet ef --version
```

---

### Passo 2 — Configurar connection string

Copie o exemplo e edite usuário/senha do Postgres:

```cmd
copy AutenticacaoJWT.API\appsettings.Example.json AutenticacaoJWT.API\appsettings.Development.json
```

Edite `AutenticacaoJWT.API\appsettings.Development.json`:

```json
"ConnectionStrings": {
  "AutenticacaoJWTDB": "Host=127.0.0.1;Port=5432;Username=postgres;Password=SUA_SENHA;Database=AutenticacaoJWTDB;"
}
```

**Alternativa (User Secrets):**

```cmd
cd AutenticacaoJWT.API
dotnet user-secrets set "ConnectionStrings:AutenticacaoJWTDB" "Host=127.0.0.1;Port=5432;Username=postgres;Password=SUA_SENHA;Database=AutenticacaoJWTDB;"
cd ..
```

---

### Passo 3 — Criar o banco no PostgreSQL

O Entity Framework **pode** criar o banco no primeiro `database update`, mas você também pode criar manualmente.

**Opção A — pgAdmin / Query Tool:**

```sql
CREATE DATABASE "AutenticacaoJWTDB";
```

**Opção B — `psql` (CMD):**

```cmd
psql -U postgres -c "CREATE DATABASE \"AutenticacaoJWTDB\";"
```

---

### Passo 4 — Restaurar pacotes NuGet

Sempre rode antes de build ou migração (evita erro `NETSDK1004`):

```cmd
dotnet restore AutenticacaoJWT.sln
```

---

### Passo 5 — Aplicar migrações (criar/atualizar tabelas)

Cria ou atualiza as tabelas: `Users`, `RefreshTokens`, `__EFMigrationsHistory`, índice único em e-mail, etc.

```cmd
dotnet ef database update --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

Saída esperada: `Done.` e mensagens `Applying migration ...`.

**Se o banco já existia de uma versão antiga**, este comando aplica só as migrações que faltam (ex.: `RefreshTokensAndUniqueEmail`).

---

### Passo 6 — Subir a aplicação (API ON)

Entre na pasta da API (obrigatório — `dotnet run` na raiz da solution **não** funciona):

```cmd
cd AutenticacaoJWT.API
dotnet run
```

**Com perfil HTTPS + Swagger:**

```cmd
dotnet run --launch-profile https
```

**Com perfil que abre o painel de login:**

```cmd
dotnet run --launch-profile ModuloLogin
```

**Parar a API:** `Ctrl + C` no terminal.

---

### Passo 7 — Acessar no navegador

| URL | Descrição |
|-----|-----------|
| http://localhost:5043/modulo-login/ | Painel de login |
| http://localhost:5043/swagger | Documentação Swagger |
| http://localhost:5043/health | Health check (API + PostgreSQL) |
| https://localhost:7060/modulo-login/ | Painel (perfil HTTPS) |

### Admin padrão (criado automaticamente no Development)

| Campo | Valor |
|-------|-------|
| E-mail | `admin@localhost.com` |
| Senha | `Admin@123` |

### Cadastro de usuário comum (senha forte)

- Mínimo **8** caracteres  
- Pelo menos **1 maiúscula**, **1 minúscula** e **1 número**  
- Exemplo: `Senha@123`

---

## Comandos rápidos (cola e usa)

```cmd
cd c:\Users\user\Desktop\autenticacao-jwt-master
dotnet restore AutenticacaoJWT.sln
dotnet ef database update --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
cd AutenticacaoJWT.API
dotnet run
```

---

## Migrações — referência completa

### Aplicar migrações no banco (gerar/atualizar tabelas)

```cmd
cd c:\Users\user\Desktop\autenticacao-jwt-master
dotnet restore AutenticacaoJWT.sln
dotnet ef database update --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

### Criar uma nova migração (após alterar entidades)

```cmd
dotnet ef migrations add NomeDescritivoDaMigration --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
dotnet ef database update --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

Exemplo:

```cmd
dotnet ef migrations add AdicionarCampoTelefone --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
dotnet ef database update --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

### Remover a última migração (ainda não aplicada no banco)

```cmd
dotnet ef migrations remove --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

### Listar migrações

```cmd
dotnet ef migrations list --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

### Gerar script SQL (opcional, para DBA)

```cmd
dotnet ef migrations script --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API -o migrate.sql
```

### Recriar banco do zero (cuidado: apaga dados)

```cmd
dotnet ef database drop --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API --force
dotnet ef database update --project AutenticacaoJWT.Infrastructure --startup-project AutenticacaoJWT.API
```

---

## Build, testes e publicação

### Compilar a solution

```cmd
dotnet build AutenticacaoJWT.sln -c Release
```

### Rodar testes

```cmd
dotnet test AutenticacaoJWT.Tests\AutenticacaoJWT.Tests.csproj
```

### Publicar para deploy

```cmd
dotnet publish AutenticacaoJWT.API\AutenticacaoJWT.API.csproj -c Release -o .\publish
cd publish
dotnet AutenticacaoJWT.API.dll
```

---

## Docker (Postgres + API)

Na raiz do repositório:

```cmd
cd c:\Users\user\Desktop\autenticacao-jwt-master
docker compose up --build
```

| URL | Descrição |
|-----|-----------|
| http://localhost:8080/modulo-login/ | Painel |
| http://localhost:8080/swagger | Swagger |

Parar containers:

```cmd
docker compose down
```

---

## Visual Studio

1. Abrir `AutenticacaoJWT.sln`
2. Definir **AutenticacaoJWT.API** como projeto de inicialização
3. Perfil: **ModuloLogin**, **https** ou **http**
4. **F5** para iniciar

Antes do primeiro F5, no **Package Manager Console**:

```powershell
Update-Database -Project AutenticacaoJWT.Infrastructure -StartupProject AutenticacaoJWT.API
```

---

## Endpoints da API

| Método | Rota | Auth |
|--------|------|------|
| POST | `/api/Login/register` | Não |
| POST | `/api/Login/login` | Não |
| POST | `/api/Login/refresh` | Não |
| POST | `/api/Login/logout` | Sim |
| GET | `/api/Login/me` | Sim |
| GET | `/api/User/Users` | Sim |
| GET | `/api/User/{id}` | Sim |
| PUT | `/api/User` | Admin |
| DELETE | `/api/User/{id}` | Admin |

Header autenticado: `Authorization: Bearer {seu_token}`

### Resposta de autenticação

```json
{
  "token": "eyJ...",
  "refreshToken": "...",
  "expiresAt": "2026-06-02T12:00:00Z",
  "refreshExpiresAt": "2026-06-09T12:00:00Z"
}
```

---

## Integrar em outro projeto

```csharp
builder.Services.AddLoginModule(builder.Configuration);
builder.Services.AddLoginModuleCors(builder.Configuration);

app.UseCors("LoginModule");
app.UseAuthentication();
app.UseAuthorization();
```

---

## Configuração (`appsettings`)

| Chave | Descrição |
|-------|-----------|
| `ConnectionStrings:AutenticacaoJWTDB` | PostgreSQL |
| `jwt:secretKey` | Mínimo 32 caracteres |
| `jwt:accessTokenMinutes` | Padrão 30 |
| `jwt:refreshTokenDays` | Padrão 7 |
| `LoginModule:SeedAdmin:Enabled` | `true` cria admin no startup |
| `LoginModule:SeedAdmin:Email` | E-mail do admin seed |
| `LoginModule:SeedAdmin:Password` | Senha do admin seed |
| `LoginModule:CorsOrigins` | Origens do front |

---

## Problemas comuns

| Erro | Solução |
|------|---------|
| `NETSDK1004` / `project.assets.json` não encontrado | `dotnet restore AutenticacaoJWT.sln` |
| `dotnet-ef` não existe | `dotnet tool install --global dotnet-ef` |
| `Não foi possível localizar um projeto para executar` | `cd AutenticacaoJWT.API` antes do `dotnet run` |
| Falha de conexão PostgreSQL | Serviço ligado, senha correta em `appsettings.Development.json` |
| `database does not exist` | Criar banco (Passo 3) ou rodar `database update` |
| Índice único em e-mail falha | Remover e-mails duplicados na tabela `Users` |
| Login falha com usuário antigo | Recadastrar (hash de senha foi atualizado) |
| Cadastro: senha inválida | Usar 8+ chars com maiúscula, minúscula e número |

---

## Estrutura das tabelas (após migrações)

- **Users** — `Id`, `Name`, `Email` (único), `Password`, `Salt`, `IsAdmin`
- **RefreshTokens** — tokens de renovação (hash, expiração, revogação)
- **__EFMigrationsHistory** — controle de versão do EF Core

---

## Licença e direitos autorais

**Copyright © Elias Ferreira. Todos os direitos reservados.**

Este projeto é propriedade intelectual de **Elias Ferreira**. Consulte o arquivo [LICENSE](LICENSE) para os termos de uso.
