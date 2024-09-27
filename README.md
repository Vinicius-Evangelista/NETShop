[Este README é apenas um exemplo, no futuro haverá modificações]

# Aplicação Microservices NETShop

## Visão Geral

**NETShop** é uma aplicação moderna de e-commerce desenvolvida para demonstrar a implementação de uma arquitetura de microsserviços utilizando **ASP.NET Core**. A plataforma foi criada para lidar com operações de compra online escaláveis, incluindo gerenciamento de usuários, catálogo de produtos, processamento de pedidos e serviços de pagamento. Cada microsserviço é implantado de forma independente, sendo construído com foco em desempenho, escalabilidade e manutenção.

O objetivo deste projeto é demonstrar as melhores práticas no desenvolvimento de sistemas distribuídos que possam ser usados como base para plataformas reais de e-commerce.

## Arquitetura

A aplicação está dividida em vários microsserviços, cada um responsável por um domínio específico:

- **Serviço 1: Gerenciamento de Usuários** - Lida com registro, autenticação e autorização de usuários.
- **Serviço 2: Catálogo de Produtos** - Gerencia o inventário de produtos, categorias e detalhes.
- **Serviço 3: Processamento de Pedidos** - Responsável por processar e rastrear pedidos de clientes.
- **Serviço 4: Gateway de Pagamentos** - Integra-se com sistemas de pagamento de terceiros para processar pagamentos.
- **Serviço 5: Notificações** - Envia notificações (e-mail, SMS, etc.) para os usuários sobre o status dos pedidos e promoções.

### Stack Tecnológica

- **ASP.NET Core** - Framework backend.
- **Docker** - Containerização dos serviços.
- **RabbitMQ** - Broker de mensagens para comunicação assíncrona entre os serviços.
- **SQL Server** - Banco de dados para armazenar dados relacionais.
- **MongoDB** - Banco de dados NoSQL para armazenar dados não estruturados (ex: logs, detalhes dos produtos).
- **Redis** - Camada de cache para melhorar o desempenho da aplicação.
- **IdentityServer** - Para autenticação e autorização entre os serviços.
- **Swagger** - Documentação e testes das APIs.
- **Kubernetes** - Orquestrador para gerenciamento de deployment dos microsserviços (opcional).

## Funcionalidades

- **Arquitetura de Microsserviços**: Cada serviço é implantado de forma independente e escalável.
- **Comunicação entre Serviços**: Comunicação síncrona (REST) e assíncrona (RabbitMQ) entre os serviços.
- **API Gateway**: Ponto de entrada centralizado para rotear as requisições dos clientes para os serviços apropriados.
- **Autenticação e Autorização**: Utilizando tokens JWT emitidos pelo IdentityServer para acesso seguro aos serviços.
- **Pipeline CI/CD**: Automatiza o processo de integração e deployment usando GitHub Actions (ou outra ferramenta CI/CD).

## Configuração

### Pré-requisitos

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker](https://www.docker.com/get-started)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [MongoDB](https://www.mongodb.com/try/download/community)
- [RabbitMQ](https://www.rabbitmq.com/download.html)
- Opcional: [Kubernetes](https://kubernetes.io/docs/setup/) para orquestração

### Executando a Aplicação

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/username/nome-do-repositorio.git
   cd nome-do-repositorio
