using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Contracts.Domain;
using Store.Core.Contracts.Interfaces.Models;
using Store.Core.Contracts.Interfaces.Requests;
using Store.Core.Contracts.Interfaces.Services;

namespace Store.Core.Services.Authorization.Roles.Queries.GetRoles
{
    public class GetRoleByIdQuery : IRequest<Role>, IRoleIdentityRequest
    {
        public Guid Id { get; set; }
    }

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Role>
    {
        private readonly ICacheService _cacheService;
        private readonly IRoleService _roleService;

        public GetRoleByIdQueryHandler(ICacheService cacheService, IRoleService roleService)
        {
            _cacheService = cacheService;
            _roleService = roleService;
        }
        
        public async Task<Role> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedRole = await _cacheService.GetCacheAsync<Role>(request.Id.ToString(), cancellationToken);

            if (cachedRole is not null)
                return cachedRole;

            var result = await _roleService.GetRoleAsync(request.Id, cancellationToken);
            
            if (result is null)
                throw new ArgumentException($"Role {request.Id} does not exist!");

            return result;
        }
    }
}