using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;


[ApiController]
[Route("api/[controller]")] // /api/*users*
public class UsersController(DataContext context) : ControllerBase
{

    //Creating Endpoints 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();
        
        return Ok(users); 

    }

    [HttpGet("{id:int}")] // Parameter of the URL /api/users/*id* the int part is for type safety 
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        
        if (user == null) return NotFound();

        return user; 

    }
}