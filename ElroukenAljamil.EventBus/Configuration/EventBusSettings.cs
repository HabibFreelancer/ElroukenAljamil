using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Configuration
{
    /// <summary>
    /// Configuration RabbitMQ partagée entre tous les services.
    /// </summary>
    public class EventBusSettings
    {
        public const string SectionName = "EventBus";

        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public int RetryCount { get; set; } = 3;
        public int RetryIntervalSeconds { get; set; } = 5;
        public int PrefetchCount { get; set; } = 16;
        public int ConcurrencyLimit { get; set; } = 10;
    }


}
