using LMS.Business.Core.Enumerations.Errors;
using LMS.Business.Core.Extensions.Errors;
using LMS.Business.Core.Interfaces.Conductors.Domain.ContentAreaVersionComponents;
using LMS.Business.Core.Interfaces.Errors;
using LMS.Business.Core.Models.Errors;
using LMS.Core.Interfaces.Conductors.Content;
using LMS.Core.Models.Entities.Content;
using LMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Business.Conductors.Domain.ContentAreaVersionComponents
{
    public class ContentAreaVersionComponentPasteConductor : IContentAreaVersionComponentPasteConductor
    {
        #region Private Members

        readonly IContentAreaVersionComponentConductor          _contentAreaVersionComponentConductor;
        readonly IContentAreaVersionComponentDuplicateConductor _duplicateConductor;
        #endregion

        #region Constructor
        public ContentAreaVersionComponentPasteConductor(
            IContentAreaVersionComponentConductor          contentAreaVersionComponentConductor,
            IContentAreaVersionComponentDuplicateConductor duplicateConductor)
        {
            _contentAreaVersionComponentConductor = contentAreaVersionComponentConductor;
            _duplicateConductor                   = duplicateConductor;
        }

        #endregion

        #region IContentAreaVersionComponentPasteConductor Implementation

        public IResult<ContentAreaVersionComponent> Paste(long id, long sourceId, long? currentUserId = null) => Do<ContentAreaVersionComponent>.Try(r =>
        {
            var getComponentToReplaceResult = _contentAreaVersionComponentConductor.FindById(id);
            getComponentToReplaceResult.ThrowIfAnyErrors();

            var componentToReplace = getComponentToReplaceResult.ResultObject;

            var getComponentToDuplicateResult = _contentAreaVersionComponentConductor.FindById(sourceId);
            getComponentToDuplicateResult.ThrowIfAnyErrors();

            var componentToDuplicate = getComponentToDuplicateResult.ResultObject;

            var duplicateResult = _duplicateConductor.Duplicate(
                                                        id: componentToDuplicate.Id,
                                                        createdById: currentUserId,
                                                        contentAreaVersionId: componentToReplace.ContentAreaVersionId);
            if (duplicateResult.HasErrors)
            {
                r.AddErrors(duplicateResult.Errors);
                return null;
            }

            var newComponent = duplicateResult.ResultObject;

            newComponent.DisplaySequence = componentToReplace.DisplaySequence;
            newComponent.ContainerName   = componentToReplace.ContainerName;
            newComponent.ParentId        = componentToReplace.ParentId;
            newComponent.PageNumber      = componentToReplace.PageNumber;

            var updateResult = _contentAreaVersionComponentConductor.Update(newComponent, currentUserId);
            if (updateResult.HasErrors)
            {
                r.AddErrors(updateResult.Errors);
                return null;
            }

            var deleteResult = _contentAreaVersionComponentConductor.Delete(id, currentUserId);
            if (deleteResult.HasErrors)
            {
                r.AddErrors(deleteResult.Errors);
                return null;
            }

            return newComponent;

        }).Result;
    }
        #endregion
}
