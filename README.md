
# Minimal API com .NET 9.0

Este projeto é uma aplicação .NET 9 usando **Minimal APIs**, autenticação via **JWT com refresh token**, banco de dados **in-memory** com Entity Framework Core, e controle de usuários com **roles e claims**.

## Documentação da API

#### Retorna o estado da api

```http
  GET /health
```

#### Retorna as informações do usuário

```http
  GET /auth/me
```

| Header   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Authorization`      | `Bearer Token` | **Obrigatório** |

#### Efetua login e retorna access e refresh tokens

```http
  POST /auth/login
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Username`      | `string` | **Obrigatório** |
| `Password`      | `string` | **Obrigatório** |

#### Renova o token de acesso com base no refresh token

```http
  POST /auth/refresh-token
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `RefreshToken`      | `string` | **Obrigatório** |
| `Username`      | `string` | **Obrigatório** |

#### Insere uma nova tarefa

```http
  POST /todos
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Title`      | `string` | **Obrigatório** |
| `Description`      | `string` | **Obrigatório** |

#### Lista todas as tarefas cadastradas

```http
  GET /todos
```

#### Retorna tarefa selecionada

```http
  GET /todos/{id}
```

#### Atualizar tarefa selecionada

```http
  PUT /todos/{id}
```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Title`      | `string` | **Obrigatório** |
| `Description`      | `string` | **Obrigatório** |

#### Atualiza o status da tarefa selecionada

```http
  PATCH /todos/{id}
```

#### Remove a tarefa selecionada

```http
  DELETE /todos/{id}
```

| Header   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Authorization`      | `Bearer Token` | **Obrigatório** |

## Rodando localmente

Requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Git](https://git-scm.com/)
- [IDE recomendada: JetBrains Rider](https://www.jetbrains.com/rider/) ou [Visual Studio Code](https://code.visualstudio.com/)

Clone o projeto
```bash
  git clone https://github.com/bcbrunofarias/minimal-api
```

Execute o projeto em modo de desenvolvimento
```
  ASPNETCORE_ENVIRONMENT=Development
```

## Usuários para utilização

#### Usuários
| Name   | Username | Password  | Role       | Claims                                   |
| :---------- | :---------- |:---------- | :--------- | :------------------------------------------ |
| `Bruno` |`admin@dotnet.com`      | `admin` | `admin` | [`CanRead`, `CanWrite`, `CanDelete`] |
| `César` |`common@dotnet.com`      | `common` | `common` | [`CanRead`] |

## Funcionalidades

- Autenticação JWT
- Refresh Token persistido
- Claims e Roles com policies
- Controle de usuários e permissões
- API RESTful com endpoints protegidos
- Swagger OpenAPI
- Banco em memória para testes



