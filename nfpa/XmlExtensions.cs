using Nfpa.CodesApi.Business.Core.Constants.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Nfpa.CodesApi.Business.Core.Extensions
{
    public static class XmlExtensions
    {
        #region ReplaceImgSrcAttributeWithAbsoluteUrl

        public static XElement ReplaceImgSrcAttributeWithAbsoluteUrl(
            this XElement body,
            string valueToReplace,
            string replacementValue
        )
        {

            if (body == null || valueToReplace == null || replacementValue == null)
            {
                return body;
            }

            var images = body.GetImages();

            foreach (var img in images)
            {
                var src = img.Attribute("src");
                var relPath = src.Value.ToString();
                src.SetValue(relPath.Replace(valueToReplace, replacementValue));
            }

            return body;
        }

        #endregion ReplaceImgSrcAttributeWithAbsoluteUrl


        #region Images

        public static IEnumerable<XElement> GetImages(this XElement body)
        {

            if (body == null)
            {
                return default;
            }
            XName imageXName = $"{{{PublicationNamespaces.CB}}}{PublicationElements.IMAGE}";

            return body.Descendants(imageXName);

        }

        #endregion Images

        #region AddParentNodeAttributes

        public static XElement AddParentNodeAttributes(this XElement body)
        {
            if (body == null)
            {
                return default;
            }
            foreach (var descendant in body.Descendants())
            {
                descendant.SetAttributeValue(PublicationElementAttributes.PARENT_NODE, descendant.Parent.Name);
            }
            return body;
        }

        #endregion AddParentNodeAttribute

    }
}