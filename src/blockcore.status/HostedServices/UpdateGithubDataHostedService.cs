﻿using blockcore.status.Services.Contracts;

namespace blockcore.status.HostedServices;

public class UpdateGithubDataHostedService : BackgroundService
{
    private readonly ILogger<UpdateGithubDataHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    public UpdateGithubDataHostedService(IServiceProvider serviceProvider, ILogger<UpdateGithubDataHostedService> logger )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _github = scope.ServiceProvider.GetRequiredService<IGithubService>();

                var orgs = await _github.GetAllOrganizationFromDB();
                foreach (var org in orgs)
                {
                    await _github.UpdateOrganizationInDB(org.Login);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    await _github.UpdateRepositoriesInDB(org.GithubOrganizationId);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    await _github.UpdateLatestRepositoriesReleaseInDB(org.Login);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                }

                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }
    }
}
