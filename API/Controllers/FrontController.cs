using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatting.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class NotesController(ILogger<NotesController> _logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {

        _logger.LogDebug("NotesController");

        return Ok("GetFront");
    }



    [HttpPost]
    public IActionResult Post()
    {



        return Ok();
    }



}
[ApiController]
[Authorize]
[Route("[controller]")]
public class AuthNotesController() : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {



        return Ok("AuthNotes......work");
    }



    [HttpPost]
    public IActionResult Post()
    {



        return Ok();
    }



}
