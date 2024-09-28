using Microsoft.AspNetCore.Mvc;
using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface IFarmsService
    {
        Task<IEnumerable<FarmDto>> GetAllFarmsAsync();
        Task<IEnumerable<FarmDto>> GetFarmsByIdsAsync(string farmIds);
        Task<IEnumerable<FarmDto>> GetFarmsByUserIdAsync(string userId);
        Task<FarmDto> GetFarmByIdAsync(int id);
        Task<FarmFullInfoDto> GetFarmWithProductsAsync(int farmId);
        Task<FarmDto> CreateFarm(FarmDto farmDto);
        Task<FarmDto> UpdateFarm(FarmDto farmDto);
        Task DeleteFarm(int id);
    }
}
