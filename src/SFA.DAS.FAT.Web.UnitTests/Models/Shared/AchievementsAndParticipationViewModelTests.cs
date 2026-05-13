using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.Shared;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Shared;

public class AchievementsAndParticipationViewModelTests
{
    [Test]
    public void AchievementRatePresent_QarContainsAchievementRate_ReturnsTrue()
    {
        var sut = new AchievementsAndParticipationViewModel
        {
            Qar = new QarModel { AchievementRate = "95.5" }
        };

        Assert.That(sut.AchievementRatePresent, Is.True);
        Assert.That(sut.ShowNationalAchievementRate, Is.True);
    }

    [Test]
    public void AchievementRatePresent_QarIsNull_ReturnsFalse()
    {
        var sut = new AchievementsAndParticipationViewModel();

        Assert.That(sut.AchievementRatePresent, Is.False);
        Assert.That(sut.ShowNationalAchievementRate, Is.False);
    }

    [Test]
    public void Properties_BackingValuesAreNull_ReturnsExpectedEmptyStrings()
    {
        var sut = new AchievementsAndParticipationViewModel();

        Assert.Multiple(() =>
        {
            Assert.That(sut.AchievementRate, Is.EqualTo(string.Empty));
            Assert.That(sut.AchievementInformation, Is.EqualTo(string.Empty));
            Assert.That(sut.NationalAchievementRate, Is.EqualTo(string.Empty));
            Assert.That(sut.ParticipationCount, Is.EqualTo(string.Empty));
            Assert.That(sut.ParticipationMessage, Is.EqualTo(string.Empty));
            Assert.That(sut.AchievementNoDataMessage, Is.EqualTo("There is not enough data to show the achievement rate for this course. This may be because a small number of apprentices completed this course, or this is a new course for this provider."));
        });
    }
}
