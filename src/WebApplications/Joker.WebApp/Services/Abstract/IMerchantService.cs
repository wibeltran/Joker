using System;
using System.Threading.Tasks;
using Joker.WebApp.ViewModels.Merchant;
using Joker.WebApp.ViewModels.Merchant.Request;

namespace Joker.WebApp.Services.Abstract
{
    public interface IMerchantService
    {
        Task<MerchantViewModel> CreateAsync(CreateMerchantViewModel createMerchantViewModel);
        Task<MerchantViewModel> UpdateAsync(UpdateMerchantViewModel updateMerchantViewModel);
        Task<MerchantViewModel> GetByIdAsync(Guid id);
    }
}