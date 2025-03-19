using AutoFixture;
using AutoFixture.Kernel;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models.Filters.Abstract;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;
using static SFA.DAS.FAT.Web.Services.FilterService;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public sealed class FilterServiceTests
{
    [Test, MoqAutoData]
    public void CreateInputFilterSection_ShouldReturnTextBoxFilterSection_WithCorrectValues(
        string id,
        string heading,
        string subHeading,
        string filterFor,
        string inputValue
    )
    {
        var _sut = CreateInputFilterSection(id, heading, subHeading, filterFor, inputValue);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.InstanceOf<TextBoxFilterSectionViewModel>());
            Assert.That(_sut.Id, Is.EqualTo(id));
            Assert.That(_sut.For, Is.EqualTo(filterFor));
            Assert.That(_sut.Heading, Is.EqualTo(heading));
            Assert.That(_sut.SubHeading, Is.EqualTo(subHeading));
            Assert.That(((TextBoxFilterSectionViewModel)_sut).FilterComponentType, Is.EqualTo(FilterComponentType.TextBox));
            Assert.That(((TextBoxFilterSectionViewModel)_sut).InputValue, Is.EqualTo(inputValue));
        });
    }

    [Test, MoqAutoData]
    public void CreateSearchFilterSection_ShouldReturnSearchFilterSection_WithCorrectValues(
        string id,
        string heading,
        string subHeading,
        string filterFor,
        string inputValue
    )
    {
        var _sut = CreateSearchFilterSection(id, heading, subHeading, filterFor, inputValue);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.InstanceOf<SearchFilterSectionViewModel>());
            Assert.That(_sut.Id, Is.EqualTo(id));
            Assert.That(_sut.For, Is.EqualTo(filterFor));
            Assert.That(_sut.Heading, Is.EqualTo(heading));
            Assert.That(_sut.SubHeading, Is.EqualTo(subHeading));
            Assert.That(((SearchFilterSectionViewModel)_sut).FilterComponentType, Is.EqualTo(FilterComponentType.Search));
            Assert.That(((SearchFilterSectionViewModel)_sut).InputValue, Is.EqualTo(inputValue));
        });
    }

    [Test, MoqAutoData]
    public void CreateDropdownFilterSection_ShouldReturnDropdownFilterSection_WithCorrectValues(
        string id,
        string filterFor,
        string heading,
        string subHeading,
        List<FilterItemViewModel> items
    )
    {
        var _sut = CreateDropdownFilterSection(id, filterFor, heading, subHeading, items);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.InstanceOf<DropdownFilterSectionViewModel>());
            Assert.That(_sut.Id, Is.EqualTo(id));
            Assert.That(_sut.For, Is.EqualTo(filterFor));
            Assert.That(_sut.Heading, Is.EqualTo(heading));
            Assert.That(_sut.SubHeading, Is.EqualTo(subHeading));
            Assert.That(((DropdownFilterSectionViewModel)_sut).Items, Is.EquivalentTo(items));
        });
    }

    [Test, MoqAutoData]
    public void CreateCheckboxListFilterSection_ShouldReturnCheckboxListFilterSection_WithCorrectValues(
        string id,
        string filterFor,
        string heading,
        List<FilterItemViewModel> items,
        string linkDisplayText,
        string linkDisplayUrl
    )
    {
        var _sut = CreateCheckboxListFilterSection(
            id,
            filterFor,
            heading,
            null,
            items,
            linkDisplayText,
            linkDisplayUrl
        );

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.InstanceOf<CheckboxListFilterSectionViewModel>());
            Assert.That(_sut.Id, Is.EqualTo(id));
            Assert.That(_sut.For, Is.EqualTo(filterFor));
            Assert.That(_sut.Heading, Is.EqualTo(heading));

            var checkboxListFilterSection = (CheckboxListFilterSectionViewModel)_sut;
            Assert.That(checkboxListFilterSection.Items, Is.EquivalentTo(items));
            Assert.That(checkboxListFilterSection.Link, Is.Not.Null);
            Assert.That(checkboxListFilterSection.Link.DisplayText, Is.EqualTo(linkDisplayText));
            Assert.That(checkboxListFilterSection.Link.Url, Is.EqualTo(linkDisplayUrl));
        });
    }

    [Test, MoqAutoData]
    public void CreateCheckboxListFilterSection_ShouldNotSetLink_WhenLinkTextAndUrlAreEmpty(
        string id,
        string filterFor,
        string heading,
        List<FilterItemViewModel> items
    )
    {
        var _sut = CreateCheckboxListFilterSection(id, filterFor, heading, string.Empty, items, "", "");

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.InstanceOf<CheckboxListFilterSectionViewModel>());
            Assert.That(((CheckboxListFilterSectionViewModel)_sut).Link, Is.Null);
        });
    }

    [Test, MoqAutoData]
    public void CreateAccordionFilterSection_ShouldReturnAccordionFilterSection_WithCorrectChildren(
        string id,
        string sectionFor
    )
    {
        var fixture = CreateFixture();

        var children = fixture.Create<List<FilterSection>>();

        var _sut = CreateAccordionFilterSection(id, sectionFor, children);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.InstanceOf<AccordionFilterSectionViewModel>());
            Assert.That(_sut.Id, Is.EqualTo(id));
            Assert.That(_sut.For, Is.EqualTo(sectionFor));
            Assert.That(((AccordionFilterSectionViewModel)_sut).Children, Is.EquivalentTo(children));
        });
    }

    [Test]
    public void AddSelectedFilter_WithValidStringValue_ShouldAddFilter()
    {
        var _sut = new Dictionary<FilterType, List<string>>();
        var filterType = FilterType.KeyWord;
        var value = "Construction";

        AddSelectedFilter(_sut, filterType, value);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.ContainsKey(filterType), Is.True, "FilterType should exist in the dictionary.");
            Assert.That(_sut[filterType], Has.Count.EqualTo(1), "FilterType should have exactly one value.");
            Assert.That(_sut[filterType][0], Is.EqualTo(value), "FilterType should contain the correct value.");
        });
    }

    [Test]
    public void AddSelectedFilter_WithNullOrWhitespaceString_ShouldNotAddFilter()
    {
        var _sut = new Dictionary<FilterType, List<string>>();
        var filterType = FilterType.KeyWord;
        string nullValue = null;

        AddSelectedFilter(_sut, filterType, "");
        AddSelectedFilter(_sut, filterType, "   ");
        AddSelectedFilter(_sut, filterType, nullValue);

        Assert.That(_sut.ContainsKey(filterType), Is.False, "FilterType should not be added for empty or null values.");
    }

    [Test]
    public void AddSelectedFilter_WithValidList_ShouldAddFilter()
    {
        var _sut = new Dictionary<FilterType, List<string>>();
        var filterType = FilterType.Levels;
        var values = new List<string> { "Level 3", "Level 4" };

        AddSelectedFilter(_sut, filterType, values);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.ContainsKey(filterType), Is.True, "FilterType should exist in the dictionary.");
            Assert.That(_sut[filterType], Is.EquivalentTo(values), "FilterType should contain all values from the list.");
        });
    }

    [Test]
    public void AddSelectedFilter_WithEmptyList_ShouldNotAddFilter()
    {
        var _sut = new Dictionary<FilterType, List<string>>();
        var filterType = FilterType.Levels;
        var emptyValues = new List<string>();

        AddSelectedFilter(_sut, filterType, emptyValues);
        Assert.That(_sut.ContainsKey(filterType), Is.False, "FilterType should not be added if list is empty.");
    }

    [Test]
    public void AddSelectedFilter_WithEmptyValue_ShouldNotAddFilter()
    {
        var _sut = new Dictionary<FilterType, List<string>>();
        var filterType = FilterType.Levels;

        AddSelectedFilter(_sut, filterType, string.Empty);

        Assert.That(_sut.ContainsKey(filterType), Is.False, "FilterType should not be added if list is null.");
    }

    [Test]
    public void GetDistanceFilterValues_ShouldReturnAllValidDistances()
    {
        var selectedDistance = "All";
        var _sut = GetDistanceFilterValues(selectedDistance);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Has.Count.EqualTo(DistanceService.Distances.Count + 1), "Result should contain all distances + 'Across England' option.");
            Assert.That(_sut.Any(i => i.Value == DistanceService.ACROSS_ENGLAND_FILTER_VALUE && i.DisplayText == ACROSS_ENGLAND_FILTER_TEXT), Is.True, "'Across England' option should be present.");
        });
    }

    [Test]
    public void GetDistanceFilterValues_WithSelectedDistance_ShouldMarkCorrectItemAsSelected()
    {
        var selectedDistance = "20";

        var _sut = GetDistanceFilterValues(selectedDistance);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(i => i.Selected && i.Value == selectedDistance), Is.True, $"'{selectedDistance} Miles' should be marked as selected.");
            Assert.That(_sut.Any(i => i.Selected && i.Value == null), Is.False, "'Across England' should not be selected when a distance is provided.");
        });
    }

    [Test]
    public void GetDistanceFilterValues_WithAllSelectedDistance_ShouldSelectAcrossEngland()
    {
        var selectedDistance = "All";

        var _sut = GetDistanceFilterValues(selectedDistance);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(i => i.Selected && i.Value == DistanceService.ACROSS_ENGLAND_FILTER_VALUE), Is.True, "'Across England' should be selected when distance is null.");
            Assert.That(_sut.Any(i => i.Selected && i.Value != DistanceService.ACROSS_ENGLAND_FILTER_VALUE), Is.False, "No specific distance should be selected when distance is null.");
        });
    }

    [Test]
    public void GetDistanceFilterValues_WithInvalidDistance_ShouldNotSelectAnyDistance()
    {
        var selectedDistance = "9999";

        var _sut = GetDistanceFilterValues(selectedDistance);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(i => i.Selected && i.Value == $"{selectedDistance} Miles"), Is.False, "An invalid distance should not be marked as selected.");
            Assert.That(_sut.Any(i => i.Selected && i.Value == DistanceService.ACROSS_ENGLAND_FILTER_VALUE), Is.True, "Across England should be selected if an invalid distance is provided.");
        });
    }

    [Test]
    public void CreateClearFilterSections_WithValidFilters_ShouldReturnCorrectSections()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "London" } },
            { FilterType.Distance, new List<string> { "10" } }
        };

        var _sut = CreateClearFilterSections(selectedFilters, null, [FilterType.Distance]);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Has.Count.EqualTo(1));
            Assert.That(_sut.Any(s => s.FilterType == FilterType.Location), Is.True);
            Assert.That(_sut.Any(s => s.FilterType == FilterType.Distance), Is.False);
        });
    }

    [Test]
    public void CreateClearFilterSections_WithExcludedFilterType_ShouldExcludeIt()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "London" } },
            { FilterType.Distance, new List<string> { "10" } }
        };
        var excludedFilters = new[] { FilterType.Distance };

        var _sut = CreateClearFilterSections(selectedFilters, null, excludedFilters);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Has.Count.EqualTo(1));
            Assert.That(_sut.Any(s => s.FilterType == FilterType.Location), Is.True);
            Assert.That(_sut.Any(s => s.FilterType == FilterType.Distance), Is.False);
        });
    }

    [Test]
    public void CreateClearFilterSections_WithOverrideValueFunctions_ShouldModifyDisplayValue()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Levels, new List<string> { "level-2", "level-3" } }
        };
        var overrideValueFunctions = new Dictionary<FilterType, Func<string, string>>
        {
            { FilterType.Levels, value => $"{value}-altered" }
        };

        var _sut = CreateClearFilterSections(selectedFilters, overrideValueFunctions);

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Has.Count.EqualTo(1), "Expected exactly one filter section.");

            var levelSection = _sut.First(s => s.FilterType == FilterType.Levels);

            Assert.That(levelSection.Items, Has.Count.EqualTo(2), "Expected exactly two filter items.");

            Assert.That(levelSection.Items[0].DisplayText, Is.EqualTo("level-2"));
            Assert.That(levelSection.Items[0].ClearLink, Does.Contain("levels=level-3-altered"), "Level 2 ClearLink should contain 'levels=level-3-altered'.");
            Assert.That(levelSection.Items[0].ClearLink, Does.Not.Contain("levels=level-2-altered"), "Level 2 ClearLink should not contain 'levels=level-2-altered'.");

            Assert.That(levelSection.Items[1].DisplayText, Is.EqualTo("level-3"));
            Assert.That(levelSection.Items[1].ClearLink, Does.Contain("levels=level-2-altered"), "Level 3 ClearLink should contain 'levels=level-2-altered'.");
            Assert.That(levelSection.Items[1].ClearLink, Does.Not.Contain("levels=level-3-altered"), "Level 3 ClearLink should not contain 'levels=level-3-altered'.");
        });
    }

    [Test]
    public void CreateClearFilterSections_WithEmptyFilters_ShouldReturnEmptyList()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();
        var _sut = CreateClearFilterSections(selectedFilters);
        Assert.That(_sut, Is.Empty);
    }

    [Test]
    public void CreateClearFilterSections_WithNullFilters_ShouldReturnEmptyList()
    {
        Dictionary<FilterType, List<string>> selectedFilters = null;
        var result = CreateClearFilterSections(selectedFilters);
        Assert.That(result, Is.Empty, "Result should be an empty list when input is null.");
    }

    [Test]
    public void CreateClearFilterSections_WithFilterHavingNoValues_ShouldNotAddSection()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string>() }
        };

        var _sut = CreateClearFilterSections(selectedFilters);
        Assert.That(_sut, Is.Empty, "Filter sections with no values should not be added.");
    }

    [Test]
    public void GetWorkLocationDistanceDisplayMessage_WithValidLocationAndDistance_ShouldReturnCorrectMessage()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "M60 7RA" } },
            { FilterType.Distance, new List<string> { "10" } }
        };

        var _sut = CreateClearFilterSections(selectedFilters, null, [FilterType.Distance]);

        var locationFilterClearSection = _sut.First(a => a.FilterType == FilterType.Location);
        Assert.That(locationFilterClearSection.Items[0].DisplayText, Is.EqualTo("M60 7RA (within 10 miles)"));
    }

    [Test]
    public void GetWorkLocationDistanceDisplayMessage_WithValidLocationAndNoDistance_ShouldReturnAcrossEngland()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "M60 7RA" } }
        };
        var _sut = CreateClearFilterSections(selectedFilters);
        var locationFilterClearSection = _sut.First(a => a.FilterType == FilterType.Location);
        Assert.That(locationFilterClearSection.Items[0].DisplayText, Is.EqualTo("M60 7RA (Across England)"), "When distance is missing, it should show 'Across England'.");
    }

    [Test]
    public void GetWorkLocationDistanceDisplayMessage_WithValidLocationAndInvalidDistance_ShouldReturnAcrossEngland()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "M60 7RA" } },
            { FilterType.Distance, new List<string> { "9999" } }
        };

        var _sut = CreateClearFilterSections(selectedFilters, null, [FilterType.Distance]);
        var locationFilterClearSection = _sut.First(a => a.FilterType == FilterType.Location);
        Assert.That(locationFilterClearSection.Items[0].DisplayText, Is.EqualTo("M60 7RA (Across England)"), "Invalid distances should default to 'Across England'.");
    }

    [Test]
    public void GetWorkLocationDistanceDisplayMessage_WithEmptyLocation_ShouldReturnEmpty()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "" } }
        };

        var _sut = CreateClearFilterSections(selectedFilters);
        var locationFilterClearSection = _sut.First(a => a.FilterType == FilterType.Location);
        Assert.That(locationFilterClearSection.Items[0].DisplayText, Is.Empty, "Empty location should return null.");
    }

    [Test]
    public void GetWorkLocationDistanceDisplayMessage_WithWhitespaceLocation_ShouldReturnNull()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>
        {
            { FilterType.Location, new List<string> { "   " } }
        };

        var _sut = CreateClearFilterSections(selectedFilters);
        var locationFilterClearSection = _sut.First(a => a.FilterType == FilterType.Location);
        Assert.That(locationFilterClearSection.Items[0].DisplayText, Is.Empty);
    }

    [Test]
    public void GetWorkLocationDistanceDisplayMessage_WithNoFilters_ShouldReturnNull()
    {
        var selectedFilters = new Dictionary<FilterType, List<string>>();
        var _sut = CreateClearFilterSections(selectedFilters);
        var locationFilterClearSection = _sut.FirstOrDefault(a => a.FilterType == FilterType.Location);
        Assert.That(locationFilterClearSection, Is.Null, "Empty filter dictionary should return null.");
    }

    private static Fixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customizations.Add(
            new TypeRelay(
                typeof(FilterSection),
                typeof(AccordionFilterSectionViewModel)
            )
        );

        return fixture;
    }
}
