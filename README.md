## 📦 Quick Starter Kafka Consumer Template for .NET

A clean, minimal, Kafka consumer template built with C# and .NET. This project provides a solid foundation for building Kafka-based message consumers using the Confluent.Kafka client, with built-in Docker support for easy deployment and scalability.

--- 

## 📁 Project Structure

```plaintext
quickstarter-dotnet-kafka-consumer-template/
├── src/
│   └── Kafka.Consumer/
│       ├── Attributes/                  
│       │   └── ConsumeAttribute.cs
│       ├── Consumers/                   
│       │   ├── AccountingConsumer.cs
│       │   └── PaymentConsumer.cs
│       ├── Core/                        
│       │   ├── main.js
│       │   └── producer.js
│       ├── Extensions/                  
│       │   ├── SerializationExtensions.cs
│       │   └── ServiceCollectionExtensions.cs
│       ├── Models/                      
│       │   ├── AccountMessage.cs
│       │   ├── BaseEntity.cs
│       │   └── PaymentMessage.cs
│       ├── Options/                     
│       │   ├── KafkaConsumerConfig.cs
│       │   ├── KafkaExtraConfig.cs
│       │   └── OpenSearchConfig.cs
│       ├── Repositories/                
│       │   └── ElasticRepository.cs
│       ├── Services/                    
│       │   ├── IKafkaConsumerLogic.cs
│       │   ├── KafkaConsumerBase.cs
│       │   └── KafkaConsumerLogic.cs
│       ├── Workers/                    
│       │   └── BackgroundRunner.cs
│       ├── appsettings*.json           
│       ├── Kafka.Consumer.csproj     
│       └── Program.cs                 
│
├── tests/
│   └── Kafka.Consumer.Tests/ 
│       ├── Components/
│       │   ├── ConfigurationManager.cs
│       │   └── DiFixture.cs
│       ├── appsettings.json   
│       └── Kafka.Consumer.Tests.csproj 
│
└── .idea/, .git/, README.md, etc.
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
docker build -t kafka-consumer

# Run the container (ensure Kafka broker is accessible)
docker run --rm kafka-consumer

# Pull Kafka, Kibana and ElasticSearch images from Docker Hub
docker-compose up -d
````

### 🛠️ Prerequisites
- .NET 8.0 or later
- Apache Kafka broker
- Node.js (for the test producer)
- Docker (optional, for containerization)
- ElasticSearch (optional, for logging)

### 🪜 How to Use
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

### 🔧 Test Producer
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
   cd src/Kafka.Consumer/_tests
   
   # Install dependencies
   npm install
   
   # Run the test producer
   node producer.js
   ```
---

### 🧪 Testing
- Unit tests are included in the `tests` directory.

---

## Give a Star⭐️
If you found this Implementation helpful or used it in your Projects, do give it a star. Thanks!