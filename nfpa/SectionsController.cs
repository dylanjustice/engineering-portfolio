using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core.Enumerations;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces.Conductors;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Nfpa.CodesApi.Business.Core.Extensions.Expressions;
using Nfpa.CodesApi.Business.Core.Interfaces.Conductors.Aspects;
using Nfpa.CodesApi.Business.Core.Models.Entities.Sections;
using Nfpa.CodesApi.Presentation.Web.Models.Dtos.Sections;

namespace Nfpa.CodesApi.Presentation.Web.Controllers.Api.V1.Publications.Annexes.Articles.Sections
{
    [FormatFilter]
    [Route("api/v1/publications/{publicationId:long}/annexes/{annexId:long}/articles/{articleId:long}/sections")]
    public class SectionsController : CodesApiController
    {
        #region Properties

        private readonly ILogger<SectionsController> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryReadConductor<Section> _sectionsReadConductor;
        private readonly IImageProcessingConductor _imageProcessingConductor;

        #endregion Properties

        #region Constructor

        public SectionsController(
            ILogger<SectionsController> logger,
            IMapper mapper,
            IRepositoryReadConductor<Section> sectionsReadConductor,
            IImageProcessingConductor imageProcessingConductor
        )
        {
            _logger = logger;
            _mapper = mapper;
            _sectionsReadConductor = sectionsReadConductor;
            _imageProcessingConductor = imageProcessingConductor;
        }

        #endregion Constructor

        #region Public Methods

        #region Get

        /// <summary>
        /// Gets Section entity with matching Id property
        /// </summary>
        /// <param name="publicationId"></param>
        /// <param name="annexId"></param>
        /// <param name="articleId"></param>
        /// <param name="id"></param>
        /// <returns>Section entity</returns>
        /// <response code="500">Error communicating with the database</response>
        /// <response code="404">Section with requested Id not found</response>
        /// <response code="200">Section found</response>
        [HttpGet("{id:long}")]
        public IActionResult Get(
            [FromRoute] long publicationId,
            [FromRoute] long annexId,
            [FromRoute] long articleId,
            [FromRoute] long id
        )
        {
            var sectionReadResult = _sectionsReadConductor.FindById(id);

            if (sectionReadResult.HasErrors)
            {
                return InternalError<SectionDto>(null, sectionReadResult.Errors, _logger);
            }

            var section = sectionReadResult.ResultObject;

            if (section == null)
            {
                return NotFound<SectionDto>(
                    null,
                    ERROR_RESOURCE_NOT_FOUND_KEY,
                    ERROR_RESOURCE_NOT_FOUND_MESSAGE
                );
            }
            if (section.PublicationId != publicationId ||
                section.AnnexId != annexId
            )
            {
                return BadRequest<SectionDto>(null);
            }

            if (!string.IsNullOrEmpty(section.Body))
            {
                var processImageResult = _imageProcessingConductor.ProcessImageSrcUrls(section.BodyAsXml);
                if (processImageResult.DoesNotHaveErrors(ErrorType.Error))
                {
                    section.BodyAsXml = processImageResult.ResultObject;
                }
            }

            return Ok<SectionDto>(_mapper.Map<SectionDto>(sectionReadResult.ResultObject), null);
        }

        #endregion Get

        #region Index

        /// <summary>
        /// Provides an array of Section Entities matching given criteria
        /// </summary>
        /// <param name="publicationId"></param>
        /// <param name="annexId"></param>
        /// <param name="articleId"></param>
        /// <param name="parentId"></param>
        /// <param name="sortBy">TODO</param>
        /// <param name="sortDirection">TODO</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Array of Section Entities</returns>
        /// <response code="400">Publication Id missing on request</response>
        /// <response code="500">Error communicating with the database</response>
        /// <response code="200">Sections found successfully</response>
        [HttpGet]
        public IActionResult Index(
            [FromRoute] long publicationId,
            [FromRoute] long annexId,
            [FromRoute] long articleId,
            [FromQuery] long? parentId = null,
            [FromQuery] string sortBy = nameof(Section.DisplaySequence),
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? skip = default(int?),
            [FromQuery] int? take = default(int?)
        )
        {

            Expression<Func<Section, bool>> filter = ((e) =>
                e.ArticleId == articleId &&
                e.AnnexId == annexId &&
                e.PublicationId == publicationId
            );

            if (parentId.HasValue)
            {
                filter = filter.AndAlso(e => e.ParentId == parentId);
            }

            //TODO: Implement OrderBy clause

            var findResult = _sectionsReadConductor.FindAll(
                filter,
                (e) => e.OrderBy(i => i.DisplaySequence),
                null,
                skip,
                take
            );

            if (findResult.HasErrors)
            {
                return InternalError<List<SectionDto>>(null, findResult.Errors, _logger);
            }

            foreach (var section in findResult.ResultObject)
            {
                if (string.IsNullOrEmpty(section.Body))
                {
                    continue;
                }

                var processImageResult = _imageProcessingConductor.ProcessImageSrcUrls(section.BodyAsXml);
                if (processImageResult.DoesNotHaveErrors(ErrorType.Error))
                {
                    section.BodyAsXml = processImageResult.ResultObject;
                }
            }

            return Ok<List<SectionDto>>(_mapper.Map<List<SectionDto>>(findResult.ResultObject), null);

        }

        #endregion Index

        #endregion Public Methods

    }
}