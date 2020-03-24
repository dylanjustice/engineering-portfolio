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

* [Controller](./ContentAreaVersionDraftComponentPasteController.cs)
* [Controller](./ContentAreaVersionDraftComponentPasteConductor.cs)

## NFPA

### Problem
Images returned in xml are returned with original filepaths. We needed a way to convert these to a known storage location without using static assets.

### Solution
We modified the xml during the data import process to point to a private asset storage bucket on Amazon Simple Storage Service (S3) during the data import process. On the frontend, these url's will need to be presigned for the client to be able to download them and view in the application.
* [Example API Controller](./nfpa/SectionsController.cs)
* [ImageProcessingConductor](./nfpa/ImageProcessingConductor.cs)
* [XmlExtensions.ReplaceImgSrcAttributeWithAbsoluteUrl](./nfpa/XmlExtensions.cs)
* [XmlExtensions.GetImages](./nfpa/XmlExtensions.cs)

This solution was hard for me to visualize at first. We split the work into four parts.
1. Take the assets from the original uploaded file and put them in a directory on S3 at `assets/publications/:id/images/<filename>`.
2. Preprocess the XML during import and replace the original `src` attribute value on `<image>` elements with the relative location on the S3 bucket.
3. Preprocess the XML body in the api call to pre-sign the URL to authorize the client.
4. Write an XML to React conversion function to display the images in Book View.

# A Code refactor

# Proposal
## Technical Discovery @ andculture

I've been involved with several "discovery" engagements with our client partners at andculture. In every case, there is a lack of emphasis being put on delivering real value, and setting expectations that set us up for success. Instead, engineering is often asked for estimates that are used to generate a waterfall style Gantt chart, large requirements documents and/or fixed price engagements that eventually puts strain on the team and the client to maintain a "pace" rather than instilling trust in the team that we (andculture) are delivering value to our clients. The term "Technical Discovery" has been coined for projects that require engineering expertise. Or, a digital solution is expected.

andculture is a Design Company. We're made up of departments that all have extremely different thought processes. The clash is what we strive for, yet we tend to isolate that environment for projects that we have already tagged as engineering focused. I recommend we involve engineering with every discovery project. If it is clear that a department's involvement with the project is not adding value, that party can be dismissed.


