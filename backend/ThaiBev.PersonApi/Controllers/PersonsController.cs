using Microsoft.AspNetCore.Mvc;
using ThaiBev.PersonApi.Dtos;
using ThaiBev.PersonApi.Services;

namespace ThaiBev.PersonApi.Controllers;

[ApiController]
[Route("api/persons")]
[Produces("application/json")]
public class PersonsController(IPersonService personService) : ControllerBase
{
    /// <summary>ดึงรายการทั้งหมด เรียง Id ASC (ข้อมูลใหม่อยู่ท้ายตาราง)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonListItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonListItem>>> GetAll(CancellationToken ct)
    {
        return Ok(await personService.GetAllAsync(ct));
    }

    /// <summary>ดึงข้อมูลรายคนสำหรับ View modal</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDetail>> GetById(int id, CancellationToken ct)
    {
        var person = await personService.GetByIdAsync(id, ct);
        return person is null ? NotFound() : Ok(person);
    }

    /// <summary>เพิ่มข้อมูลใหม่</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PersonDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDetail>> Create(CreatePersonRequest request, CancellationToken ct)
    {
        var created = await personService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
