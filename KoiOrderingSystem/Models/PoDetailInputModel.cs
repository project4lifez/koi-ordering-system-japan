
namespace KoiOrderingSystem.Models;

public class PoDetailInputModel
{
    public DateOnly? Day { get; set; }
    public int FarmId { get; set; }
    public decimal? Deposit { get; set; }
    public string Note { get; set; }

    public int[] KoiId { get; set; }
    public int[] Quantity { get; set; }
    public decimal?[] TotalKoiPrice { get; set; }
}
