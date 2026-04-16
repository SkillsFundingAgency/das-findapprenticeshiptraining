using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.CourseProviders;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests;

public class WhenCreatingTrainingOptionsTableViewModel
{
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("  ", false)]
    [TestCase("Coventry", true)]
    public void HasLocation_LocationValueProvided_ReturnsExpectedValue(string location, bool expectedHasLocation)
    {
        var sut = new TrainingOptionsTableViewModel
        {
            Location = location,
        };

        sut.HasLocation.Should().Be(expectedHasLocation);
    }

    [Test]
    public void BlockReleaseRow_PropertiesSet_ReturnsMappedRowViewModel()
    {
        var blockReleaseLocations = new List<LocationModel>
        {
            new() { AddressLine1 = "1 Street", Postcode = "AB1 2CD" },
        };

        var sut = new TrainingOptionsTableViewModel
        {
            Location = "Coventry",
            ShowBlockReleaseOption = true,
            HasMultipleBlockReleaseLocations = true,
            ClosestBlockReleaseLocationDistanceDisplay = "10.5",
            ClosestBlockReleaseLocation = blockReleaseLocations[0],
            BlockReleaseLocations = blockReleaseLocations,
        };

        var result = sut.BlockReleaseRow;

        result.ShowReleaseLocationsOption.Should().BeTrue();
        result.TrainingOptionTitle.Should().Be("Block release");
        result.Hint.Should().Be(TrainingOptionsTableViewModel.BlockReleaseHint);
        result.HasLocation.Should().BeTrue();
        result.HasMultipleLocations.Should().BeTrue();
        result.MultipleLocationsMessage.Should().Be(TrainingOptionsTableViewModel.BlockReleaseMultipleLocations);
        result.ViewAllLocationsText.Should().Be(TrainingOptionsTableViewModel.ViewAllBlockReleaseLocations);
        result.ClosestLocationDistanceDisplay.Should().Be("10.5");
        result.ClosestLocation.Should().BeSameAs(blockReleaseLocations[0]);
        result.Locations.Should().BeSameAs(blockReleaseLocations);
    }

    [Test]
    public void DayReleaseRow_PropertiesSet_ReturnsMappedRowViewModel()
    {
        var dayReleaseLocations = new List<LocationModel>
        {
            new() { AddressLine1 = "2 Road", Postcode = "XY9 8ZT" },
        };

        var sut = new TrainingOptionsTableViewModel
        {
            Location = "Coventry",
            ShowDayReleaseOption = true,
            HasMultipleDayReleaseLocations = true,
            ClosestDayReleaseLocationDistanceDisplay = "7.2",
            ClosestDayReleaseLocation = dayReleaseLocations[0],
            DayReleaseLocations = dayReleaseLocations,
        };

        var result = sut.DayReleaseRow;

        result.ShowReleaseLocationsOption.Should().BeTrue();
        result.TrainingOptionTitle.Should().Be("Day release");
        result.Hint.Should().Be(TrainingOptionsTableViewModel.DayReleaseHint);
        result.HasLocation.Should().BeTrue();
        result.HasMultipleLocations.Should().BeTrue();
        result.MultipleLocationsMessage.Should().Be(TrainingOptionsTableViewModel.DayReleaseMultipleLocations);
        result.ViewAllLocationsText.Should().Be(TrainingOptionsTableViewModel.ViewAllDayReleaseLocations);
        result.ClosestLocationDistanceDisplay.Should().Be("7.2");
        result.ClosestLocation.Should().BeSameAs(dayReleaseLocations[0]);
        result.Locations.Should().BeSameAs(dayReleaseLocations);
    }
}
