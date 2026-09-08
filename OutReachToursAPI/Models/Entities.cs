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
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
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

    public class Hotel
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Country { get; set; } = "Kenya";
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ReservationDesk { get; set; } = string.Empty;
        public double Rating { get; set; } = 4.5;
        public string ImageUrl { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string RoomTypesJson { get; set; } = "[]";
    }

    public class HotelBooking
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string HotelId { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string TourId { get; set; } = string.Empty;
        public string TourTitle { get; set; } = string.Empty;
        public string CheckInDate { get; set; } = string.Empty; // YYYY-MM-DD
        public string CheckOutDate { get; set; } = string.Empty; // YYYY-MM-DD
        public int RoomsCount { get; set; } = 1;
        public int GuestsCount { get; set; } = 2;
        public string Status { get; set; } = "Confirmed"; // Confirmed, Provisional Hold, Checked In, Completed, Cancelled
        public string? HoldExpiryDate { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public double RatePerNightKES { get; set; }
        public double TotalAmountKES { get; set; }
        public double PaidAmountKES { get; set; } = 0;
        public string? PaystackReference { get; set; }
        public string? PaymentUrl { get; set; }
        public string? HotelContactEmail { get; set; }
        public string? HotelNotifiedAt { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }

    public class LiveAvailabilityQuery
    {
        public string CityCode { get; set; } = "NBO";
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set; } = string.Empty;
        public int Adults { get; set; } = 2;
        public int Rooms { get; set; } = 1;
        public string? HotelName { get; set; }
    }

    public class LiveHotelOffer
    {
        public string OfferId { get; set; } = string.Empty;
        public string HotelId { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public string RoomCategory { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string BedType { get; set; } = string.Empty;
        public int Beds { get; set; } = 1;
        public double RatePerNight { get; set; }
        public double TotalPrice { get; set; }
        public string Currency { get; set; } = "KES";
        public bool IsAvailable { get; set; } = true;
        public string CancellationPolicy { get; set; } = "Refundable with advance notice";
        public string Description { get; set; } = string.Empty;
        public double Rating { get; set; } = 4.5;
        public string Address { get; set; } = string.Empty;
    }

    public class LiveHotelAvailabilityResult
    {
        public int HotelsFound { get; set; }
        public int OffersFound { get; set; }
        public string CityCode { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set; } = string.Empty;
        public string DataSource { get; set; } = "Amadeus GDS Live API";
        public bool IsLiveGds { get; set; } = false;
        public string? Note { get; set; }
        public List<LiveHotelOffer> Offers { get; set; } = new();
    }
}
