using System;
using System.Xml;
using System.Xml.Linq;
using AndcultureCode.CSharp.Testing.Constants;
using AndcultureCode.CSharp.Testing.Extensions;
using AndcultureCode.CSharp.Testing.Extensions.Mocks;
using AndcultureCode.CSharp.Testing.Extensions.Mocks.Conductors;
using Moq;
using Nfpa.CodesApi.Business.Conductors.Aspects;
using Nfpa.CodesApi.Business.Core.Interfaces.Providers.Storage;
using Nfpa.CodesApi.Business.Core.Models.Storage;
using Xunit;
using Xunit.Abstractions;
using Shouldly;

namespace Nfpa.CodesApi.Business.Conductors.Tests.Unit.Aspects
{
    [Collection("ConductorUnitTests")]
    public class ImageProcessingConductorTest : ConductorUnitTest
    {
        #region Setup

        public ImageProcessingConductorTest(ITestOutputHelper output) : base(output)
        {
        }

        #endregion Setup

        #region ProcessImageSrcUrls

        [Fact]
        public void ProcessImageSrcUrls_When_Xml_Contains_Images_Then_Returns_XElement_With_New_Value()
        {
            // Arrange
            var sectionBodyDoc = new XmlDocument();
            sectionBodyDoc.Load("Fixtures/Sections/Body/SectionBodyWithImages.xml");
            var sectionBody = XElement.Load(new XmlNodeReader(sectionBodyDoc));
            var presignedUrl = "https://some-fancy-presignedurl";
            var mockStorageProvider = new Mock<IStorageProvider>();
            var mockRemoteAccessDetails = new RemoteAccessDetails
            {
                Url = presignedUrl
            };

            mockStorageProvider.Setup(x => x.GetRemoteAccessDetails(
                It.IsAny<string>(),
                default,
                default,
                default,
                default
                )).ReturnsGivenResult(mockRemoteAccessDetails);

            // Act
            var sut = new ImageProcessingConductor(
                mockStorageProvider.Object,
                null
            );

            var result = sut.ProcessImageSrcUrls(sectionBody);

            // Assert
            result.ShouldNotHaveErrors();
            result.ResultObject.ToString().ShouldContain(presignedUrl);
        }

        [Fact]
        public void ProcessImageSrcUrls_When_StorageProvider_HasErrors_Then_Returns_Original_XElement_WithErrors()
        {
            // Arrange
            var sectionBodyDoc = new XmlDocument();
            sectionBodyDoc.Load("Fixtures/Sections/Body/SectionBodyWithImages.xml");
            var sectionBody = XElement.Load(new XmlNodeReader(sectionBodyDoc));
            var control = XElement.Load(new XmlNodeReader(sectionBodyDoc));
            var mockStorageProvider = new Mock<IStorageProvider>();
            mockStorageProvider.Setup(x => x.GetRemoteAccessDetails(
                It.IsAny<string>(),
                default,
                default,
                default,
                default
                )).ReturnsBasicErrorResult();

            // Act
            var sut = new ImageProcessingConductor(
                mockStorageProvider.Object,
                null
            );

            var result = sut.ProcessImageSrcUrls(sectionBody);

            // Assert
            result.ResultObject.ToString().ShouldBe(control.ToString());
            result.ShouldHaveErrorsFor(ErrorConstants.BASIC_ERROR_KEY);
        }

        [Fact]
        public void ProcessImageSrcUrls_When_SrcAttribute_Is_Not_Found_Then_Returns_Original_XElement_WithErrors()
        {
            // Arrange
            var sectionBodyDoc = new XmlDocument();
            sectionBodyDoc.Load("Fixtures/Sections/Body/SectionBodyMalformedImage.xml");
            var sectionBody = XElement.Load(new XmlNodeReader(sectionBodyDoc));
            var presignedUrl = "https://some-fancy-presignedurl";
            var mockStorageProvider = new Mock<IStorageProvider>();
            var mockRemoteAccessDetails = new RemoteAccessDetails
            {
                Url = presignedUrl
            };

            mockStorageProvider.Setup(x => x.GetRemoteAccessDetails(
                It.IsAny<string>(),
                default,
                default,
                default,
                default
                )).ReturnsGivenResult(mockRemoteAccessDetails);

            // Act
            var sut = new ImageProcessingConductor(
                mockStorageProvider.Object,
                null
            );

            var result = sut.ProcessImageSrcUrls(sectionBody);

            // Assert
            result.ShouldHaveErrorsFor(ImageProcessingConductor.ATTRIBUTE_NOT_FOUND_ERROR_KEY);
        }

        #endregion ProcessImageSrcUrls

    }
}
