## 📦 Quick Starter Kafka Consumer Template for .NET

A clean, minimal, Kafka consumer template built with C# and .NET. This project provides a solid foundation for building Kafka-based message consumers using the Confluent.Kafka client, with built-in Docker support for easy deployment and scalability.

--- 

## ⬇️ Installation via dotnet new

You can install this template directly from NuGet using the .NET CLI:

```bash
# Install the template
dotnet new install Quickstarter.Kafka.Consumer::1.0.0
````

---

### Create a new Kafka consumer project

```bash
# Create a new Kafka consumer project
dotnet new kafka-consumer -n MyKafkaConsumer
```

---

## ⚙️ Template Options

You can customize your generated project with the following options:

| Option            | Description                   | Values                       | Default  |
| ----------------- | ----------------------------- | ---------------------------- | -------- |
| `--Framework`     | Target .NET framework version | `net6.0`, `net7.0`, `net8.0` | `net8.0` |
| `--UseOpenSearch` | Enable OpenSearch integration | `true`, `false`              | `false`  |

Example with Options:
```bash
dotnet new kafka-consumer -n MyKafkaConsumer --Framework net8.0 --UseOpenSearch true
```

---

## 📁 Project Structure

```plaintext
Kafka.Consumer
│
├── _tests/                  # JavaScript-based test Kafka message producer
│   └── Core/
│       └── producer.js
│
├── Attributes/              # Custom attributes for consumer discovery
│   └── ConsumeAttribute.cs
│
├── Consumers/              # Application-specific Kafka message consumers
│   ├── AccountingConsumer.cs
│   └── PaymentConsumer.cs
│
├── Extensions/             # Extension methods for DI and serialization
│   ├── SerializationExtensions.cs
│   └── ServiceCollectionExtensions.cs
│
├── Models/                 # Message and base domain models
│   ├── AccountMessage.cs
│   ├── BaseEntity.cs
│   └── PaymentMessage.cs
│
├── Options/                # Strongly typed configuration classes
│   ├── KafkaConsumerConfig.cs
│   ├── KafkaExtraConfig.cs
│   └── OpenSearchConfig.cs
│
├── Services/               # Core consumer logic and business handling
│   ├── IKafkaConsumerBase.cs
│   ├── KafkaConsumerLogic.cs
│   └── KafkaConsumerLogic.cs
|
├── Repositories/           # Core repositories for data access
│   ├── IElasticRepository.cs
│   ├── ElasticRepository.cs
|
├── Workers/                # Background service entry point
│   └── BackgroundRunner.cs
│
├── Dockerfile              # Production-ready Dockerfile
├── docker-compose.yml      # Optional: for local Kafka test setup
├── appsettings.json        # Application configuration
├── appsettings.Development.json
├── GlobalUsings.cs
└── Program.cs              # Host builder and entry point
```

---

## ✅ Features

- Fully asynchronous Kafka consumer loop
- Graceful shutdown and cancellation handling
- Configurable topic subscription and consumer settings
- Integrated with `IHostedService` for background processing
- Structured logging using `ILogger`
- **Docker-ready** with production-grade `Dockerfile`
- Follows modern .NET and microservice best practices with service-repository pattern
- In-built Javascript test producer for local testing

---

## 🛠️ Use Cases

- Event-driven architectures
- Microservices communication
- Real-time data processing
- Containerized applications (Docker, Kubernetes)

---

## 🐳 Docker Support

This project includes a lightweight `Dockerfile` to easily build and run the Kafka consumer in a containerized environment. It's ideal for local development, CI/CD pipelines, and deployment to cloud platforms.

### Build & Run with Docker

```bash
# Build the Docker image
docker build -t kafka-consumer .

# Run the container (ensure Kafka broker is accessible)
docker run -d -p 5000:80 --name kafka-consumer kafka-consumer

# Pull Kafka, Kibana and ElasticSearch images from Docker Hub
docker-compose up -d
````

### 🛠️ Prerequisites
- .NET 8.0 or later
- Apache Kafka broker
- Node.js (for the test producer)
- Docker (optional, for containerization)
- ElasticSearch (optional, for logging)

### ✅ How to Use
1. Clone the repository:
   ```bash
   # clone the repository
   git clone https://github.com/MichaelNchor/quickstarter-dotnet-kafka-consumer-template.git
   
   # navigate to the project directory
   cd quickstarter-dotnet-kafka-consumer-template
    ```
2. Update the `appsettings.json` file with your Kafka broker and topic settings:
3. Build the project:
   ```bash
    # Build and run the project
   dotnet build
   ```
###

---

### 🛠️ Test Producer
- The `producer.js` file in the `_tests` directory contains a simple JavaScript test producer that can be used to send test messages to your Kafka topic.
- To run the test producer, ensure you have Node.js installed and execute the following command
1. Install Node.js dependencies:
   ```bash
   # Install Node.js dependencies
   npm install
   ```
2. Run the test producer:
   ```bash
   # Navigate to the test producer directory
   cd Kafka.Consumer/_tests
   
   # Install dependencies
   npm install
   
   # Run the test producer
   node producer.js
   ```
   
---

## ⭐ Give a Star
If you found this Implementation helpful or used it in your Projects, do give it a star. Thanks!