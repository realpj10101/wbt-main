using api.Extensions;
using api.Helpers;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class CoachController(ICoachRepository _coachRepository) : BaseApiController
{

    [AllowAnonymous]
    [HttpGet("get-coaches")]
    public async Task<ActionResult<IEnumerable<CoachDto>>> GetAllCoachs([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        PagedList<RootModel> pagedCoach = await _coachRepository.GetAllCoachsAsync(paginationParams, cancellationToken);

        if (pagedCoach.Count == 0)
            return NoContent();

        PaginationHeader paginationHeader = new(
            CurrentPage: pagedCoach.CurrentPage,
            ItemsPerPage: pagedCoach.PageSize,
            TotalItems: pagedCoach.TotalItems,
            TotalPages: pagedCoach.TotalPages
        );

        Response.AddPaginationHeader(paginationHeader);

        List<CoachDto> coachDtos = [];

        foreach (RootModel coach in pagedCoach)
        {
            coachDtos.Add(CoachMappers.ConvertRootModelToCoachDto(coach));
        }

        return coachDtos;
    }

    [HttpGet("get-by-coach-id/{coachId}")]
    public async Task<ActionResult<CoachDto>> GetByCoachId(string coachId, CancellationToken cancellationToken)
    {
        CoachDto? coachDto = await _coachRepository.GetByCoachIdAsync(coachId, cancellationToken);

        if (coachDto is null)
            return NotFound("No Coach Found");

        return coachDto;
    }

    [HttpGet("get-by-username/{coachUserName}")]
    public async Task<ActionResult<CoachDto>> GetByCoachUserName(string coachUserName, CancellationToken cancellationToken)
    {
        CoachDto? coachDto = await _coachRepository.GetByCoachUserNameAsync(coachUserName, cancellationToken);

        if (coachDto is null)
            return NotFound("No coach with this address");

        return coachDto;
    }
}
