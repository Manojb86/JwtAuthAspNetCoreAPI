using JwtAuthAspNetCoreApi.Data.DatabaseContext;
using JwtAuthAspNetCoreApi.Helpers;
using JwtAuthAspNetCoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace JwtAuthAspNetCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly InventoryDbContext inventoryDbContext;
        private readonly IConfiguration configuration;
        public AccountController(InventoryDbContext inventoryDbContext, IConfiguration configuration)
        {
            this.inventoryDbContext = inventoryDbContext;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register(UserAccount userAccount)
        {
            if (userAccount != null && !string.IsNullOrEmpty(userAccount.Email) &&
                !string.IsNullOrEmpty(userAccount.Password))
            {
                var passwordDetails = PasswordProtector.GetHashAndSalt(userAccount.Password);
                userAccount.Password = passwordDetails.HashText;
                userAccount.PasswordSalt = passwordDetails.SaltText;
                userAccount.HashIteration = passwordDetails.HashIteration;
                userAccount.HashLength = passwordDetails.HashLength;
                userAccount.CreatedDate = DateTime.Now;
                userAccount.UpdateDate = DateTime.Now;
                userAccount.Version = 1;
                userAccount.UserName = userAccount.Email;

                this.inventoryDbContext.UserAccounts.Add(userAccount);
                await inventoryDbContext.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Required data for user registeration not found");
            }

            return Ok();
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            if (signInModel != null && !string.IsNullOrEmpty(signInModel.UserName) && !string.IsNullOrEmpty(signInModel.Password))
            {
                var user = await inventoryDbContext.UserAccounts.FirstOrDefaultAsync(x => x.Email.Equals(signInModel.UserName));
                if (user != null && !string.IsNullOrEmpty(user.Password) && !string.IsNullOrEmpty(user.PasswordSalt))
                {
                    bool passwordVerification = PasswordProtector.VerifyPassword
                        (signInModel.Password,
                        user.Password,
                        user.PasswordSalt,
                        user.HashLength
                        , user.HashIteration);

                    if (passwordVerification)
                    {
                        string jwtToken = JwtTokenGenerator.GetJwtToken(user, this.configuration);

                        return Ok(jwtToken);
                    }
                    else
                    {
                        return Unauthorized("User authentication failed. Please enter valid user account details for sign in.");
                    }
                }
                else
                {
                    return NotFound("User not found. Please enter valid user name");
                }

            }
            else
            {
                return BadRequest("Required data for user sign in not found");
            }
        }
    }
}