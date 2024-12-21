using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.Users;
using Mika.Domain.Contracts.DTOs.Users.Filter;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using Mika.Infastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Application.Services.Implementations
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserRepository _userRepository;
        public UserApplication(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public async Task<OperationResult<object>> CreateAdmin(long SuperAdminId, Request_CreateAdminDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.CreateAdmin(SuperAdminId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> CreateUser(long SuperAdminOrAdminId, Request_CreateUserDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.CreateUser(SuperAdminOrAdminId, model, cancellationToken);
        }

        public async Task<OperationResult<Respone_GetUserDTO>> GetUser(long UserId, Request_UserIdDTO RequestModel, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUser(UserId, RequestModel, cancellationToken);
        }

        public async Task<OperationResult<PagedList<Respone_GetUserDTO>>> GetUsers(long UserId, Filter_GetUserDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUsers(UserId, filter, page, cancellationToken);
        }

        public async Task<OperationResult<Response_AuthenticationDTO>> Login(Request_LoginDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.Login(model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateAdmin(long SuperAdminId, Request_UpdateAdminDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.UpdateAdmin(SuperAdminId, model, cancellationToken);
        }

        public async Task<OperationResult<Response_AuthenticationDTO>> UpdateProfile(long UserId, Request_UpdateSelfUserDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.UpdateProfile(UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateUser(long SuperAdminOrAdminId, Request_UpdateUserDTO model, CancellationToken cancellationToken = default)
        {
            return await _userRepository.UpdateUser(SuperAdminOrAdminId, model, cancellationToken);
        }
    }
}
