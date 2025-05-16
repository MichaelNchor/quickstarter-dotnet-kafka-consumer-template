## 📦 Kafka Consumer Template for .NET

A clean, minimal, and production-ready Kafka consumer template built with C# and .NET. This project provides a solid foundation for building Kafka-based message consumers using the Confluent.Kafka client, with built-in Docker support for easy deployment and scalability.

--- 

## ✅ Features

- Fully asynchronous Kafka consumer loop
- Graceful shutdown and cancellation handling
- Configurable topic subscription and consumer settings
- Integrated with `IHostedService` for background processing
- Structured logging using `ILogger`
- **Docker-ready** with production-grade `Dockerfile`
- Follows modern .NET and microservice best practices with service-repository pattern

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
