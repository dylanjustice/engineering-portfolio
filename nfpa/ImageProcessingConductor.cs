using System.Xml.Linq;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Nfpa.CodesApi.Business.Core.Constants.Xml;
using Nfpa.CodesApi.Business.Core.Enumerations;
using Nfpa.CodesApi.Business.Core.Extensions;
using Nfpa.CodesApi.Business.Core.Interfaces.Conductors.Aspects;
using Nfpa.CodesApi.Business.Core.Interfaces.Providers.Storage;

namespace Nfpa.CodesApi.Business.Conductors.Aspects
{
    public class ImageProcessingConductor : IImageProcessingConductor
    {
        #region Private Members

        private IStorageProvider _storageProvider;
        private ILogger<ImageProcessingConductor> _logger;

        #endregion Private Members

        #region Constants

        public const string ATTRIBUTE_NOT_FOUND_ERROR_KEY = "ImageProcessingConductor.ProcessImages.AttributeNotFound";

        #endregion Constants

        #region Constructors

        public ImageProcessingConductor(
            IStorageProvider storageProvider,
            ILogger<ImageProcessingConductor> logger)
        {
            _storageProvider = storageProvider;
            _logger = logger;
        }

        #endregion Constructors

        #region Implementation

        public IResult<XElement> ProcessImageSrcUrls(XElement root) => Do<XElement>.Try((r) =>
        {
            var images = root.GetImages();

            foreach (var img in images)
            {
                var src = img.Attribute("src");
                if (src == null)
                {
                    r.AddErrorAndLog(_logger,
                        ATTRIBUTE_NOT_FOUND_ERROR_KEY,
                        "Unable to process Image. Attribute 'src' not found on element 'img'");
                    break;
                }

                var getRemoteAccessDetailsResult = _storageProvider.GetRemoteAccessDetails(src.Value);
                if (getRemoteAccessDetailsResult.HasErrors)
                {
                    r.AddErrors(getRemoteAccessDetailsResult);
                    continue;
                }

                src.SetValue(getRemoteAccessDetailsResult.ResultObject.Url);
            }

            return root;

        }).Result;

        #endregion Implementation

    }
}