using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mika.Domain.Contracts.DTOs.Users;
using Mika.Domain.Contracts.DTOs.Users.Filter;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
namespace Mika.Application.Services.Interfaces
{
    public interface IUserApplication
    {
        Task<OperationResult<Response_AuthenticationDTO>> Login(Request_LoginDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_AuthenticationDTO>> UpdateProfile(long UserId, Request_UpdateSelfUserDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<PagedList<Respone_GetUserDTO>>> GetUsers(long UserId, Filter_GetUserDTO? filter, PageModel? page, CancellationToken cancellationToken = default);
        Task<OperationResult<Respone_GetUserDTO>> GetUser(long UserId, Request_UserIdDTO RequestModel, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateAdmin(long SuperAdminId, Request_CreateAdminDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateAdmin(long SuperAdminId, Request_UpdateAdminDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateUser(long SuperAdminOrAdminId, Request_CreateUserDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateUser(long SuperAdminOrAdminId, Request_UpdateUserDTO model, CancellationToken cancellationToken = default);
    }
}
