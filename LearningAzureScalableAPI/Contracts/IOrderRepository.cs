using LearningAzureScalableAPI.DTOs;
using LearningAzureScalableAPI.Models;

namespace LearningAzureScalableAPI.Contracts
{
    public interface IOrderRepository
    {
        Task<OrderDTO> GetOrderById(int id);
        Task<string> GetApplicationName();
    }
}
