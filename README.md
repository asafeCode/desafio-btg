# Desafio BTG - API de Limite PIX

API REST em .NET 8 para cadastro de contas, consulta, atualizacao de limite PIX, exclusao e processamento de transacoes PIX com controle atomico de saldo em DynamoDB.

## Visao Geral

Este projeto esta organizado em camadas:

- `DesafioBtg.API`: camada HTTP (controllers, filtros, swagger)
- `DesafioBtg.Application`: casos de uso (`UseCases`) e validacoes (`FluentValidation`)
- `DesafioBtg.Domain`: entidades, DTOs e contratos de dominio
- `DesafioBtg.Infrastructure`: repositorios e acesso ao DynamoDB
- `DesafioBtg.IoC`: injecao de dependencias
- `AppHost` (Aspire): orquestracao local de API + DynamoDB Local + DynamoDB Admin

## Stack Tecnologica

- .NET 8
- ASP.NET Core Web API
- AWS DynamoDB (DynamoDB Local em desenvolvimento)
- FluentValidation
- Swagger / OpenAPI
- .NET Aspire AppHost

## Pre-requisitos

### Obrigatorios

- .NET SDK 8 (`global.json` fixa `8.0.0` com `rollForward: latestMinor`)
- Docker Desktop (para DynamoDB Local)

### Opcionais

- .NET Aspire workload (para subir via `AppHost`)

Instalacao do workload Aspire (opcional):

```bash
dotnet workload install aspire
```

## Estrutura de Pastas

```text
src/
  appHost/
    AppHost/
  backend/
    DesafioBtg.API/
    DesafioBtg.Application/
    DesafioBtg.Domain/
    DesafioBtg.Infrastructure/
    DesafioBtg.IoC/
```

## Configuracao de Ambiente

A API depende da variavel abaixo:

- `AWS_ENDPOINT_URL_DYNAMODB`: endpoint HTTP do DynamoDB

Exemplo local:

- `http://localhost:8000`

Observacoes:

- Em desenvolvimento, a API executa uma migracao que garante a criacao da tabela `users`.
- As credenciais AWS sao fixadas como `test/test` no IoC para ambiente local.

## Como Rodar

## 1) Opcao recomendada: via Aspire AppHost

Esse modo sobe:

- DynamoDB Local
- DynamoDB Admin (`http://localhost:8001`)
- API

Comando:

```bash
dotnet run --project src/appHost/AppHost/AppHost.csproj
```

## 2) Opcao manual: API + DynamoDB Local

### 2.1 Subir DynamoDB Local no Docker

```bash
docker run --name dynamodb-local -p 8000:8000 amazon/dynamodb-local
```

### 2.2 Exportar variavel de ambiente

PowerShell:

```powershell
$env:AWS_ENDPOINT_URL_DYNAMODB = "http://localhost:8000"
```

### 2.3 Rodar a API

```bash
dotnet run --project src/backend/DesafioBtg.API/DesafioBtg.API.csproj
```

Swagger:

- `http://localhost:5178/swagger`
- `https://localhost:7288/swagger`

## Build e Testes

Build da solucao:

```bash
dotnet build desafio-btg.sln
```

Observacao:

- Atualmente nao ha projeto de testes automatizados no repositorio.

## Design da Solucao

## Casos de uso

A camada `Application` foi organizada em `UseCases`, cada um com sua interface em `Abstractions`:

- `ICreateUserUseCase`
- `IGetUserByAccountUseCase`
- `IUpdatePixLimitUseCase`
- `IDeleteUserUseCase`
- `IProcessPixUseCase`

Os registros sao feitos no IoC em `AddApplication`, via funcao privada `AddUseCases`.

## Validacao

Validacao com `FluentValidation`:

- `CreateUserRequestValidator`
- `AccountRouteRequestValidator`
- `UpdatePixLimitRequestValidator`
- `ProcessPixRequestValidator`

Regras principais:

