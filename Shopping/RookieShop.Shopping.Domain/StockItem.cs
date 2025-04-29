namespace RookieShop.Shopping.Domain;

public class StockItem
{
    public readonly string Sku;
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public Guid ImageId { get; private set; }
    
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    
#pragma warning disable CS8618, CS9264
    public StockItem() {}
#pragma warning restore CS8618, CS9264

    public StockItem(string sku, string name, decimal price, Guid imageId)
    {
        Sku = sku;
        Name = name;
        Price = price;
        ImageId = imageId;
        AvailableQuantity = 0;
        ReservedQuantity = 0;
    }

    public void UpdateInfo(string name, decimal price, Guid imageId)
    {
        Name = name;
        Price = price;
        ImageId = imageId;
    }

    public void AddUnits(int quantity)
    {
        AvailableQuantity += quantity;
    }

    public bool CanReserve(int quantity)
    {
        return AvailableQuantity >= quantity;
    }

    public void Reserve(int quantity)
    {
        if (!CanReserve(quantity))
        {
            throw new NotEnoughUnitsToReserveException(Sku, quantity);
        }
        
        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
    }

    public void ConfirmReservation(int quantity)
    {
        if (ReservedQuantity < quantity)
        {
            throw new InvalidOperationException($"Cannot confirm reservation of {quantity} units of item {Sku}.");
        }
        
        ReservedQuantity -= quantity;
    }

    public void ReleaseReservation(int quantity)
    {
        if (ReservedQuantity < quantity)
        {
            throw new InvalidOperationException($"Cannot release reservation of {quantity} units of item {Sku}.");
        }
        
        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;
    }
}

public class NotEnoughUnitsToReserveException : Exception
{
    public readonly string Sku;
    public readonly int Quantity;

    public NotEnoughUnitsToReserveException(string sku, int quantity)
        : base($"Cannot reserve {quantity} units of item {sku}.")
    {
        Sku = sku;
        Quantity = quantity;
    }
}