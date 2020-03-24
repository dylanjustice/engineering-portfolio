using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AndcultureCode.CSharp.Core.Interfaces.Conductors;
using AndcultureCode.CSharp.Core.Models;
using AndcultureCode.CSharp.Testing.Constants;
using AndcultureCode.CSharp.Testing.Extensions;
using AndcultureCode.CSharp.Testing.Extensions.Mocks;
using AndcultureCode.CSharp.Testing.Extensions.Mocks.Conductors;
using Bogus;
using Moq;
using Nfpa.CodesApi.Business.Core.Constants.Xml;
using Nfpa.CodesApi.Business.Core.Models.Entities.Chapters;
using Nfpa.CodesApi.Business.Core.Models.Entities.Publications;
using Nfpa.CodesApi.Business.Core.Models.Entities.Sections;
using Nfpa.CodesApi.Core.Models.Entities.Articles;
using Nfpa.CodesApi.Core.Models.Entities.Parts;
using Nfpa.CodesApi.Presentation.Web.Controllers.Api.V1.Publications.Chapters.Articles.Sections;
using Nfpa.CodesApi.Presentation.Web.Models.Dtos.Sections;
using Nfpa.CodesApi.Presentation.Web.Tests.Extensions;
using Nfpa.CodesApi.Testing.Factories.Models.Entities.Sections;
using Nfpa.CodesApi.Tests.Presentation.Web.Tests.Integration.Controllers;
using Nfpa.CodesApi.Tests.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Nfpa.CodesApi.Presentation.Web.Tests.Integration.Controllers.Api.V1.Publications.Chapters.Articles.Sections
{
    [Collection("ControllerIntegration")]
    public class SectionsControllerTest : ControllerTest<SectionsController>, IDisposable
    {
        #region Setup

        public SectionsControllerTest(
            ControllerFixture fixture,
            ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        #endregion Setup

        #region TestData

        /// <summary>
        /// Use this method to build out test data to not be matched on.
        /// (i.e. For ensuring that the filters don't match on false positives.)
        /// Creates this structure.
        /// Publication > Chapter > Article > Part > Section (2) > Subsection (2) > Subsection (2)
        /// </summary>
        private void CreateSecondaryTestData()
        {
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);

            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );

            var part = Create<Part>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            var sections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                )
            };

            var subsections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = sections[0].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = sections[0].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = sections[1].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = sections[1].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                )
            };

            var subSubsections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[0].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[0].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[1].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[1].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[2].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[2].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[3].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.ParentId = subsections[3].Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                )
            };
        }

        #endregion TestData

        #region Get

        [Fact]
        public void Get_When_SectionRead_HasErrors_Then_Returns_InternalError()
        {
            // Arrange
            var mockSectionConductor = new Mock<IRepositoryConductor<Section>>();
            mockSectionConductor.Setup(m => m.FindById(It.IsAny<long>()))
                                .ReturnsBasicErrorResult();
            RegisterDep(mockSectionConductor);

            // Act
            var result = Sut.Get(0, 0, 0, 0, 0).AsInternalError<Result<SectionDto>>();

            // Assert
            result.ShouldHaveErrorsFor(ErrorConstants.BASIC_ERROR_KEY);
        }

        [Fact]
        public void Get_When_SectionRead_Returns_Null_Then_Returns_NotFound()
        {
            // Act && Assert
            var result = Sut.Get(0, 0, 0, 0, 0).AsNotFound<Result<SectionDto>>();
        }

        [Fact]
        public void Get_When_SectionRead_Succeeds_Then_Returns_Ok()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var section = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            // Act && Assert
            Sut.Get(
                pub.Id,
                chapter.Id,
                article.Id,
                section.Id,
                null
            ).AsOk<Result<SectionDto>>(); // No Part specified or related.
        }

        [Fact]
        public void Get_With_Part_Query_Param_Without_Part_Foreign_Key_Returns_BadRequest()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var section = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            var partId = new Faker().Random.Long();

            // Act && Assert
            Sut.Get(
                pub.Id,
                chapter.Id,
                article.Id,
                section.Id,
                partId
            ).AsBadRequest<Result<SectionDto>>();
        }

        [Fact]
        public void Get_With_Part_Relation_Without_Part_Query_Returns_BadRequest()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var part = Create<Part>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );
            var section = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            // Act && Assert
            Sut.Get(
                pub.Id,
                chapter.Id,
                article.Id,
                section.Id,
                null
            ).AsBadRequest<Result<SectionDto>>();
        }

        [Fact]
        public void Get_When_Body_Contains_Images_Returns_Ok_And_Img_Src_Values_Are_Presigned()
        {
            // Arrange
            var publication = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = publication.Id);
            var article = Create<Article>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id);

            var section = Create<Section>(
                SectionFactory.SECTION_WITH_IMAGE_IN_BODY,
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ArticleId = article.Id);

            // Act
            var result = Sut.Get(publication.Id, chapter.Id, article.Id, section.Id).AsOk<Result<SectionDto>>();

            // Assert
            result.ShouldNotHaveErrors();
            result.ResultObject.Body.ShouldContain(TestingConstants.AWS_S3_PRESIGNED_URL_IDENTIFIER);
        }

        [Fact]
        public void Get_When_Body_Is_Null_Then_Returns_Ok()
        {
            // Arrange
            var publication = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = publication.Id);
            var article = Create<Article>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id);

            var section = Create<Section>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ArticleId = article.Id,
                e => e.Body = null);

            // Act
            var result = Sut.Get(publication.Id, chapter.Id, article.Id, section.Id).AsOk<Result<SectionDto>>();

            // Assert
            result.ShouldNotHaveErrors();
            result.ResultObject.Body.ShouldBeNull();
        }

        #endregion

        #region Index

        [Fact]
        public void Index_When_FindAll_HasErrors_Then_Returns_InternalError()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            var mockSectionConductor = new Mock<IRepositoryConductor<Section>>();

            mockSectionConductor.SetupFindAllReturnsBasicErrorResult();
            RegisterDep(mockSectionConductor);

            // Act
            var result = Sut
                .Index(pub.Id, chapter.Id, article.Id)
                .AsInternalError<Result<List<SectionDto>>>();

            // Assert
            result.ShouldHaveErrorsFor(ErrorConstants.BASIC_ERROR_KEY);
        }

        [Fact]
        public void Index_When_Findall_Returns_Null_Then_Returns_Ok()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            // Act
            var result = Sut.Index(pub.Id, chapter.Id, article.Id).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.ShouldBeEmpty();
        }

        [Fact]
        public void Index_When_ArticleId_Does_Not_Match_Returns_Ok_With_No_Matches()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var sections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PublicationId = pub.Id
                )
            };

            var articleId = article.Id - 1;  // <== Use wrong articleId to test filter.

            // Act
            var result = Sut.Index(pub.Id, chapter.Id, articleId).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.Count.ShouldBe(0);
        }

        [Fact]
        public void Index_When_PartId_Does_Not_Match_Returns_Ok_With_No_Matches()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var part = Create<Part>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );
            var sections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                )
            };

            var partId = new Faker().Random.Long();  // <== Use wrong partId to test filter.

            // Act
            var result = Sut.Index(pub.Id, chapter.Id, article.Id, null, partId).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.Count.ShouldBe(0);
        }

        [Fact]
        public void Index_Finds_Section_Without_Part_Parent_Returns_Ok()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var sections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PublicationId = pub.Id
                )
            };

            // Act
            var result = Sut.Index(pub.Id, chapter.Id, article.Id).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.Count.ShouldBe(2);
        }

        [Fact]
        public void Index_Finds_Section_With_Article_And_Part_Parents_Returns_Ok()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);
            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );
            var part = Create<Part>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );
            var sections = new List<Section>
            {
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                ),
                Create<Section>(
                    e => e.ArticleId = article.Id,
                    e => e.ChapterId = chapter.Id,
                    e => e.PartId = part.Id,
                    e => e.PublicationId = pub.Id
                )
            };

            CreateSecondaryTestData();

            // Act
            var result = Sut.Index(
                publicationId: pub.Id,
                chapterId: chapter.Id,
                articleId: article.Id,
                partId: part.Id,
                parentId: null
            ).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.Count.ShouldBe(2);
        }

        [Fact]
        public void Index_Finds_Sections_And_Subsections_Returns_Ok()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);

            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );

            var part = Create<Part>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            var section = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            var subSection = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ParentId = section.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            CreateSecondaryTestData();

            // Act
            var result = Sut.Index(
                publicationId: pub.Id,
                chapterId: chapter.Id,
                articleId: article.Id,
                parentId: section.Id,
                partId: part.Id
            ).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.Count.ShouldBe(1);

            var ids = result.ResultObject.Select(e => e.Id);
            ids.ShouldContain(subSection.Id);
        }

        [Fact]
        public void Index_Finds_Sections_And_Nested_Subsections_Returns_Ok()
        {
            // Arrange
            var pub = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = pub.Id);

            var article = Create<Article>(
                e => e.PublicationId = pub.Id,
                e => e.ChapterId = chapter.Id
            );

            var part = Create<Part>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PublicationId = pub.Id
            );

            var section = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            var subSection = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ParentId = section.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            var firstSubSubSection = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ParentId = subSection.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            var secondSubSubSection = Create<Section>(
                e => e.ArticleId = article.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ParentId = subSection.Id,
                e => e.PartId = part.Id,
                e => e.PublicationId = pub.Id
            );

            CreateSecondaryTestData();

            // Act
            var result = Sut.Index(
                publicationId: pub.Id,
                chapterId: chapter.Id,
                articleId: article.Id,
                parentId: null,
                partId: part.Id
            ).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ResultObject.Count.ShouldBe(4);

            var ids = result.ResultObject.Select(e => e.Id);
            ids.ShouldContain(section.Id);
            ids.ShouldContain(subSection.Id);
            ids.ShouldContain(firstSubSubSection.Id);
            ids.ShouldContain(secondSubSubSection.Id);
        }

        [Fact]
        public void Index_When_Body_Contains_Images_Returns_Ok_And_Img_Src_Values_Are_Presigned()
        {
            // Arrange
            var publication = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = publication.Id);
            var article = Create<Article>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id);

            var bogusAssetLocation = $"assets/publications/{publication.Id}/images";
            var bodyXml = new XElement(PublicationElements.BODY,
                            new XElement((XNamespace)PublicationNamespaces.CB + PublicationElements.IMAGE,
                                new XAttribute("src", bogusAssetLocation)));

            var sections = new List<Section>();

            for (var i = 0; i <= 30; i++)
            {
                sections.Add(Create<Section>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ArticleId = article.Id,
                e => e.BodyAsXml = bodyXml));
            }

            // Act
            var result = Sut.Index(publication.Id, chapter.Id, article.Id).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ShouldNotHaveErrors();
            result.ResultObject.FirstOrDefault().Body.ShouldContain(TestingConstants.AWS_S3_PRESIGNED_URL_IDENTIFIER);
        }

        [Fact]
        public void Index_When_Results_And_Body_Is_Null_Then_Returns_Ok()
        {
            // Arrange
            var publication = Create<Publication>();
            var chapter = Create<Chapter>(e => e.PublicationId = publication.Id);
            var article = Create<Article>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id);

            var sections = new List<Section>();

            for (var i = 0; i <= 30; i++)
            {
                sections.Add(Create<Section>(
                e => e.PublicationId = publication.Id,
                e => e.ChapterId = chapter.Id,
                e => e.ArticleId = article.Id,
                e => e.Body = null));
            }

            // Act
            var result = Sut.Index(publication.Id, chapter.Id, article.Id).AsOk<Result<List<SectionDto>>>();

            // Assert
            result.ShouldNotHaveErrors();
            result.ResultObject.FirstOrDefault().Body.ShouldBeNull();
        }

        #endregion
    }
}
