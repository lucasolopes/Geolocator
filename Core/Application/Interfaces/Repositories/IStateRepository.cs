﻿using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStateRepository : IRepository<State>
{
    Task<List<State>> GetAllAsync();
    Task<State> GetByIdAsync(long id);
    Task<List<State>> GetByIdsWithRelationshipsAsync(List<long> ids);
}
