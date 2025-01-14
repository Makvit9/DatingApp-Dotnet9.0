using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseAPIController
{
    
    [HttpPost("register")] // account/register 
    public async Task<ActionResult<UserDTO>> Register (RegisterDTO registerDTO)
    {

        if (await _UserExists(registerDTO.Username)) return BadRequest("Username already exists!!");
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            Username = registerDTO.Username.ToLower(), // We are going to sae all the usernames as a lowercase  
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key 
            
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => 
        x.Username == loginDTO.Username.ToLower());

        if (user == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for (int i =0 ; i < computedHash.Length ; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Incorrect Password");
        }

        return new UserDTO
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        };

    }

    private async Task<bool> _UserExists(string Username)
    {
        return await context.Users.AnyAsync(x => x.Username.ToLower() == Username.ToLower());
    }
}

