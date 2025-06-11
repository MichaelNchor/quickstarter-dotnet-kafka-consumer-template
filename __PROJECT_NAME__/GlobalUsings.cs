global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using System.Reflection;
global using Confluent.Kafka;
global using __PROJECT_NAME__.Consumers;
global using __PROJECT_NAME__.Extensions;
global using __PROJECT_NAME__.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using __PROJECT_NAME__.Models;
#if UseOpenSearch
global using __PROJECT_NAME__.Repositories;
global using Nest;
#endif
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Newtonsoft.Json;
global using Quickstarter.Kafka.Consumer.Core.Attributes;
global using Quickstarter.Kafka.Consumer.Core.Options;
global using Quickstarter.Kafka.Consumer.Core.Services;
global using Quickstarter.Kafka.Consumer.Core.Workers;