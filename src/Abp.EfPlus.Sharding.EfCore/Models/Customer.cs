namespace Abp.EfPlus.Sharding.EfCore.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime LastLogin { get; set; }
    public Boolean IsActive { get; set; }
}