using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class MemberAccountService : IMemberAccountService
    {
        private readonly IMemberAccountRepository _memberAccountRepository;
        private readonly IIndustryRepository _industryRepository;

        public MemberAccountService(IMemberAccountRepository memberAccountRepository, IIndustryRepository industryRepository)
        {
            _memberAccountRepository = memberAccountRepository;
            _industryRepository = industryRepository;
        }

        public async Task<IEnumerable<MemberAccountDto>> GetAllMemberAccountsAsync()
        {
            var memberAccounts = await _memberAccountRepository.GetAllAsync();
            return memberAccounts.Select(MapToDto);
        }

        public async Task<MemberAccountDto?> GetMemberAccountByIdAsync(int id)
        {
            var memberAccount = await _memberAccountRepository.GetByIdAsync(id);
            return memberAccount != null ? MapToDto(memberAccount) : null;
        }

        public async Task<MemberAccountDto> CreateMemberAccountAsync(CreateMemberAccountDto createMemberAccountDto, string createdBy)
        {
            // Validate unique member number
            var exists = await _memberAccountRepository.ExistsByMemberNumberAsync(createMemberAccountDto.MemberNumber);
            if (exists)
            {
                throw new ArgumentException("Member number already exists", nameof(createMemberAccountDto.MemberNumber));
            }

            // Validate industry exists if provided
            if (createMemberAccountDto.IndustryId.HasValue)
            {
                var industry = await _industryRepository.GetByIdAsync(createMemberAccountDto.IndustryId.Value);
                if (industry == null)
                {
                    throw new ArgumentException("Industry not found", nameof(createMemberAccountDto.IndustryId));
                }
            }

            var memberAccount = new MemberAccount
            {
                MemberNumber = createMemberAccountDto.MemberNumber,
                FacilityName = createMemberAccountDto.FacilityName,
                Address = createMemberAccountDto.Address,
                City = createMemberAccountDto.City,
                State = createMemberAccountDto.State,
                ZipCode = createMemberAccountDto.ZipCode,
                Country = createMemberAccountDto.Country,
                PhoneNumber = createMemberAccountDto.PhoneNumber,

                IndustryId = createMemberAccountDto.IndustryId,
                W9 = createMemberAccountDto.W9,
                W9Date = createMemberAccountDto.W9Date,
                TaxId = createMemberAccountDto.TaxId,
                BusinessType = createMemberAccountDto.BusinessType,
                // New fields
                LopDate = createMemberAccountDto.LopDate ?? DateTime.UtcNow.Date,
                InternalNotes = createMemberAccountDto.InternalNotes,
                ClientGroupEnrollment = createMemberAccountDto.ClientGroupEnrollment,
                SalesforceAccountName = string.IsNullOrWhiteSpace(createMemberAccountDto.SalesforceAccountName)
                    ? createMemberAccountDto.FacilityName
                    : createMemberAccountDto.SalesforceAccountName,
                VMAPNumber = createMemberAccountDto.VMAPNumber,
                VMSupplierName = createMemberAccountDto.VMSupplierName,
                VMSupplierSite = createMemberAccountDto.VMSupplierSite,
                PayType = createMemberAccountDto.PayType,
                ParentMemberAccountNumber = createMemberAccountDto.ParentMemberAccountNumber,
                EntegraGPONumber = createMemberAccountDto.EntegraGPONumber,
                ClientGroupNumber = createMemberAccountDto.ClientGroupNumber,
                EntegraIdNumber = createMemberAccountDto.EntegraIdNumber,
                AuditDate = DateTime.UtcNow,
                Status = (MemberAccountStatus)createMemberAccountDto.Status,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var createdMemberAccount = await _memberAccountRepository.AddAsync(memberAccount);
            return MapToDto(createdMemberAccount);
        }

        public async Task<MemberAccountDto> UpdateMemberAccountAsync(int id, UpdateMemberAccountDto updateMemberAccountDto, string modifiedBy)
        {
            var memberAccount = await _memberAccountRepository.GetByIdAsync(id);
            if (memberAccount == null)
            {
                throw new ArgumentException("Member account not found", nameof(id));
            }

            // Validate unique member number
            var exists = await _memberAccountRepository.ExistsByMemberNumberAsync(updateMemberAccountDto.MemberNumber, id);
            if (exists)
            {
                throw new ArgumentException("Member number already exists", nameof(updateMemberAccountDto.MemberNumber));
            }

            // Validate industry exists if provided
            if (updateMemberAccountDto.IndustryId.HasValue)
            {
                var industry = await _industryRepository.GetByIdAsync(updateMemberAccountDto.IndustryId.Value);
                if (industry == null)
                {
                    throw new ArgumentException("Industry not found", nameof(updateMemberAccountDto.IndustryId));
                }
            }

            memberAccount.MemberNumber = updateMemberAccountDto.MemberNumber;
            memberAccount.FacilityName = updateMemberAccountDto.FacilityName;
            memberAccount.Address = updateMemberAccountDto.Address;
            memberAccount.City = updateMemberAccountDto.City;
            memberAccount.State = updateMemberAccountDto.State;
            memberAccount.ZipCode = updateMemberAccountDto.ZipCode;
            memberAccount.Country = updateMemberAccountDto.Country;
            memberAccount.PhoneNumber = updateMemberAccountDto.PhoneNumber;

            memberAccount.IndustryId = updateMemberAccountDto.IndustryId;
            memberAccount.W9 = updateMemberAccountDto.W9;
            memberAccount.W9Date = updateMemberAccountDto.W9Date;
            memberAccount.TaxId = updateMemberAccountDto.TaxId;
            memberAccount.BusinessType = updateMemberAccountDto.BusinessType;
            // New fields
            memberAccount.LopDate = updateMemberAccountDto.LopDate ?? memberAccount.LopDate ?? DateTime.UtcNow.Date;
            memberAccount.InternalNotes = updateMemberAccountDto.InternalNotes;
            memberAccount.ClientGroupEnrollment = updateMemberAccountDto.ClientGroupEnrollment;
            memberAccount.SalesforceAccountName = string.IsNullOrWhiteSpace(updateMemberAccountDto.SalesforceAccountName)
                ? memberAccount.FacilityName
                : updateMemberAccountDto.SalesforceAccountName;
            memberAccount.VMAPNumber = updateMemberAccountDto.VMAPNumber;
            memberAccount.VMSupplierName = updateMemberAccountDto.VMSupplierName;
            memberAccount.VMSupplierSite = updateMemberAccountDto.VMSupplierSite;
            memberAccount.PayType = updateMemberAccountDto.PayType ?? memberAccount.PayType;
            memberAccount.ParentMemberAccountNumber = updateMemberAccountDto.ParentMemberAccountNumber;
            memberAccount.EntegraGPONumber = updateMemberAccountDto.EntegraGPONumber;
            memberAccount.ClientGroupNumber = updateMemberAccountDto.ClientGroupNumber;
            memberAccount.EntegraIdNumber = updateMemberAccountDto.EntegraIdNumber;
            memberAccount.AuditDate = DateTime.UtcNow;
            memberAccount.Status = (MemberAccountStatus)updateMemberAccountDto.Status;
            memberAccount.IsActive = updateMemberAccountDto.IsActive;
            memberAccount.ModifiedDate = DateTime.UtcNow;
            memberAccount.ModifiedBy = modifiedBy;

            var updatedMemberAccount = await _memberAccountRepository.UpdateAsync(memberAccount);
            return MapToDto(updatedMemberAccount);
        }

        public async Task<bool> DeleteMemberAccountAsync(int id)
        {
            var memberAccount = await _memberAccountRepository.GetByIdAsync(id);
            if (memberAccount == null)
            {
                return false;
            }

            // Check if member account has customer accounts
            if (memberAccount.CustomerAccounts?.Any() == true)
            {
                throw new InvalidOperationException("Cannot delete member account with existing customer accounts");
            }

            await _memberAccountRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ActivateMemberAccountAsync(int id, string modifiedBy)
        {
            var memberAccount = await _memberAccountRepository.GetByIdAsync(id);
            if (memberAccount == null)
            {
                return false;
            }

            memberAccount.IsActive = true;
            memberAccount.Status = MemberAccountStatus.Active;
            memberAccount.ModifiedDate = DateTime.UtcNow;
            memberAccount.ModifiedBy = modifiedBy;

            await _memberAccountRepository.UpdateAsync(memberAccount);
            return true;
        }

        public async Task<bool> DeactivateMemberAccountAsync(int id, string modifiedBy)
        {
            var memberAccount = await _memberAccountRepository.GetByIdAsync(id);
            if (memberAccount == null)
            {
                return false;
            }

            memberAccount.IsActive = false;
            //memberAccount.Status = MemberAccountStatus.Inactive;
            memberAccount.ModifiedDate = DateTime.UtcNow;
            memberAccount.ModifiedBy = modifiedBy;

            await _memberAccountRepository.UpdateAsync(memberAccount);
            return true;
        }

        public async Task<MemberAccountDto?> GetMemberAccountByMemberNumberAsync(string memberNumber)
        {
            var memberAccount = await _memberAccountRepository.GetByMemberNumberAsync(memberNumber);
            return memberAccount != null ? MapToDto(memberAccount) : null;
        }

        public async Task<IEnumerable<MemberAccountDto>> GetMemberAccountsByIndustryIdAsync(int industryId)
        {
            var memberAccounts = await _memberAccountRepository.GetByIndustryIdAsync(industryId);
            return memberAccounts.Select(MapToDto);
        }

        public async Task<IEnumerable<MemberAccountDto>> GetMemberAccountsByStatusAsync(int status)
        {
            var memberAccounts = await _memberAccountRepository.GetByStatusAsync((MemberAccountStatus)status);
            return memberAccounts.Select(MapToDto);
        }

        public async Task<(IEnumerable<MemberAccountDto> MemberAccounts, int TotalCount)> SearchMemberAccountsAsync(string searchTerm, int? industryId = null, int? status = null, string? w9 = null, string? state = null, int page = 1, int pageSize = 10)
        {
            var memberAccounts = await _memberAccountRepository.SearchAsync(searchTerm, industryId, status.HasValue ? (MemberAccountStatus)status.Value : null, w9, state, page, pageSize);
            var totalCount = await _memberAccountRepository.GetCountAsync(searchTerm, industryId, status.HasValue ? (MemberAccountStatus)status.Value : null, w9, state);

            return (memberAccounts.Select(MapToDto), totalCount);
        }

        private static MemberAccountDto MapToDto(MemberAccount memberAccount)
        {
            return new MemberAccountDto
            {
                Id = memberAccount.Id,
                MemberNumber = memberAccount.MemberNumber,
                FacilityName = memberAccount.FacilityName,
                Address = memberAccount.Address,
                City = memberAccount.City,
                State = memberAccount.State,
                ZipCode = memberAccount.ZipCode,
                Country = memberAccount.Country,
                PhoneNumber = memberAccount.PhoneNumber,

                IndustryId = memberAccount.IndustryId,
                IndustryName = memberAccount.Industry?.Name,
                W9 = memberAccount.W9,
                W9Date = memberAccount.W9Date,
                TaxId = memberAccount.TaxId,
                BusinessType = memberAccount.BusinessType,
                // New fields
                LopDate = memberAccount.LopDate,
                InternalNotes = memberAccount.InternalNotes,
                ClientGroupEnrollment = memberAccount.ClientGroupEnrollment,
                SalesforceAccountName = memberAccount.SalesforceAccountName,
                VMAPNumber = memberAccount.VMAPNumber,
                VMSupplierName = memberAccount.VMSupplierName,
                VMSupplierSite = memberAccount.VMSupplierSite,
                PayType = memberAccount.PayType,
                PayTypeName = memberAccount.PayType,
                ParentMemberAccountNumber = memberAccount.ParentMemberAccountNumber,
                EntegraGPONumber = memberAccount.EntegraGPONumber,
                ClientGroupNumber = memberAccount.ClientGroupNumber,
                EntegraIdNumber = memberAccount.EntegraIdNumber,
                AuditDate = memberAccount.AuditDate,
                Status = (int)memberAccount.Status,
                StatusName = memberAccount.Status.ToString(),
                IsActive = memberAccount.IsActive,
                CreatedDate = memberAccount.CreatedDate,
                ModifiedDate = memberAccount.ModifiedDate,
                CreatedBy = memberAccount.CreatedBy,
                ModifiedBy = memberAccount.ModifiedBy,
                CustomerAccountsCount = memberAccount.CustomerAccounts?.Count ?? 0
            };
        }
    }
}
