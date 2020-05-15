using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.Data;
using MoviesAPI.Dtos;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration,
            ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;

        }

    [HttpPost("Create", Name= "CreateUser")]
    public async Task<ActionResult<UserToken>> Create([FromBody]UserInfo model)
    {
        var user = new IdentityUser { UserName = model.EmailAddress, Email = model.EmailAddress };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return await BuildToken(model);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("Login", Name= "Login")]
    public async Task<ActionResult<UserToken>> Login([FromBody]UserInfo model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return await BuildToken(model);
        }
        return BadRequest("Invalid login attempt");
    }

    [HttpPost("RenewToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<UserToken>> Renew()
    {
        var userInfo = new UserInfo
        {
            EmailAddress = HttpContext.User.Identity.Name
        };

        return await BuildToken(userInfo);
    }

    [HttpGet("Users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery]PaginationDto paginationDto)
    {
        var queryable = context.Users.AsQueryable();
        queryable = queryable.OrderBy(u => u.Email);
        await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDto.RecordsPerPage);
        var users = await queryable.Paginate(paginationDto).ToListAsync();

        return mapper.Map<List<UserDto>>(users);
    }

    [HttpGet("Roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<List<string>>> GetRoles()
    {
        return await context.Roles.Select(r => r.Name).ToListAsync();
    }

    [HttpPost("AssignRole")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> AssignRole(EditRoleDto editRoleDto)
    {
        var user = await _userManager.FindByIdAsync(editRoleDto.UserId);

        if (user == null)
            return NotFound();

        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDto.RoleName));
        return NoContent();
    }

    [HttpPost("RemoveRole")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> RemoveRole(EditRoleDto editRoleDto)
    {
        var user = await _userManager.FindByIdAsync(editRoleDto.UserId);

        if (user == null)
            return NotFound();

        await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDto.RoleName));
        return NoContent();
    }
    private async Task<UserToken> BuildToken(UserInfo userInfo)
    {
        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.EmailAddress),
                new Claim(ClaimTypes.Email, userInfo.EmailAddress),
            };

            var identityUser = await _userManager.FindByEmailAsync(userInfo.EmailAddress);
            var claimDb = await _userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimDb);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddHours(1);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );
        return new UserToken()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };

    }
}
}