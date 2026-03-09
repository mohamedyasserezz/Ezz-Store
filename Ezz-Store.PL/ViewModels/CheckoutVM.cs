using System.ComponentModel.DataAnnotations;

namespace Ezz_Store.PL.ViewModels;

public class CheckoutVM
{
    [Display(Name = "Saved Address")]
    public int? SelectedAddressId { get; set; }

    public List<AddressOptionVM> AddressOptions { get; set; } = [];

    [Display(Name = "Country")]
    [MaxLength(100)]
    public string? Country { get; set; }

    [Display(Name = "City")]
    [MaxLength(100)]
    public string? City { get; set; }

    [Display(Name = "Street")]
    [MaxLength(250)]
    public string? Street { get; set; }

    [Display(Name = "Zip")]
    [MaxLength(20)]
    public string? Zip { get; set; }

    public CartVM Cart { get; set; } = new();

    public bool CreateNewAddress => !SelectedAddressId.HasValue;
}

public class AddressOptionVM
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
}
