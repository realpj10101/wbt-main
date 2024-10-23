using api.DTOs.Team;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Team;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace api.Repositories.Team;

public class TeamRepository : ITeamRepository
{
    #region db and token

    readonly IMongoCollection<RootModel>? _collection;

    public TeamRepository(IMongoClient client, MongoDbSettings dbSettings)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionTeams);
    }

    #endregion

    public async Task<PagedList<RootModel>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        IMongoQueryable<RootModel> query = _collection.AsQueryable();

        return await PagedList<RootModel>.CreatePagedListAsync(query, paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);
    }

    public async Task<TeamDto?> GetByIdAsync(string teamId, CancellationToken cancellationToken)
    {
        RootModel team = await _collection.Find<RootModel>(team =>
                team.Id.ToString() == teamId).FirstOrDefaultAsync(cancellationToken);

        if (teamId.ToString() is not null)
        {
            return TeamMappers.ConvertRootModelToTeamDto(team);
        }

        return null;
    }

    public async Task<TeamDto?> GetByUserNameAsync(string teamUserName, CancellationToken cancellationToken)
    {
        RootModel team = await _collection.Find<RootModel>(team =>
                team.NormalizedUserName ==teamUserName).FirstOrDefaultAsync(cancellationToken);

        if (team.Id.ToString() is not null)
        {
            return TeamMappers.ConvertRootModelToTeamDto(team);
        }

        return null;
    }
}