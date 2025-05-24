namespace MalyFarmar.Messages
{
    public class ProductUpdatedMessage
    {
        public int ProductId { get; }

        public ProductUpdatedMessage(int productId)
        {
            ProductId = productId;
        }
    }
}