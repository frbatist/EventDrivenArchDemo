# EventDrivenArchDemo

A simple event-driven architecture demonstration using Azure Functions as containerized event handlers with RabbitMQ messaging for learning and educational purposes.

## Overview

This project demonstrates a basic event-driven architecture pattern using containerized Azure Functions as event handlers. It showcases how to persist data in a SQL database and export information to a denormalized reporting database in CosmosDB through event-driven communication, with minimal code by leveraging Azure Functions bindings and triggers.

**Key Focus**: Testing Azure Functions as containerized event handlers that respond to RabbitMQ message triggers and output directly to databases with minimal boilerplate code.

## ⚠️ Important Notice

**This is a simplified demonstration project and should NOT be used as a reference for production environments.** 

This demo lacks several production-ready features including:
- Security implementation
- Proper separation of concerns
- Design patterns application
- Error handling and resilience patterns
- Monitoring and logging
- Configuration management
- Testing strategies

**Note on messaging**: While the original intention was to use Azure Service Bus, this demo uses RabbitMQ instead to enable out-of-the-box execution with Docker Compose. Azure Service Bus cannot be easily containerized for local development. However, the messaging concepts and patterns demonstrated are very similar between both technologies.

## Architecture

The solution implements a basic event-driven architecture with the following components:

- **SQL Database**: Primary data storage for transactional data
- **CosmosDB Emulator**: Denormalized reporting database (containerized)
- **RabbitMQ**: Event messaging infrastructure
- **Azure Functions**: Containerized event handlers that process RabbitMQ messages and update reporting data

## Technology Stack

- **Backend**: C# .NET
- **Database**: SQL Server (Azure SQL Database)
- **NoSQL Database**: CosmosDB Emulator (Docker) / Azure CosmosDB (optional)
- **Event Processing**: Azure Functions (containerized)
- **Messaging**: RabbitMQ
- **Containerization**: Docker & Docker Compose
- **Cloud Platform**: Microsoft Azure

## Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET SDK (version 8.0 or higher) - for development only

### Configuration

1. Clone the repository:
```bash
git clone https://github.com/yourusername/EventDrivenArchDemo.git
cd EventDrivenArchDemo
```

2. The application runs completely out-of-the-box using Docker Compose with:
   - SQL Server container for transactional data
   - CosmosDB Emulator container for reporting data
   - RabbitMQ container for messaging
   - Azure Functions containers for event processing

3. Optional: For testing with Azure CosmosDB in the cloud, you can:
   - Create a CosmosDB account in Azure
   - Update the connection string in the configuration
   - This is useful for learning Azure CosmosDB features but not required

### Running the Application

1. Start the entire application stack using Docker Compose:
```bash
docker-compose up -d
```

2. The application will start with:
   - SQL Server database
   - CosmosDB Emulator
   - RabbitMQ message broker
   - API application
   - Azure Functions containers (event handlers)

3. Access the application:
   - API: `http://localhost:5000` (or configured port)
   - RabbitMQ Management UI: `http://localhost:15672` (guest/guest)
   - CosmosDB Emulator: `https://localhost:8081/_explorer/index.html`

4. To stop the application:
```bash
docker-compose down
```

## Project Structure

```
EventDrivenArchDemo/
├── src/
│   ├── EventDrivenArchDemo.Api/          # API layer
│   ├── EventDrivenArchDemo.Core/         # Business logic
│   ├── EventDrivenArchDemo.Data/         # Data access layer
│   ├── EventDrivenArchDemo.Events/       # Event definitions
│   └── EventDrivenArchDemo.Handlers/     # Event handlers
├── docs/                                 # Documentation
└── README.md
```

## How It Works

1. **Data Input**: Application receives data through API endpoints
2. **Data Persistence**: Data is stored in SQL Server (containerized)
3. **Event Publishing**: Events are published to RabbitMQ
4. **Event Processing**: Azure Functions consume messages from RabbitMQ and update CosmosDB Emulator using output bindings
5. **Reporting**: Denormalized data is available in CosmosDB Emulator for reporting

## Learning Objectives

This demo helps understand:
- Basic event-driven architecture concepts
- Azure Functions as containerized event handlers
- RabbitMQ messaging patterns and triggers
- Azure Functions bindings for minimal-code database operations
- Data synchronization between different storage systems
- Event sourcing fundamentals
- Containerized application development

## Next Steps / Future Improvements

This demo can be extended with the following enhancements:

- **Service Integration**: Replace hardcoded calls to rent details endpoint with Azure Functions input bindings for better decoupling
- **Authentication**: Implement authentication and authorization mechanisms across all services
- **Frontend Application**: Add a web frontend to interact with the API and display real-time data
- **Real-time Notifications**: Integrate SignalR or similar technology to provide real-time notifications when events are processed
- **Enhanced Error Handling**: Add retry policies, dead letter queues, and comprehensive error handling
- **Monitoring**: Implement logging, metrics, and health checks
- **Testing**: Add unit tests, integration tests, and end-to-end testing

## Contributing

This is an educational project. If you find issues or have suggestions for improvements, feel free to:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Disclaimer

This project is created for educational and demonstration purposes only. It represents a simplified implementation of event-driven architecture and should not be used in production environments without significant enhancements for security, scalability, and reliability.

## Resources

- [Azure Functions Documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- [Azure Functions with Containers](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-linux-custom-image)
- [Azure CosmosDB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Azure CosmosDB Documentation](https://docs.microsoft.com/en-us/azure/cosmos-db/)
- [Event-Driven Architecture Patterns](https://docs.microsoft.com/en-us/azure/architecture/guide/architecture-styles/event-driven)
