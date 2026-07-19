using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Application.DTOs
{
    /// <summary>
    /// DTO pour le tableau de bord admin.
    /// </summary>
    public record DashboardDto
    {
        public int TotalSent { get; init; }
        public int TotalFailed { get; init; }
        public double OverallSuccessRate { get; init; }
        public double AverageDeliveryTimeMs { get; init; }
        public List<ChannelMetricsDto> ByChannel { get; init; } = new();
        public List<HourlyMetricsDto> Hourly { get; init; } = new();
    }

    public record ChannelMetricsDto
    {
        public string Channel { get; init; } = string.Empty;
        public int Sent { get; init; }
        public int Failed { get; init; }
        public double SuccessRate { get; init; }
        public double AvgDeliveryMs { get; init; }
    }

    public record HourlyMetricsDto
    {
        public DateTime Hour { get; init; }
        public int Sent { get; init; }
        public int Failed { get; init; }
    }

    /// <summary>
    /// DTO pour la configuration digest de l'utilisateur.
    /// </summary>
    public record DigestConfigDto
    {
        public string Frequency { get; init; } = "Daily";
        public int PreferredHour { get; init; } = 8;
        public string PreferredDay { get; init; } = "Monday";
        public string TimeZone { get; init; } = "Europe/Paris";
        public bool IsActive { get; init; } = true;
    }

}
