using System.IO.Pipelines;
using api.Extensions;
using api.Helpers;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace api.Repositories;
// 
public class CoachRepository : ICoachRepository
{
    #region db and token
    IMongoCollection<RootModel>? _collection;

    public CoachRepository(IMongoClient client, MongoDbSettings dbSettings)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionCoaches);
        // _tokenService = tokenService;
    }

    #endregion

    public async Task<PagedList<RootModel>> GetAllCoachsAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        IMongoQueryable<RootModel> query = _collection.AsQueryable();

        return await PagedList<RootModel>.CreatePagedListAsync(query, paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);
    }

    public async Task<CoachDto?> GetByCoachIdAsync(string coachId, CancellationToken cancellationToken)
    {
        RootModel coach = await _collection.Find<RootModel>(coach =>
            coach.Id.ToString() == coachId).FirstOrDefaultAsync(cancellationToken);

        if (coach.Id.ToString() is not null)
        {
            return CoachMappers.ConvertRootModelToCoachDto(coach);
        }

        return null;
    }

    public async Task<CoachDto?> GetByCoachUserNameAsync(string coachUserName, CancellationToken cancellationToken)
    {
        RootModel coach = await _collection.Find<RootModel>(coach =>
            coach.NormalizedUserName == coachUserName).FirstOrDefaultAsync(cancellationToken);

        if (coach.Id.ToString() is not null)
        {
            return CoachMappers.ConvertRootModelToCoachDto(coach);
        }

        return null;
    }
}
