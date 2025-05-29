global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using System.Reflection;
global using Confluent.Kafka;
global using Kafka.Consumer.Attributes;
global using Kafka.Consumer.Consumers;
global using Kafka.Consumer.Extensions;
global using Kafka.Consumer.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Kafka.Consumer.Models;
#if UseOpenSearch
global using Kafka.Consumer.Repositories;
global using Nest;
#endif
global using Kafka.Consumer.Services;
global using Kafka.Consumer.Workers;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Newtonsoft.Json;