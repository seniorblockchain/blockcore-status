﻿using blockcore.status.Services.Contracts;
using blockcore.status.Services.Contracts.Admin;
using blockcore.status.ViewModels.Admin;
using blockcore.status.ViewModels.Github;
using Microsoft.AspNetCore.Mvc;

namespace blockcore.status.Areas.Admin.ViewComponents;

public class GithubOrganizationInfoViewComponent : ViewComponent
{
    private readonly IGithubService _github;

    public GithubOrganizationInfoViewComponent(IGithubService github)
    {
        _github = github;
    }


    public async Task<IViewComponentResult> InvokeAsync(string OrgName)
    {
        var orgInfo = await _github.GetOrganizationByName(OrgName);
        var reposlist = orgInfo.GithubRepository.Select(c => new RepositoryInfoViewModel()
        {
            LastVersion = "-",
            Name = c.Name,
            RepositoryURL = c.HtmlUrl
        }).ToList();
 
 

        var org = await _github.GetOrganizationInfo(OrgName).ConfigureAwait(false);
        if (org == null)
        {
            return View("~/Areas/Admin/Views/Shared/Components/Github/OrganizationInfo.cshtml", new OrganizationInfoViewModel());
        }
        return View("~/Areas/Admin/Views/Shared/Components/Github/OrganizationInfo.cshtml",
            new OrganizationInfoViewModel
            {
                Name = org.Name,
                Description = org.Description,
                AvatarUrl = org.AvatarUrl,
                Blog = org.Blog,
                Login = org.Login,
                Apiurl = org.Url,
                Repositories = reposlist
            });
    }

}