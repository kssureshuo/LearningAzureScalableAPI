namespace LearningAzureScalableAPI.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int Ammount { get; set; }
        public string CreatedOn { get; set; }
    }
}
