using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly ICustomerAccountRepository _customerAccountRepository;
        private readonly IMemberAccountRepository _memberAccountRepository;
        private readonly IDistributorRepository _distributorRepository;
        private readonly IOpCoRepository _opCoRepository;

        public CustomerAccountService(
            ICustomerAccountRepository customerAccountRepository,
            IMemberAccountRepository memberAccountRepository,
            IDistributorRepository distributorRepository,
            IOpCoRepository opCoRepository)
        {
            _customerAccountRepository = customerAccountRepository;
            _memberAccountRepository = memberAccountRepository;
            _distributorRepository = distributorRepository;
            _opCoRepository = opCoRepository;
        }

        public async Task<IEnumerable<CustomerAccountDto>> GetAllCustomerAccountsAsync()
        {
            var customerAccounts = await _customerAccountRepository.GetAllAsync();
            return customerAccounts.Select(MapToDto);
        }

        public async Task<CustomerAccountDto?> GetCustomerAccountByIdAsync(int id)
        {
            var customerAccount = await _customerAccountRepository.GetByIdAsync(id);
            return customerAccount != null ? MapToDto(customerAccount) : null;
        }

        public async Task<CustomerAccountDto> CreateCustomerAccountAsync(CreateCustomerAccountDto createCustomerAccountDto, string createdBy)
        {
            // Validate member account exists
            var memberAccount = await _memberAccountRepository.GetByIdAsync(createCustomerAccountDto.MemberAccountId);
            if (memberAccount == null)
            {
                throw new ArgumentException("Member account not found", nameof(createCustomerAccountDto.MemberAccountId));
            }

            // Validate distributor exists
            var distributor = await _distributorRepository.GetByIdAsync(createCustomerAccountDto.DistributorId);
            if (distributor == null)
            {
                throw new ArgumentException("Distributor not found", nameof(createCustomerAccountDto.DistributorId));
            }

            // Validate OpCo exists if provided
            if (createCustomerAccountDto.OpCoId.HasValue)
            {
                var opCo = await _opCoRepository.GetByIdAsync(createCustomerAccountDto.OpCoId.Value);
                if (opCo == null)
                {
                    throw new ArgumentException("OpCo not found", nameof(createCustomerAccountDto.OpCoId));
                }
            }

            // Validate unique customer account number per distributor
            var exists = await _customerAccountRepository.ExistsByCustomerAccountNumberAsync(
                createCustomerAccountDto.DistributorId, 
                createCustomerAccountDto.CustomerAccountNumber);
            if (exists)
            {
                throw new ArgumentException("Customer account number already exists for this distributor", 
                    nameof(createCustomerAccountDto.CustomerAccountNumber));
            }

            var customerAccount = new CustomerAccount
            {
                MemberAccountId = createCustomerAccountDto.MemberAccountId,
                DistributorId = createCustomerAccountDto.DistributorId,
                OpCoId = createCustomerAccountDto.OpCoId,
                CustomerName = createCustomerAccountDto.CustomerName,
                CustomerAccountNumber = createCustomerAccountDto.CustomerAccountNumber,
                Address = createCustomerAccountDto.Address,
                City = createCustomerAccountDto.City,
                State = createCustomerAccountDto.State,
                ZipCode = createCustomerAccountDto.ZipCode,
                Country = createCustomerAccountDto.Country,
                PhoneNumber = createCustomerAccountDto.PhoneNumber,
                Email = createCustomerAccountDto.Email,
                // New fields
                SalesRep = createCustomerAccountDto.SalesRep,
                DSO = createCustomerAccountDto.DSO,
                StartDate = createCustomerAccountDto.StartDate,
                EndDate = createCustomerAccountDto.EndDate,
                TRACSAccess = createCustomerAccountDto.TRACSAccess,
                Markup = createCustomerAccountDto.Markup,
                AuditDate = createCustomerAccountDto.AuditDate,
                ToEntegra = createCustomerAccountDto.ToEntegra,
                DateToEntegra = createCustomerAccountDto.DateToEntegra,
                CombinedUniqueID = createCustomerAccountDto.CombinedUniqueID,
                InternalNotes = createCustomerAccountDto.InternalNotes,
                Association = (createCustomerAccountDto.Association.HasValue ? (Models.CustomerAssociation)createCustomerAccountDto.Association.Value : Models.CustomerAssociation.CSN),
                Status = (CustomerAccountStatus)createCustomerAccountDto.Status,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var createdCustomerAccount = await _customerAccountRepository.AddAsync(customerAccount);
            return MapToDto(createdCustomerAccount);
        }

        public async Task<CustomerAccountDto> UpdateCustomerAccountAsync(int id, UpdateCustomerAccountDto updateCustomerAccountDto, string modifiedBy)
        {
            var customerAccount = await _customerAccountRepository.GetByIdAsync(id);
            if (customerAccount == null)
            {
                throw new ArgumentException("Customer account not found", nameof(id));
            }

            // Validate member account exists
            var memberAccount = await _memberAccountRepository.GetByIdAsync(updateCustomerAccountDto.MemberAccountId);
            if (memberAccount == null)
            {
                throw new ArgumentException("Member account not found", nameof(updateCustomerAccountDto.MemberAccountId));
            }

            // Validate distributor exists
            var distributor = await _distributorRepository.GetByIdAsync(updateCustomerAccountDto.DistributorId);
            if (distributor == null)
            {
                throw new ArgumentException("Distributor not found", nameof(updateCustomerAccountDto.DistributorId));
            }

            // Validate OpCo exists if provided
            if (updateCustomerAccountDto.OpCoId.HasValue)
            {
                var opCo = await _opCoRepository.GetByIdAsync(updateCustomerAccountDto.OpCoId.Value);
                if (opCo == null)
                {
                    throw new ArgumentException("OpCo not found", nameof(updateCustomerAccountDto.OpCoId));
                }
            }

            // Validate unique customer account number per distributor
            var exists = await _customerAccountRepository.ExistsByCustomerAccountNumberAsync(
                updateCustomerAccountDto.DistributorId, 
                updateCustomerAccountDto.CustomerAccountNumber, 
                id);
            if (exists)
            {
                throw new ArgumentException("Customer account number already exists for this distributor", 
                    nameof(updateCustomerAccountDto.CustomerAccountNumber));
            }

            customerAccount.MemberAccountId = updateCustomerAccountDto.MemberAccountId;
            customerAccount.DistributorId = updateCustomerAccountDto.DistributorId;
            customerAccount.OpCoId = updateCustomerAccountDto.OpCoId;
            customerAccount.CustomerName = updateCustomerAccountDto.CustomerName;
            customerAccount.CustomerAccountNumber = updateCustomerAccountDto.CustomerAccountNumber;
            customerAccount.Address = updateCustomerAccountDto.Address;
            customerAccount.City = updateCustomerAccountDto.City;
            customerAccount.State = updateCustomerAccountDto.State;
            customerAccount.ZipCode = updateCustomerAccountDto.ZipCode;
            customerAccount.Country = updateCustomerAccountDto.Country;
            customerAccount.PhoneNumber = updateCustomerAccountDto.PhoneNumber;
            customerAccount.Email = updateCustomerAccountDto.Email;
            // New fields
            customerAccount.SalesRep = updateCustomerAccountDto.SalesRep;
            customerAccount.DSO = updateCustomerAccountDto.DSO;
            customerAccount.StartDate = updateCustomerAccountDto.StartDate;
            customerAccount.EndDate = updateCustomerAccountDto.EndDate;
            customerAccount.TRACSAccess = updateCustomerAccountDto.TRACSAccess;
            customerAccount.Markup = updateCustomerAccountDto.Markup;
            customerAccount.AuditDate = updateCustomerAccountDto.AuditDate;
            customerAccount.ToEntegra = updateCustomerAccountDto.ToEntegra;
            customerAccount.DateToEntegra = updateCustomerAccountDto.DateToEntegra;
            customerAccount.CombinedUniqueID = updateCustomerAccountDto.CombinedUniqueID;
            customerAccount.InternalNotes = updateCustomerAccountDto.InternalNotes;
            if (updateCustomerAccountDto.Association.HasValue)
            {
                customerAccount.Association = (Models.CustomerAssociation)updateCustomerAccountDto.Association.Value;
            }
            customerAccount.Status = (CustomerAccountStatus)updateCustomerAccountDto.Status;
            if (updateCustomerAccountDto.IsActive.HasValue)
            {
                customerAccount.IsActive = updateCustomerAccountDto.IsActive.Value;
            }
            // if not provided, preserve existing IsActive
            customerAccount.ModifiedDate = DateTime.UtcNow;
            customerAccount.ModifiedBy = modifiedBy;

            var updatedCustomerAccount = await _customerAccountRepository.UpdateAsync(customerAccount);
            return MapToDto(updatedCustomerAccount);
        }

        public async Task<bool> DeleteCustomerAccountAsync(int id)
        {
            var customerAccount = await _customerAccountRepository.GetByIdAsync(id);
            if (customerAccount == null)
            {
                return false;
            }

            await _customerAccountRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ActivateCustomerAccountAsync(int id, string modifiedBy)
        {
            var customerAccount = await _customerAccountRepository.GetByIdAsync(id);
            if (customerAccount == null)
            {
                return false;
            }

            customerAccount.IsActive = true;
            customerAccount.Status = CustomerAccountStatus.Active;
            customerAccount.ModifiedDate = DateTime.UtcNow;
            customerAccount.ModifiedBy = modifiedBy;

            await _customerAccountRepository.UpdateAsync(customerAccount);
            return true;
        }

        public async Task<bool> DeactivateCustomerAccountAsync(int id, string modifiedBy)
        {
            var customerAccount = await _customerAccountRepository.GetByIdAsync(id);
            if (customerAccount == null)
            {
                return false;
            }

            customerAccount.IsActive = false;
            //customerAccount.Status = CustomerAccountStatus.Inactive;
            customerAccount.ModifiedDate = DateTime.UtcNow;
            customerAccount.ModifiedBy = modifiedBy;

            await _customerAccountRepository.UpdateAsync(customerAccount);
            return true;
        }

        public async Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByMemberAccountIdAsync(int memberAccountId)
        {
            var customerAccounts = await _customerAccountRepository.GetByMemberAccountIdAsync(memberAccountId);
            return customerAccounts.Select(MapToDto);
        }

        public async Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByDistributorIdAsync(int distributorId)
        {
            var customerAccounts = await _customerAccountRepository.GetByDistributorIdAsync(distributorId);
            return customerAccounts.Select(MapToDto);
        }

        public async Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByOpCoIdAsync(int opCoId)
        {
            var customerAccounts = await _customerAccountRepository.GetByOpCoIdAsync(opCoId);
            return customerAccounts.Select(MapToDto);
        }

        public async Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByStatusAsync(int status)
        {
            var customerAccounts = await _customerAccountRepository.GetByStatusAsync((CustomerAccountStatus)status);
            return customerAccounts.Select(MapToDto);
        }

        public async Task<CustomerAccountDto?> GetCustomerAccountByAccountNumberAsync(int distributorId, string customerAccountNumber)
        {
            var customerAccount = await _customerAccountRepository.GetByCustomerAccountNumberAsync(distributorId, customerAccountNumber);
            return customerAccount != null ? MapToDto(customerAccount) : null;
        }

        public async Task<(IEnumerable<CustomerAccountDto> CustomerAccounts, int TotalCount)> SearchCustomerAccountsAsync(string searchTerm, int? memberAccountId = null, int? distributorId = null, int? opCoId = null, int? status = null, bool? isActive = null, int? industryId = null, int? association = null, DateTime? startDate = null, DateTime? endDate = null, bool? tracsAccess = null, bool? toEntegra = null, string? state = null, int page = 1, int pageSize = 10)
        {
            var statusEnum = status.HasValue ? (CustomerAccountStatus)status.Value : null as CustomerAccountStatus?;
            var customerAccounts = await _customerAccountRepository.SearchAsync(
                searchTerm,
                memberAccountId,
                distributorId,
                opCoId,
                statusEnum,
                isActive,
                industryId,
                association,
                startDate,
                endDate,
                tracsAccess,
                toEntegra,
                state,
                page,
                pageSize);
            var totalCount = await _customerAccountRepository.GetCountAsync(
                searchTerm,
                memberAccountId,
                distributorId,
                opCoId,
                statusEnum,
                isActive,
                industryId,
                association,
                startDate,
                endDate,
                tracsAccess,
                toEntegra,
                state);

            return (customerAccounts.Select(MapToDto), totalCount);
        }


        private static CustomerAccountDto MapToDto(CustomerAccount customerAccount)
        {
            return new CustomerAccountDto
            {
                Id = customerAccount.Id,
                MemberAccountId = customerAccount.MemberAccountId,
                MemberAccountName = customerAccount.MemberAccount?.FacilityName,
                MemberNumber = customerAccount.MemberAccount?.MemberNumber,
                DistributorId = customerAccount.DistributorId,
                DistributorName = customerAccount.Distributor?.Name,
                OpCoId = customerAccount.OpCoId,
                OpCoName = customerAccount.OpCo?.Name,
                CustomerName = customerAccount.CustomerName,
                CustomerAccountNumber = customerAccount.CustomerAccountNumber,
                Address = customerAccount.Address,
                City = customerAccount.City,
                State = customerAccount.State,
                ZipCode = customerAccount.ZipCode,
                Country = customerAccount.Country,
                PhoneNumber = customerAccount.PhoneNumber,
                Email = customerAccount.Email,
                // New fields
                SalesRep = customerAccount.SalesRep,
                DSO = customerAccount.DSO,
                StartDate = customerAccount.StartDate,
                EndDate = customerAccount.EndDate,
                TRACSAccess = customerAccount.TRACSAccess,
                Markup = customerAccount.Markup,
                AuditDate = customerAccount.AuditDate,
                ToEntegra = customerAccount.ToEntegra,
                DateToEntegra = customerAccount.DateToEntegra,
                CombinedUniqueID = customerAccount.CombinedUniqueID,
                InternalNotes = customerAccount.InternalNotes,
                Association = (int)customerAccount.Association,
                AssociationName = customerAccount.Association.ToString(),
                Status = (int)customerAccount.Status,
                StatusName = customerAccount.Status.ToString(),
                IsActive = customerAccount.IsActive,
                CreatedDate = customerAccount.CreatedDate,
                ModifiedDate = customerAccount.ModifiedDate,
                CreatedBy = customerAccount.CreatedBy,
                ModifiedBy = customerAccount.ModifiedBy
            };
        }
    }
}
