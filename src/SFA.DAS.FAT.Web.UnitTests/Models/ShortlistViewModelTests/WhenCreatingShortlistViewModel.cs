using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ShortlistViewModelTests
{
    public class WhenCreatingShortlistViewModel
    {
        [Test, AutoData]
        public void Then_The_Values_Are_Mapped_Correctly(ShortlistItem source)
        {
            var actual = (ShortlistItemViewModel)source;

            actual.Should().BeEquivalentTo(source, options => options
                .Excluding(item => item.ShortlistUserId)
                .Excluding(item => item.Course)
                .Excluding(item => item.Provider));
            actual.Course.Should().BeEquivalentTo((CourseViewModel)source.Course);
            actual.Provider.Should().BeEquivalentTo((ProviderViewModel)source.Provider);
        }

        [Test, AutoData]
        public void Then_Shortlist_Default_Is_Empty_List()
        {
            var actual = new ShortlistsViewModel();

            actual.ShortlistedItems.Should().BeEmpty();
        }

        //[Test, AutoData]
        //public void Then_The_Expiry_Text_Is_Correctly_Set(ShortlistsViewModel model)
        //{
        //    var maxDate = model.ShortlistedItems.Select(c => c.CreatedDate).Max();
        //    model.ExpiryDateText.Should().Be($"We will save your shortlist until {maxDate.AddDays(30):dd MMMM yyyy}.");
        //}

        //[Test, AutoData]
        //public void Then_The_Expiry_Text_Is_Correctly_Set_When_No_Items(ShortlistsViewModel model)
        //{
        //    model.ShortlistedItems = new List<ShortlistItemViewModel>();
        //    model.ExpiryDateText.Should().BeEmpty();
        //}
    }
}
