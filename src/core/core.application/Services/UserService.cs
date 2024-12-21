using core.application.Contract.API.DTO.Party.User;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.infrastructure.Services;
using core.application.Contract.Infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.UserDTOs;
using core.domain.entity.structureModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace core.application.Services
{
    public class UserService : IUserService
    {
        public IUserRepository _userRepository { get; set; }
        public readonly IMessagingService _messagingService;
        public UserService(IUserRepository userRepository, IMessagingService messagingService)
        {
            _userRepository = userRepository;
            _messagingService = messagingService;
        }


        public async Task<UserGetResponseDTO> GetUserById(int id)
        {
            UserModel userModel = await _userRepository.GetUserAsync(id);
            ;
            return userModel.MapUserModelToUserGetResponse();
        }

        public async Task<List<UserGetResponseDTO>> GetAllUser()
        {
            List<UserModel> userModels = await _userRepository.GetAllUserAsync();
            List<UserGetResponseDTO> userResponses = userModels
                .Select(userModel => userModel.MapUserModelToUserGetResponse())
                .ToList();
            return userResponses;
        }


        public async Task<UserGetResponseDTO> GetUserByPhone(string phoneNumber)
        {
            UserModel userModel = await _userRepository.GetUserByPhoneAsync(phoneNumber);

            return userModel.MapUserModelToUserGetResponse();
        }

        public async Task<List<string>> GetUsersPhoneNumber(int ComplexId)
        {
            return await _userRepository.GetUsersPhoneNumberAsync(ComplexId);
        }


        public async Task<int> CreateUser(UserCreateRequest userCreateDTO)
        {
            UserModel userModel = userCreateDTO.MapUserCreateRequestToUserModel();
            int result = await _userRepository.AddUserAsync(userModel);
            if (result > 0) return userModel.Id;
            else
                throw new Exception("Some thing go wrong in save user in database");

        }
        public async Task<bool> UpdateUser(UserUpdateRequest userUpdateDTO) //this part probably should be simplified by giving the dto to our infrastructure or sth else i need time for this
        {

            UserModel userModel = userUpdateDTO.MapUserUpdateDTOToUserModel();
            return await _userRepository.UpdateUserAsync(userModel) > 0;
        }
        public async Task<bool> DeletUser(int id)
        {
            return await _userRepository.DeleteUserAsync(id) > 0;
        }

        public async Task<OperationResult<object>> SetUserConnection(int userId, string connection)
        {
            return await _userRepository.SetUserConnection(userId, connection);
        }

        public async Task<OperationResult<object>> RemoveUserConnection(int userId)
        {
            return await _userRepository.RemoveUserConnection(userId);
        }

        public async Task<OperationResult<object>> RemoveUserConnection(string connection)
        {
            return await _userRepository.RemoveUserConnection(connection);
        }

        public async Task<List<string>> GetAllConnections()
        {
            return await _userRepository.GetAllConnections();
        }

        public async Task<OperationResult<object>> CreateUserAndResident(string token, int adminId, Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _userRepository.CreateUserAndResident(adminId, model, cancellationToken);
            if (!operation.Success)
            {
                return new OperationResult<object>(operation.OperationName).Failed(operation.Message, operation.ExMessage, operation.Status);
            }
            return await _messagingService.CreateUser(token, operation.Object.UserId, model, cancellationToken);

        }
        public async Task<OperationResult<object>> UpdateUserAndResident(string token, int adminId, Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _userRepository.UpdateUserAndResident(adminId, model, cancellationToken);
            if (!operation.Success)
            {
                return new OperationResult<object>(operation.OperationName).Failed(operation.Message, operation.ExMessage, operation.Status);
            }
            return await _messagingService.UpdateUser(token, operation.Object.UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateUserAndOwner(string token, int adminId, Request_UpdateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _userRepository.UpdateUserAndOwner(adminId, model, cancellationToken);
            if (!operation.Success)
            {
                return new OperationResult<object>(operation.OperationName).Failed(operation.Message, operation.ExMessage, operation.Status);
            }
            return await _messagingService.UpdateUser(token, operation.Object.UserId, new Request_UpdateUserAndResidentDomainDTO
            {
                Address = model.Address,
                Age = model.Age,
                Email = model.Email,
                Gender = model.Gender,
                Lastname = model.Lastname,
                Name = model.Name,
                NationalID = model.NationalID,
                PhoneNumber = model.PhoneNumber,
                UserId = model.UserId,
                Unit = new Update_UnitDomainDTO
                {
                    FromDate = model.Unit.FromDate,
                    IsHead = null,
                    Renting = null,
                    ToDate = model.Unit.ToDate,
                    UnitId = model.Unit.UnitId,
                },
                Password = model.Password,
            }, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteUserAndOwnerResident(string token, int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _userRepository.DeleteUserAndOwnerResident(adminId, model, cancellationToken);
            if (!operation.Success)
            {
                return new OperationResult<object>(operation.OperationName).Failed(operation.Message, operation.ExMessage, operation.Status);
            }
            return await _messagingService.DeleteUser(token, model, cancellationToken);
        }

        public async Task<OperationResult<object>> CreateUserAndOwner(string token, int adminId, Request_CreateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _userRepository.CreateUserAndOwner(adminId, model, cancellationToken);
            if (!operation.Success)
            {
                return new OperationResult<object>(operation.OperationName).Failed(operation.Message, operation.ExMessage, operation.Status);
            }
            return await _messagingService.CreateUser(token, operation.Object.UserId, new Request_CreateUserAndResidentDomainDTO
            {
                Name = model.Name,
                Lastname = model.Lastname,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Password = model.Password,
            }, cancellationToken);
        }

        public async Task<OperationResult<object>> CreateResidentsOwnersOfUnit(string token, int adminId, Request_CreateResidentsOwnersOfUnitDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _userRepository.CreateResidentsOwnersOfUnit(adminId, model, cancellationToken);
            if (!operation.Success)
            {
                return new OperationResult<object>(operation.OperationName).Failed(operation.Message, operation.ExMessage, operation.Status);
            }
            return await _messagingService.CreateUsers(token, operation.List, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteOwnership(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DeleteOwnership(model, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteResidence(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DeleteResidence(model, cancellationToken);
        }

        public async Task<OperationResult<Response_GetUsersByUnitDomainDTO>> GetUsersByUnitId(Request_UnitIdDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUsersByUnitId(model, cancellationToken);
        }

        public async Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> CreateCoreUser(int adminId, Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.CreateCoreUser(adminId, model, cancellationToken);
        }

        public async Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> UpdateCoreUser(int adminId, Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.UpdateCoreUser(adminId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteCoreUser(int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DeleteCoreUser(adminId, model, cancellationToken);
        }

        public async Task<OperationResult<Response_GetUserDomainDTO>> GetCoreUsers(Filter_GetUserDomainDTO? filter, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetCoreUsers(filter, cancellationToken);
        }
    }
}
