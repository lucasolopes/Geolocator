# Documentação - Projeto Geolocator

## Visão Geral

O Geolocator é uma API desenvolvida em .NET 8 que fornece serviços de dados geográficos brasileiros baseados nos dados oficiais do IBGE. O sistema gerencia informações sobre regiões, estados, mesorregiões, microrregiões, municípios, distritos e subdistritos, permitindo sincronização automática com a API do IBGE e oferecendo funcionalidades avançadas de busca através de integração com Elasticsearch.

## Tecnologias Utilizadas

### Backend
- **.NET 8**: Framework base de desenvolvimento
- **ASP.NET Core**: Para o desenvolvimento da API REST
- **Entity Framework Core 8**: ORM para acesso ao banco de dados
- **PostgreSQL 15**: Banco de dados relacional
- **Elasticsearch**: Motor de busca para consultas eficientes
- **MediatR**: Implementação do padrão mediator para CQRS
- **Quartz.NET**: Agendamento de jobs para sincronização de dados
- **NEST**: Cliente .NET para Elasticsearch
- **Docker & Docker Compose**: Containerização da aplicação

### Arquitetura
- **Arquitetura Onion (Hexagonal)**: Separação clara de responsabilidades por camadas
- **CQRS (Command Query Responsibility Segregation)**: Separação entre comandos e consultas
- **Repository Pattern**: Abstração da camada de acesso a dados
- **DDD (Domain-Driven Design)**: Princípios de design orientado ao domínio
- **Clean Architecture**: Princípios de arquitetura limpa

## Estrutura do Projeto

O projeto segue uma estrutura organizada em camadas conforme a arquitetura Onion:

### Core
- **Domain**: Entidades de domínio e regras de negócio
- **Application**: Casos de uso, interfaces, DTOs, comandos e consultas

### Infraestrutura
- **Persistence**: Implementação do acesso a dados com EF Core
- **ExternalServices**: Serviços externos como a API do IBGE
- **Elasticsearch**: Implementação da integração com Elasticsearch
- **BackgroundJobs**: Jobs agendados para sincronização de dados

### API
- **Geolocator**: Controllers, configurações, middlewares e ponto de entrada da aplicação

## Funcionalidades

### Sincronização de Dados do IBGE
- Sincronização automática de dados geográficos do IBGE
- Agendamento de sincronização periódica
- Endpoints para sincronização manual

### Busca Avançada
- Busca de localidades por nome ou sigla
- Suporte a busca com correção ortográfica (fuzzy search)
- Filtragem por tipo de localidade (região, estado, município, etc.)
- Paginação de resultados

### Armazenamento Eficiente
- Persistência de dados em PostgreSQL
- Indexação de conteúdo em Elasticsearch para buscas rápidas

## Uso da Aplicação

### Requisitos
- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento)

### Configurações
As configurações da aplicação estão disponíveis no arquivo `geolocator.env`:
- `ConnectionString`: String de conexão com o PostgreSQL
- `ElasticsearchUri`: URI do servidor Elasticsearch

### Inicialização
Para iniciar a aplicação:

```bash
docker-compose up -d
```

### Endpoints Principais

#### Sincronização
- `POST /api/ibge/sync`: Sincroniza todos os dados do IBGE
- `GET /api/ibge/regions`: Sincroniza apenas as regiões
- `GET /api/ibge/states`: Sincroniza apenas os estados
- `GET /api/ibge/municipalities`: Sincroniza apenas os municípios
- `POST /api/elasticsearch/sync`: Sincroniza dados com o Elasticsearch

#### Busca
- `GET /api/search?q=termo`: Busca em todas as localidades
- `GET /api/search/regions?q=termo`: Busca apenas em regiões
- `GET /api/search/states?q=termo`: Busca apenas em estados
- `GET /api/search/municipalities?q=termo`: Busca apenas em municípios

## Arquitetura Detalhada

### Padrão CQRS
O projeto implementa o padrão CQRS para separar operações de leitura e escrita:
- **Commands**: Responsáveis por operações de alteração de estado (ex: SyncRegionsCommand)
- **Queries**: Responsáveis por operações de leitura (ex: SearchLocationsQuery)
- **Handlers**: Processadores dos comandos e consultas

### Jobs Automáticos
Jobs configurados com Quartz.NET:
- **IbgeSyncJob**: Sincroniza dados com o IBGE periodicamente
- **ElasticsearchSyncJob**: Atualiza os índices no Elasticsearch

### Integração com Elasticsearch
- Implementação de índices otimizados para busca textual
- Analisadores de texto para português brasileiro
- Busca com correção ortográfica (fuzzy search)

## Desenvolvimento e Contribuição

### Setup de Desenvolvimento
1. Clone o repositório
2. Instale o .NET 8 SDK
3. Instale o Docker e Docker Compose
4. Execute `docker-compose up -d` para iniciar os serviços dependentes
5. Abra a solução no Visual Studio ou VS Code

### Execução de Testes
Testes podem ser executados via Visual Studio ou via linha de comando:

```bash
dotnet test
```

### Migrações do Banco de Dados
O Entity Framework Core gerencia o esquema do banco de dados:

```bash
dotnet ef migrations add [MigrationName] -p ./Infraestructure/Persistence/Persistence.csproj -s ./Geolocator/Geolocator.csproj
dotnet ef database update -p ./Infraestructure/Persistence/Persistence.csproj -s ./Geolocator/Geolocator.csproj
```

## Licença
Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE.txt para detalhes.

## Contato
Para mais informações ou suporte, entre em contato com o mantenedor do projeto.

---

*Nota: Este projeto usa dados oficiais do IBGE e serve como um exemplo de implementação de API moderna seguindo os princípios de arquitetura limpa, DDD e CQRS em .NET 8.*

---
Essa doc foi gerada Totalmente por IA, sem intervenção humana. :robot:
```