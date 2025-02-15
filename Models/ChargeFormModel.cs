namespace Inventory.Models
{
    public class ChargeFormModel
    {
        public int SelectedItemID { get; set; }
        public int SelectedServiceID { get; set; }
        public string AddOn { get; set; } = string.Empty;
        public bool IsNewCustomer { get; set; }

        public List<ChargeItemModel> ChargeItems { get; set; } = new List<ChargeItemModel>();
        public List<ChargeServiceModel> ChargeServices { get; set; } = new List<ChargeServiceModel>();

    }
}
