using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.SelectTrainingProviderControllerTests;
public class WhenPostingSelectTrainingProviderSubmitViewModel
{
    [Test, MoqAutoData]
    public void And_SubmitViewModel_Is_Valid_Redirects_To_Expected_Action(
        SelectTrainingProviderSubmitViewModel viewModel,
        [Frozen] Mock<IValidator<SelectTrainingProviderSubmitViewModel>> validator,
        [Greedy] SelectTrainingProviderController controller,
        CancellationToken cancellationToken)
    {
        //Arrange
        validator.Setup(x => x.Validate(viewModel)).Returns(new ValidationResult());

        //Act
        var actual = controller.Index(viewModel, cancellationToken);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToActionResult;
        result.Should().NotBeNull();
        result!.ActionName.Should().Be("Index");
        result.ControllerName.Should().Be("SelectTrainingProvider");
    }

    [Test, MoqAutoData]
    public void And_SubmitViewModel_Is_Invalid_Reloads_View(
        SelectTrainingProviderSubmitViewModel viewModel,
        [Frozen] Mock<IValidator<SelectTrainingProviderSubmitViewModel>> validator,
        [Greedy] SelectTrainingProviderController controller,
        CancellationToken cancellationToken)
    {
        //Arrange
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        validator.Setup(x => x.Validate(viewModel))
            .Returns(validationResult);

        //Act
        var actual = controller.Index(viewModel, cancellationToken);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as ViewResult;
        result.Should().NotBeNull();
        var model = result!.Model as SelectTrainingProviderViewModel;

        model!.ShortListItemCount.Should().Be(viewModel.ShortListItemCount);
        model.ShowSearchCrumb.Should().BeTrue();
        model.ShowShortListLink.Should().BeTrue();
    }
}
