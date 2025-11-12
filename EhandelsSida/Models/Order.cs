namespace EhandelsSida.Models
{
    public class Order
    {
        public int id { get; set; }

        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }

        public ApplicationBuilder User { get; set; }

        public List<OrderItem> OrderItems { get; set; } new List<OrderItem>();

            public decimal TotalAmount
        {

            get
            {
                decimal total = 0;
                foreach (var item in OrderItems)
                {
                    total += item.Quantity * item.UnitPrice;
                }
                return total;
            }
        }

             
      
    }
}
