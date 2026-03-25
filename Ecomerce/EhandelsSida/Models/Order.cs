namespace EhandelsSida.Models
{
    public class Order
    {
        private decimal totalAmount = 0;
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public string Status { get; set; } = "Open";

        public decimal TotalAmount
        {
            get
            {
                foreach (var item in OrderItems)
                {
                    totalAmount += item.Quantity * item.UnitPrice;
                }
                return totalAmount;
            }
        }
    }
}
