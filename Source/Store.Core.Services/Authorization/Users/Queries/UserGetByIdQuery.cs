﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Contracts.Interfaces;
using Store.Core.Contracts.Models;
using Store.Core.Services.Common.Interfaces;

namespace Store.Core.Services.Authorization.Users.Queries
{
    public class UserGetByIdQuery : IRequest<User>, IIdentity
    {
        public Guid Id { get; set; }
    }
    
    public class UserGetByIdQueryHandler : IRequestHandler<UserGetByIdQuery, User>
    {
        private readonly IUserService _userService;

        public UserGetByIdQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<User> Handle(UserGetByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserAsync(request.Id, cancellationToken);

            if (user is null)
                throw new ArgumentException($"Can't get user {request.Id}");

            return user;
        }
    }
}