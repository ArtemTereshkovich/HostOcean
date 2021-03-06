﻿using System.Threading.Tasks;

namespace HostOcean.Application.Interfaces.Persistence
{
    public interface IHostOceanDataBaseContextInitializer
    {
        Task SeedDataBase();
        Task InitializeMigration();
    }
}