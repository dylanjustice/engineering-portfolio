using LMS.Business.Core.Models.Errors;
using LMS.Core.Models.Entities.Content;
using LMS.Presentation.Web.Controllers.Api.V1.Staff.Content;
using LMS.Presentation.Web.Models.Dtos.Content;
using LMS.Testing.Extensions;
using Refactored.Web.Tests.Extensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Refactored.Web.Tests.Integration.Controllers.Api.V1.Staff.Content
{
    [Collection("ControllerIntegration")]
    public class ContentAreaVersionComponentsPasteControllerTest: ControllerTest<ContentAreaVersionComponentsPasteController>, IDisposable
    {
        #region Setup

        private class ArrangeContentAreaVersionComponentData
        {
            public ContentArea                  ContentArea        { get; set; }
            public ContentAreaVersion           ContentAreaVersion { get; set; }
            public ContentAreaVersionComponent  VersionComponent   { get; set; }
        }

        private ArrangeContentAreaVersionComponentData ArrangeContentAreaVersionComponent()
        {
            var contentAreaVersionComponent = SeedContentAreaVersionComponent();
            return new ArrangeContentAreaVersionComponentData
            {
                ContentArea        = contentAreaVersionComponent.ContentAreaVersion.ContentArea,
                ContentAreaVersion = contentAreaVersionComponent.ContentAreaVersion,
                VersionComponent   = contentAreaVersionComponent,
            };
        }

        public ContentAreaVersionComponentsPasteControllerTest(
            ControllerFixture fixture,
            ITestOutputHelper output)
        : base(fixture, output)
        {
        }

        #endregion

        #region Teardown

        public void Dispose()
        {

        }

        #endregion

        #region HTTP POST

        #region Paste

        [Fact]
        public void Paste_When_Invalid_Id_Then_Returns_Bad_Request()
        {
            // Arrange
            MockAuthenticatedStaffUser();
            var arrangeData = ArrangeContentAreaVersionComponent();
            var component2 = Create<ContentAreaVersionComponent>(
                e => e.ContentAreaVersionId = arrangeData.ContentAreaVersion.Id,
                e => e.ComponentId          = arrangeData.VersionComponent.ComponentId,
                e => e.ParentId             = arrangeData.VersionComponent.ParentId,
                e => e.ContainerName        = arrangeData.VersionComponent.ContainerName,
                e => e.DisplaySequence      = 2
            );
            var course = SeedCourse();
            var dto = new ContentAreaVersionComponentPasteDto()
            {
                SourceId = component2.Id,
                CourseId = course.Id
            };

            // Act
            var result = Sut.Post(-1, dto).AsBadRequest<Result<ContentAreaVersionComponentDto>>();

            // Assert
            result.ShouldHaveErrors();
            result.ResultObject.ShouldBeNull();
        }

        [Fact]
        public void Paste_When_Invalid_Course_Then_Returns_BadRequest()
        {
            //Arrange
            MockAuthenticatedStaffUser();
            var lesson = SeedLesson();
            var arrangeData = ArrangeContentAreaVersionComponent();
            lesson.ContentAreaId = arrangeData.ContentArea.Id;

            var targetComponent = Create<ContentAreaVersionComponent>(
                e => e.ContentAreaVersionId = arrangeData.ContentAreaVersion.Id,
                e => e.ComponentId          = arrangeData.VersionComponent.ComponentId,
                e => e.ParentId             = arrangeData.VersionComponent.ParentId,
                e => e.ContainerName        = arrangeData.VersionComponent.ContainerName,
                e => e.DisplaySequence      = 2
            );

            var dto = new ContentAreaVersionComponentPasteDto()
            {
                SourceId = targetComponent.Id,
                CourseId = -1
            };

            //Act
            var result = Sut.Post(targetComponent.Id, dto).AsBadRequest<Result<ContentAreaVersionComponentDto>>();

            //Assert
            result.ShouldHaveErrors();
            result.ResultObject.ShouldBeNull();
        }

        [Fact]
        public void Paste_When_SourceId_Is_Invalid_Then_Returns_BadRequest()
        {
            //Arrange
            MockAuthenticatedStaffUser();
            var lesson = SeedLesson();
            var arrangeData = ArrangeContentAreaVersionComponent();
            lesson.ContentAreaId = arrangeData.ContentArea.Id;

            var targetComponent = Create<ContentAreaVersionComponent>(
                e => e.ContentAreaVersionId = arrangeData.ContentAreaVersion.Id,
                e => e.ComponentId          = arrangeData.VersionComponent.ComponentId,
                e => e.ParentId             = arrangeData.VersionComponent.ParentId,
                e => e.ContainerName        = arrangeData.VersionComponent.ContainerName,
                e => e.DisplaySequence      = 2
            );

            var course = SeedCourse();
            var dto = new ContentAreaVersionComponentPasteDto()
            {
                CourseId = course.Id,
                SourceId = -1
            };

            //Act
            var result = Sut.Post(targetComponent.Id, dto).AsBadRequest<Result<ContentAreaVersionComponentDto>>();

            //Assert
            result.ShouldHaveErrors();
            result.ResultObject.ShouldBeNull();
        }

        [Fact]
        public void Paste_When_Valid_Request_Returns_Created()
        {
            // Arrange
            MockAuthenticatedStaffUser();
            var arrangeTargetComponentData = ArrangeContentAreaVersionComponent();
            var targetComponent = Create<ContentAreaVersionComponent>(
                e => e.ContentAreaVersionId = arrangeTargetComponentData.ContentAreaVersion.Id,
                e => e.ComponentId          = arrangeTargetComponentData.VersionComponent.ComponentId,
                e => e.ParentId             = arrangeTargetComponentData.VersionComponent.ParentId,
                e => e.ContainerName        = arrangeTargetComponentData.VersionComponent.ContainerName,
                e => e.DisplaySequence      = 5
            );

            var arrangeSourceComponentData = ArrangeContentAreaVersionComponent();
            var lesson = SeedLesson();
            lesson.ContentAreaId = arrangeSourceComponentData.ContentArea.Id;
            var sourceComponent = Create<ContentAreaVersionComponent>(
                e => e.ContentAreaVersionId = arrangeSourceComponentData.ContentAreaVersion.Id,
                e => e.ComponentId          = arrangeSourceComponentData.VersionComponent.ComponentId,
                e => e.ParentId             = arrangeSourceComponentData.VersionComponent.ParentId,
                e => e.ContainerName        = arrangeSourceComponentData.VersionComponent.ContainerName,
                e => e.DisplaySequence      = 2
            );
            var dto = new ContentAreaVersionComponentPasteDto()
            {
                CourseId = lesson.Day.CourseId,
                SourceId = sourceComponent.Id
            };

            // Act
            var result = Sut.Post(targetComponent.Id, dto).AsCreated<Result<ContentAreaVersionComponentDto>>();

            // Assert
            result.ShouldNotHaveErrors();
            result.ResultObject.ShouldNotBeNull();
            result.ResultObject.DisplaySequence.ShouldBe(sourceComponent.DisplaySequence);
            result.ResultObject.ParentId.ShouldBe(sourceComponent.ParentId);
            result.ResultObject.ContainerName.ShouldBe(sourceComponent.ContainerName);
            result.ResultObject.ComponentId.ShouldBe(sourceComponent.ComponentId);
            result.ResultObject.ContentAreaVersionId.ShouldBe(targetComponent.ContentAreaVersionId);


            Reload(targetComponent);
            targetComponent.DeletedOn.ShouldNotBeNull();
        }

        #endregion


        #endregion
    }
}
