using LearningAzureScalableAPI.Contracts;
using LearningAzureScalableAPI.Data;
using LearningAzureScalableAPI.DTOs;
using LearningAzureScalableAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LearningAzureScalableAPI.Repository
{
    public class OrdersRepository : IOrderRepository
    {
        private readonly AppDbContextFile _dbContext;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        public OrdersRepository(AppDbContextFile appDbContextFile, IDistributedCache distributedCache, IConfiguration configuration)
        {
            _dbContext = appDbContextFile;
            _cache = distributedCache;
            _configuration = configuration;
        }

        public async Task<OrderDTO> GetOrderById(int id)
        {
            try
            {
                Orders ob = new();
                string cahceKey = $"order_{id}";
                var cacheData = await _cache.GetStringAsync(cahceKey);
                if (cacheData != null)
                {
                    return JsonConvert.DeserializeObject<OrderDTO>(cacheData);
                }
                ob = await _dbContext.orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (ob == null) return null;
                var dto = new OrderDTO
                {
                    Id = ob.Id,
                    OrderNumber = ob.OrderNumber,
                    Amount = ob.Ammount
                };

                await _cache.SetStringAsync(cahceKey, JsonConvert.SerializeObject(dto), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
                return dto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GetApplicationName()
        {
            string result = string.Empty;
            result = _configuration["APP_NAME"];
            if (string.IsNullOrEmpty(result))
                result = "NotFound";
            return result;

        }
    }
}
