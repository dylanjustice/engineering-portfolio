# Senior Software Engineer Application
## Dylan Justice

### Purpose
Provide examples where you demonstrated the following:
1. Write clean, testable, and maintainable code.
2. A code refactor that improved performance, maintainability, or scalability.
3. A proposal for a change to an existing approach (code or process) & explain what makes their proposed change an improvement over the existing approach.

---

# Clean, testable maintainable code
## edio

### Problem
Course authors want to be able to "copy and paste" course components in the course editor/builder.

### Solution
Edio had the aspect of "cloning", but lacked the capacity to clone, and create recursively.

In working on this feature I learned a valuable lesson in getting team feedback, EARLY! I was new to the project and had not yet gotten acclimated to the patterns and preferences of the team.

ContentAreaVersionDraftComponentPasteController.cs
ContentAreaVersionDraftComponentPasteConductor.cs


## NFPA

### Problem
Images returned in xml are returned with original filepaths. We needed a way to convert these to a known storage location without using static assets.

### Solution
We modified the xml during the data import process to point to a private asset storage bucket on Amazon Simple Storage Service (S3) during the data import process. On the frontend, these url's will need to be presigned for the client to be able to download them and view in the application.
* [Example API Controller](https://bitbucket.org/andCulture/nfpa/src/development/dotnet/api/Presentation/Web/Controllers/Api/V1/Publications/Chapters/Articles/Sections/SectionsController.cs)
* [ImageProcessingConductor](https://bitbucket.org/andCulture/nfpa/src/development/dotnet/api/Business/Conductors/Aspects/ImageProcessingConductor.cs
)
* [XmlExtensions.ReplaceImgSrcAttributeWithAbsoluteUrl](https://bitbucket.org/andCulture/nfpa/src/development/dotnet/api/Business/Core/Extensions/XmlExtensions.cs)
* [XmlExtensions.GetImages](https://bitbucket.org/andCulture/nfpa/src/development/dotnet/api/Business/Core/Extensions/XmlExtensions.cs)

This solution was hard for me to visualize at first. We split the work into four parts.
1. Take the assets from the original uploaded file and put them in a directory on S3 at `assets/publications/:id/images/<filename>`.
2. Preprocess the XML during import and replace the original `src` attribute value on `<image>` elements with the relative location on the S3 bucket.
3. Preprocess the XML body in the api call to pre-sign the URL to authorize the client.
4. Write an XML to React conversion function to display the images in Book View.

# A Code refactor

# Proposal
