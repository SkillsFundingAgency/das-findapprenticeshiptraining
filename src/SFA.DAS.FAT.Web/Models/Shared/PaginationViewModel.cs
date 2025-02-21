using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.FAT.Web.Models.Shared;

public sealed class PaginationViewModel
{
    public const string PreviousPageTitle = "« Previous";

    public const string NextPageTitle = "Next »";

    private const string PageNumberQueryName = "PageNumber";

    public List<PageLink> Pages { get; } = [];

    private readonly List<ValueTuple<string, string>> _queryParams;

    private readonly string _routeName;

    private readonly IUrlHelper _urlHelper;

    public PaginationViewModel(
        int pageNumber, 
        int totalCount, 
        int pageSize, 
        IUrlHelper urlHelper, 
        string routeName,
        List<ValueTuple<string, string>> queryParams
    )
    {
        _routeName = routeName;
        _queryParams = queryParams;
        _urlHelper = urlHelper;

        int currentPage = 1;

        var queryPageNumber = queryParams.FirstOrDefault(a => a.Item1 == PageNumberQueryName);

        if (!string.IsNullOrWhiteSpace(queryPageNumber.Item2) && int.TryParse(queryPageNumber.Item2, out var _currentPage))
        {
            currentPage = _currentPage;
        }

        if (totalCount == 0)
        {
            return;
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalPages <= 1 || currentPage > totalPages)
        {
            return;
        }

        var (startPage, endPage) = GetPageRange(currentPage, totalCount, pageSize);

        AddPreviousLinkIfApplicable(totalPages, currentPage);

        AddPageLinks(currentPage, startPage, endPage);

        AddNextLinkIfApplicable(totalPages, currentPage);
    }

    private void AddPreviousLinkIfApplicable(int totalPages, int currentPage)
    {
        if (currentPage > 1 && totalPages > 1)
        {
            Pages.Add(new(PreviousPageTitle, GetPageLink(currentPage - 1)));
        }
    }

    private void AddPageLinks(int currentPage, int startPage, int endPage)
    {
        for (int pageNumber = startPage; pageNumber <= endPage; pageNumber++)
        {
            string? pageUrl = pageNumber == currentPage ? null : GetPageLink(pageNumber);
            Pages.Add(new(pageNumber.ToString(), pageUrl));
        }
    }

    private void AddNextLinkIfApplicable(int totalPages, int endPage)
    {
        if (endPage < totalPages) Pages.Add(new(NextPageTitle, GetPageLink(endPage + 1)));
    }

    private string GetPageLink(int pageNumber)
    {
        int index = _queryParams.FindIndex(q => q.Item1 == PageNumberQueryName);

        if (index >= 0)
        {
            _queryParams[index] = (PageNumberQueryName, pageNumber.ToString());
        }
        else
        {
            _queryParams.Add((PageNumberQueryName, pageNumber.ToString()));
        }

        var queryString = string.Join("&", _queryParams.Select(q => 
                $"{q.Item1}={Uri.EscapeDataString(q.Item2)}"
            )
        );

        return $"{_urlHelper.RouteUrl(_routeName)}?{queryString}";
    }

    static (int startPage, int endPage) GetPageRange(int currentPage, int totalRecords, int pageSize)
    {
        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        int startPage = Math.Max(1, currentPage - 2);

        int endPage = Math.Min(totalPages, currentPage + 3);

        if (endPage - startPage < 5)
        {
            if (startPage == 1)
            {
                endPage = Math.Min(totalPages, startPage + 5);
            }
            else if (endPage == totalPages)
            {
                startPage = Math.Max(1, endPage - 5);
            }
        }

        if ((totalPages > 6) && (endPage - startPage < 5))
        {
            if (startPage == 1)
            {
                endPage = startPage + 5;
            }
            else if (endPage == totalPages)
            {
                startPage = endPage - 5;
            }
        }

        return (startPage, endPage);
    }
}

public record struct PageLink(string Title, string? Url)
{
    public bool HasLink => !string.IsNullOrWhiteSpace(Url);
}
