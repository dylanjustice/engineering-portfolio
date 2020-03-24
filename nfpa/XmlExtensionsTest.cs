using System.Xml;
using System.Xml.Linq;
using Nfpa.CodesApi.Testing.Tests;
using Xunit;
using Xunit.Abstractions;
using Nfpa.CodesApi.Business.Core.Extensions;
using AndcultureCode.CSharp.Testing.Extensions;
using Shouldly;
using Nfpa.CodesApi.Business.Core.Constants.Xml;
using System.Linq;
using System;

namespace Nfpa.CodesApi.Business.Core.Tests.Unit.Extensions
{
    public class XmlExtensionsTest : CodesApiUnitTest
    {

        #region Setup

        public XmlExtensionsTest(ITestOutputHelper output) : base(output)
        {
        }

        #endregion Setup

        #region ReplaceImgSrcAttributeWithAbsoluteUrl

        [Fact]
        public void ReplaceImgSrcAttributeWithAbsoluteUrl_When_Img_Found_Then_Returns_With_Updated_Src()
        {
            // Arrange
            var oldPath = "images";
            var newPath = "assets/publications/1/images";
            var fixture = new XmlDocument();
            fixture.Load("Fixtures/Sections/SectionBodyWithImage.xml");
            var bodyAsXml = XElement.Load(new XmlNodeReader(fixture));
            var control = XElement.Load(new XmlNodeReader(fixture));

            // Act
            bodyAsXml.ReplaceImgSrcAttributeWithAbsoluteUrl(oldPath, newPath);

            // Assert
            bodyAsXml.ToString().ShouldNotBe(control.ToString());
            bodyAsXml.ToString().ShouldContain(newPath);
        }

        [Fact]
        public void ReplaceImgSrcAttributeWithAbsoluteUrl_When_Img_Not_Found_Then_Returns_Without_Changes()
        {
            // Arrange
            var oldPath = "images";
            var newPath = "assets/publications/1/images";
            var fixture = new XmlDocument();
            fixture.Load("Fixtures/Sections/SectionBodyWithoutImage.xml");
            var bodyAsXml = XElement.Load(new XmlNodeReader(fixture));
            var control = XElement.Load(new XmlNodeReader(fixture));

            // Act
            bodyAsXml = bodyAsXml.ReplaceImgSrcAttributeWithAbsoluteUrl(oldPath, newPath);

            // Assert
            bodyAsXml.ToString().ShouldBe(control.ToString());
        }

        [Fact]
        public void ReplaceImgSrcAttributeWithAbsoluteUrl_When_Body_Null_Then_Returns_Null()
        {
            // Arrange
            XElement bodyAsXml = null;

            // Act
            bodyAsXml.ReplaceImgSrcAttributeWithAbsoluteUrl("replace", "replaceWith");

            // Assert
            bodyAsXml.ShouldBeNull();
        }

        #endregion ReplaceImgSrcAttributeWithAbsoluteUrl

        #region GetImages

        [Fact]
        public void GetImages_When_SectionBody_Contains_Image_Then_Returns_One_XElement()
        {
            // Arrange

            var fixture = new XmlDocument();
            fixture.Load("Fixtures/Sections/SectionBodyWithImage.xml");
            var xml = XElement.Load(new XmlNodeReader(fixture));

            // Act
            var result = xml.GetImages();

            // Assert
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void GetImages_When_No_Images_Then_Returns_Empty()
        {
            // Arrange
            var root = new XElement(PublicationElements.BODY);

            // Act
            var result = root.GetImages();

            // Assert
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetImages_When_Body_Is_Null_Then_Returns_Default()
        {
            // Arrange
            XElement root = null;

            // Act
            var result = root.GetImages();

            // Assert
            result.ShouldBeNull();
        }

        #endregion GetImages

        #region AddParentNodeAttribute

        [Fact]
        public void AddParentNodeAttribute_When_Body_Is_Null_Then_Returns_Default()
        {
            // Arrange
            XElement root = null;

            // Act
            var result = root.AddParentNodeAttributes();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void AddParentNodeAttribute_When_Body_Is_Empty_Returns_Body()
        {
            // Arrange
            var root = new XElement(PublicationElements.BODY);

            // Act
            var result = root.AddParentNodeAttributes();

            // Assert
            result.ShouldBe(root);
        }

        [Fact]
        public void AddParentNodeAttribute_When_Body_Has_Descendants_Then_Returns_Body_And_Descendants_With_ParentNode_Attribute()
        {
            // Arrange
            var root = new XElement(PublicationElements.BODY,
                new XElement(PublicationElements.SECTION)
            );

            // Act
            var result = root.AddParentNodeAttributes();

            // Assert
            result.Descendants()
                .FirstOrDefault()
                .Attribute(PublicationElementAttributes.PARENT_NODE)
                .Value.ShouldBe(PublicationElements.BODY);
        }

        #endregion AddParentNodeAttribute

    }
}