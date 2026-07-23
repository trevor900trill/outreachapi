using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutReachToursAPI.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public int? ActiveLeads { get; set; }
        public double? ConversionRate { get; set; }
    }

    public class CustomRole
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    public class PipelineStage
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class ClientActivity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Foreign Key
        public string ClientId { get; set; } = string.Empty;
    }

    public class Client
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string SalesRepId { get; set; } = string.Empty;
        public string StageId { get; set; } = string.Empty;
        public double? BudgetKES { get; set; }
        public string? LastContactDate { get; set; }
        
        public List<ClientActivity> Activities { get; set; } = new();
    }

    public class Tour
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Segment { get; set; } = string.Empty;
        public double PriceKES { get; set; }
        public int DurationDays { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
    }

    public class POSTransaction
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string TourId { get; set; } = string.Empty;
        public double AmountKES { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class Notification
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public bool Read { get; set; }
    }
}
