﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HostOcean.Domain.Entities;

namespace HostOcean.Application.Interfaces.Persistence
{
    public interface IUserQueueRepository: IRepository<UserQueue>
    {
        IEnumerable<TResult> GetPredicatedAsync<TResult>(
            Expression<Func<UserQueue, bool>> predicate,
            Expression<Func<UserQueue, TResult>> selector);
    }
}
