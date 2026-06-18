using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class ApplicationUserController(IApplicationUserRepository applicationUserRepository) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var users = await applicationUserRepository.GetUsersAsync();
    if (users == null || !users.Any())
    {
      return NotFound();
    }
    return Ok(users);
  }

  [HttpGet("{userId:guid}")]
  public async Task<IActionResult> GetById(Guid userId)
  {
    var user = await applicationUserRepository.GetUserByIdAsync(userId);
    if (user == null)
    {
      return NotFound();
    }
    return Ok(user);
  }

  [HttpPost]
  public async Task<IActionResult> Create(ApplicationUser user)
  {
    var createdUser = await applicationUserRepository.CreateUserAsync(user);
    return CreatedAtAction(nameof(GetById), new { userId = createdUser.Id }, createdUser);
  }

  [HttpPut]
  public async Task<IActionResult> Update(ApplicationUser user)
  {
    if (user.Id == Guid.Empty)
    {
      return BadRequest();
    }
    var updatedUser = await applicationUserRepository.UpdateUserAsync(user);
    if (updatedUser == null)
    {
      return NotFound();
    }
    return Ok(updatedUser);
  }

  [HttpDelete("{userId:guid}")]
  public async Task<IActionResult> Delete(Guid userId)
  {
    var deleted = await applicationUserRepository.DeleteUserAsync(userId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
