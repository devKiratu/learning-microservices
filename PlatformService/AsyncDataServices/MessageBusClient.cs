using AutoMapper;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration config;
        private readonly IConnection connection;
        private readonly IModel channel;

        public MessageBusClient(IConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            var factory = new ConnectionFactory 
            { 
                HostName = config["RabbitMQHost"], 
                Port = int.Parse(config["RabbitMQPort"]) 
            };

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

                Console.WriteLine("--> Connected to Message Bus");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            //serialize obj to be sent over the wire
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMq connection open, sending message ...");

                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMq connection is closed.");

            }

        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"--> Message sent: {message}");
        }

        public void Dispose()
        {
            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbiMq Connection Shutdown");
        }
    }
}
