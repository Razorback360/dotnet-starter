using System.ComponentModel.DataAnnotations;

namespace CarDealer.Api.DTOs.Purchases;

public class PurchaseRequestCreate
{
    [Required]
    public int VehicleId { get; set; }
}
