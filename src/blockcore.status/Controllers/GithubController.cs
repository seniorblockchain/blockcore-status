﻿using blockcore.status.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace blockcore.status.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GithubController : Controller
{
    private readonly IGithubService _github;

    public GithubController(IGithubService github)
    {
        _github = github ?? throw new ArgumentNullException(nameof(github));
    }

    [HttpGet("[action]/{name}")]
    public async Task<IActionResult> GetOrganizationInfo(string name)
    {
        var org = await _github.GetOrganizationByName(name, false);
        if (org == null)
        {
            return NotFound();
        }
        try
        {
            return Ok(org);
        }
        catch (Exception ex)
        {

            return Ok(ex.Message);
        }

    }

    [HttpGet("[action]/{owner}")]
    public async Task<IActionResult> GetRepositories(string owner, int page = 1)
    {
        var orgInfo = await _github.GetOrganizationByName(owner, false);

        var repos = await _github.GetAllRepositoriesFromDB(orgInfo.GithubOrganizationId, page);

        if (repos == null)
        {
            return NotFound();
        }
        return Ok(repos);
    }


    [HttpGet("[action]/{owner}/{name}")]
    public async Task<IActionResult> GetRepositoryInfo(string owner, string name)
    {
        var repo = await _github.GetRepositoryByNameFromDB(owner, name);
        if (repo == null)
        {
            return NotFound();
        }
        return Ok(repo);
    }



    [HttpGet("[action]/{owner}/{name}")]
    public async Task<IActionResult> GetLatestRepositoryRelease(string owner, string name)
    {
        var releases = await _github.GetLatestRepositoryReleaseFromDB(owner, name);
        if (releases == null)
        {
            return NotFound();
        }
        var lastTag = releases;
        return Ok(lastTag);
    }
}
