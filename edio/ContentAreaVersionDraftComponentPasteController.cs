using AutoMapper;
using LMS.Business.Core.Enumerations.Errors;
using LMS.Business.Core.Extensions.Errors;
using LMS.Business.Core.Interfaces.Conductors.Domain.ContentAreaVersionComponents;
using LMS.Core.Models.Security;
using LMS.Presentation.Web.Attributes.Security;
using LMS.Presentation.Web.Models.Dtos.Content;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Presentation.Web.Controllers.Api.V1.Staff.Content
{
    [FormatFilter]
    [ResponseCache(CacheProfileName = "Never")]
    [Route("api/v1/staff/contentareaversioncomponents/{id:long}/paste")]
    public class ContentAreaVersionComponentsPasteController : LmsController
    {

        #region Private Members

        readonly IContentAreaVersionComponentPasteConductor         _pasteConductor;
        readonly IContentAreaVersionComponentPasteValidateConductor _pasteValidateConductor;
        readonly IMapper                                            _mapper;

        #endregion

        #region Constructors

        public ContentAreaVersionComponentsPasteController(
            IContentAreaVersionComponentPasteConductor pasteConductor,
            IContentAreaVersionComponentPasteValidateConductor pasteValidateConductor,
            IMapper mapper)
        {
            _pasteConductor         = pasteConductor;
            _pasteValidateConductor = pasteValidateConductor;
            _mapper                 = mapper;
        }

        #endregion

        #region POST

        [AclAuthorize(AclStrings.STAFF_CONTENT_AREAS_CREATE)]
        [HttpPost]
        public IActionResult Post(long id, [FromBody] ContentAreaVersionComponentPasteDto dto)
        {
            var pasteValidationResult = _pasteValidateConductor.Validate(
                                                                    contentAreaVersionComponentId:       id,
                                                                    courseId:                            dto.CourseId,
                                                                    sourceContentAreaVersionComponentId: dto.SourceId);

            if (pasteValidationResult.HasErrors(ErrorType.Error))
            {
                return InternalError<ContentAreaVersionComponentDto>(null, pasteValidationResult.Errors);
            }
            if (pasteValidationResult.HasErrors(ErrorType.ValidationError))
            {
                return BadRequest<ContentAreaVersionComponentDto>(null, pasteValidationResult.Errors);
            }
            var pasteResult = _pasteConductor.Paste(
                                                id:          id,
                                                sourceId:    dto.SourceId,
                                                createdById: CurrentUserId);
            if (pasteResult.HasErrors)
            {
                return InternalError<ContentAreaVersionComponentDto>(null, pasteResult.Errors);
            }
            if (pasteResult.ResultObject == null)
            {
                return BadRequest<ContentAreaVersionComponentDto>(null, null);
            }

            return Created(string.Empty, _mapper.Map<ContentAreaVersionComponentDto>(pasteResult.ResultObject), null);

        }

        #endregion


    }
}
