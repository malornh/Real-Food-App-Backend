using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface IFarmsService
    {
        Task<IEnumerable<FarmDto>> GetFarms();
        Task<IEnumerable<FarmDto>> GetFarmsByIds(string farmIds);
        Task<IEnumerable<FarmDto>> GetFarmsByUserId(string userId);
        Task<FarmDto> GetFarm(int id);
        Task<FarmFullInfoDto> GetFarmWithProducts(int farmId);
        Task<FarmDto> CreateFarm(FarmDto farmDto);
        Task<FarmDto> UpdateFarm(int id, FarmDto farmDto);
        Task<bool> DeleteFarm(int id);
    }
}