- `AgencyNumber`: exatamente 4 digitos
- `AccountNumber`: 5-10 digitos, com DV opcional (`-d`)
- `NationalId` (CPF): 11 digitos numericos
- `PixLimit`: `>= 0` e `<= 1_000_000`
- `Amount` da transacao: `> 0` e maximo 2 casas decimais

## Persistencia DynamoDB

Tabela: `users`

- PK: `pk` (`AGENCY#{agencyNumber}`)
- SK: `sk` (`ACCOUNT#{accountNumber}`)

Campos de negocio:

- `cpf`
- `agency_number`
- `account_number`
- `pix_limit`
- `created_at`
- `active`

O processamento de PIX usa `UpdateItem` com condicao atomica para evitar corrida:

- debita apenas se `pix_limit >= :amount`

## Tratamento de Erros

A API usa `ExceptionFilter` e retorna `ProblemDetails`:

- `400 Validation error` para erros de `FluentValidation`
- status de negocio para `BusinessException` (ex.: `409 Conflict`)
- `500 Unknown error` para falhas nao tratadas

Formato resumido:

```json
{
  "status": 400,
  "title": "Validation error",
  "detail": "...",
  "errors": ["..."]
}
```

## Endpoints da API

Base route: `api/users`

## 1) Criar usuario

`POST /api/users`

Request:

```json
{
  "nationalId": "12345678901",
  "agencyNumber": "0001",
  "accountNumber": "12345-6",
  "pixLimit": 1000.00
}
```

Responses:

- `201 Created` (usuario criado)
- `400 Bad Request` (validacao)
- `409 Conflict` (conta ja cadastrada)

## 2) Buscar usuario por conta

`GET /api/users/{agencyNumber}/{accountNumber}`

Responses:

- `200 OK`
- `404 Not Found`

## 3) Atualizar limite PIX

`PUT /api/users/{agencyNumber}/{accountNumber}/pix-limit`

Request:

```json
{
  "pixLimit": 5000.00
}
```

Responses:

- `200 OK`
- `400 Bad Request`
- `404 Not Found`

## 4) Excluir usuario

`DELETE /api/users/{agencyNumber}/{accountNumber}`

Responses:

- `204 No Content`
- `404 Not Found`

## 5) Processar transacao PIX

`POST /api/users/{agencyNumber}/{accountNumber}/pix-transactions`

Request:

```json
{
  "amount": 120.50
}
```

Response (`200 OK`):

```json
{
  "approved": true,
  "remainingLimit": 879.50,
  "reason": "APPROVED"
}
```

Possiveis `reason`:

- `APPROVED`
- `ACCOUNT_NOT_FOUND`
- `INSUFFICIENT_LIMIT`
- `DENIED`

## Exemplo Rapido com curl

Criar usuario:

```bash
curl -X POST "http://localhost:5178/api/users" \
  -H "Content-Type: application/json" \
  -d '{"nationalId":"12345678901","agencyNumber":"0001","accountNumber":"12345-6","pixLimit":1000.00}'
```

Processar PIX:

```bash
curl -X POST "http://localhost:5178/api/users/0001/12345-6/pix-transactions" \
  -H "Content-Type: application/json" \
  -d '{"amount":120.50}'
```

## Troubleshooting

## Erro: `Endpoint nao encontrado.`

Causa:

- variavel `AWS_ENDPOINT_URL_DYNAMODB` nao definida.

Correcao:

- definir a variavel com endpoint do DynamoDB Local, por exemplo `http://localhost:8000`.

## API sobe mas falha ao iniciar tabela

Verifique:

- DynamoDB Local realmente rodando
- porta `8000` livre e acessivel

## Swagger nao abre

Verifique:

- ambiente `Development`
- URL correta (`http://localhost:5178/swagger`)

## Status do Repositorio (Infra)

No estado atual:

- nao ha `Dockerfile`
- nao ha workflow GitHub Actions

A execucao recomendada e:

- `AppHost` para desenvolvimento local orquestrado, ou
- API + DynamoDB Local manualmente via Docker

---
