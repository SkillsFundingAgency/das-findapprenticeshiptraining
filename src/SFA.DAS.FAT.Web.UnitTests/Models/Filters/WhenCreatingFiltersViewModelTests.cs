using NUnit.Framework;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Filters;

public sealed class WhenCreatingFiltersViewModelTests
{
    [Test]
    public void Then_Show_Filter_Options_Should_Be_True_When_Clear_Filter_Sections_Is_Not_Empty()
    {
        var clearFilterSections = new List<ClearFilterSectionViewModel>
        {
            new ClearFilterSectionViewModel { FilterType = FilterFactory.FilterType.Categories, Title = "Category", Items = new List<ClearFilterItemViewModel>() }
        };

        var _sut = new FiltersViewModel
        {
            Route = RouteNames.Courses,
            ClearFilterSections = clearFilterSections
        };

        Assert.That(_sut.ShowFilterOptions, Is.True);
    }

    [Test]
    public void Then_Show_Filter_Options_Should_Be_False_When_Clear_Filter_Sections_Is_Not_Empty()
    {
        var clearFilterSections = new List<ClearFilterSectionViewModel>();

        var _sut = new FiltersViewModel
        {
            Route = RouteNames.Courses,
            ClearFilterSections = clearFilterSections
        };

        Assert.That(_sut.ShowFilterOptions, Is.False);
    }
}
