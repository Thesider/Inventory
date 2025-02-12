namespace Inventory.Enums
{

    public enum Unit { Pharmacy, Clinic, Personal, Other }

    public enum Role { Admin, Pharmacy, Clinic }

    public enum Category
    {
        Pill, Syrup, Injection, Cream, Powder, Drops, Spray, Inhaler, Other
    }

    public enum Status
    {
        Available, OutOfStock, Expired, RunningLow
    }


}
