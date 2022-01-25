using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IEventProcessor _eventProcessor;
        private IConnection? _connection;
        private IModel? _channel;
        private string? _queueName;

        public MessageBusSubscriber(IConfiguration config,
                IEventProcessor eventProcessor)
        {
            _config = config;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQHost"],
                Port = Convert.ToInt16(_config["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(
                    queue: _queueName,
                    exchange: "trigger",
                    routingKey: "");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine($"--> RabbitMQ connected!");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> RabbitMQHost connect error: {ex.Message}");
            }

        }
        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"--> RabbitMQ connection Shutdown");
        }
        public override void Dispose()
        {
            Console.WriteLine($"--> RabbitMQ Dispose");
            if (_channel?.IsOpen ?? false)
            {
                _channel?.Close();
                _connection?.Close();
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
             {
                 Console.WriteLine($"--> RabbitMQ Get event");
                 var body = ea.Body;
                 var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                 _eventProcessor.ProcessEvent(notificationMessage);
             };

            _channel.BasicConsume(
                        queue: _queueName,
                        autoAck: true,
                        consumer: consumer
            );

            return Task.CompletedTask;
        }
    }
}