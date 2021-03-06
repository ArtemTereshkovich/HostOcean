using AutoMapper;
using FluentValidation;
using HostOcean.Application.Tokens.Models;
using HostOcean.Application.Users.Commands.ChangePassword;
using HostOcean.Application.Users.Commands.CreateUser;
using HostOcean.Application.Users.Commands.UpdateUser;
using HostOcean.Application.Users.Models;
using HostOcean.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostOcean.Api.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(UserModel))]
        public async Task<IActionResult> GetMe()
        {
            return Ok(await Mediator.Send(new GetCurrentUserQuery()));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(JwtToken))]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            try
            {
                await Mediator.Send(command);

                var signInQuery = new SignInJwtTokenQuery { Username = command.UserName, Password = command.Password };
                var token = await Mediator.Send(signInQuery);
                return Ok(token);
            }
            catch (ValidationException ex)
            {
                //TODO: move to handler

                var dict = new Dictionary<string, string>();
                foreach (var error in ex.Errors)
                {
                    dict.TryAdd(error.PropertyName, error.ErrorMessage);
                }

                return BadRequest(new { Errors = dict });
            }
        }

        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(JwtToken))]
        public async Task<IActionResult> SignIn([FromBody] SignInJwtTokenQuery signInQuery)
        {
            return Ok(await Mediator.Send(signInQuery));
        }

        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(UserModel))]
        public async Task<IActionResult> Update([FromBody] UpdateUserCommand updateUserCommand)
        {
            return Ok(await Mediator.Send(updateUserCommand));
        }

        [Authorize]
        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(void))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand changePasswordCommand)
        {
            await Mediator.Send(changePasswordCommand);
            return Ok();
        }
    }
}