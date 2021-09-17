using CommandsService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration config;
        private readonly IEventProcessor eventProcessor;
        private IConnection connection;
        private IModel channel;
        private string queueName;

        public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));

            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQHost"],
                Port = int.Parse(config["RabbitMQPort"])
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine($"--> Listening on the Message Bus ...");

            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"--> Connection shutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event received");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                eventProcessor.ProcessEvent(notificationMessage);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;

        }

        public override void Dispose()
        {
            if (connection.IsOpen)
            {
                connection.Close();
                channel.Close();
            }

            base.Dispose();
        }
    }
}
