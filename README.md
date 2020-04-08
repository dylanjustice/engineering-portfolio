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

In working on this feature I learned a valuable lesson in getting team feedback, EARLY! I was new to the project and had not yet gotten acclimated to the patterns and preferences of the team. Also, I was introduced to the concept of "Aspects", and how they can be leveraged to make the API more RESTful.

* [ContentAreaVersionDraftComponentPasteController](./edio/ContentAreaVersionDraftComponentPasteController.cs)
* [ContentAreaVersionDraftPasteConductor](./edio/ContentAreaVersionDraftPasteConductor.cs)

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
## Pinnacle Revitalization

Remembering a refactor that I was particularly proud of was challenging. I find myself refactoring code in smaller chunks as I approach it rather than larger refactors that necessarily improve performance, scalability or maintainability. Maybe this is an indication of something to work on.
My biggest learning experience with andculture to date has been the Pinnacle Revitalization project.

### Problem
Pinnacle suffered from a lot of style bugs, inconsistencies, and regressions on changes. On top of that, the development team struggled to fix these problems without unintentionally breaking something else. The site used bootstrap and Sass, but the styles were two large `*.scss` files (one for "general" and one for "mobile") that were hard to read. On top of that, the stylesheet had to be split in two files when it was rendered because it was so large! Older browsers would actually refuse to load it because of how many classes were defined in it.

### Solution
Partially redesign the site. In doing so, rip out the styles, javascript and bootstrap framework and implement a prototypical javascript namespacing pattern, ITCSS with Sass CSS extension and rework the styles already existing on the site. Given my experience had not been heavy on the frontend of things, this was a challenging exercise in getting up to speed quickly with new technology and concepts. When fully implemented, the stylesheet was drastically reduced in size, and the naming conventions of styles had been standardized to make debugging and extension much easier for developers working on the project. Style bugs became a non-issue, and the mobile site was awarded Platinum in an e-healthcare leadership for Best Mobile Website.


# Proposal
## Technical Discovery @ andculture

I've been involved with several "discovery" engagements with our client partners at andculture. In every case, there is a lack of emphasis being put on delivering real value, and setting expectations that will set us up for success. Instead, engineering is often asked for estimates that generate a waterfall style Gantt chart, large requirements documents and/or fixed price engagements that eventually puts strain on the team and the client to maintain a "pace" rather than instilling trust that we (andculture) are delivering value. The term "Technical Discovery" has been coined for projects that require engineering expertise. In other words, a digital solution is expected from the beginning.

andculture has established a brand as a Design Company. We're made up of departments that all have extremely different thought processes, and we strive for that "clash". However, we tend to isolate that environment for projects that we have already tagged as, engineering focused. Discovery is our opportunity to show our client partners that we can work effectively as a unit composed of all departments, rather than relying on a single individual to come up with the solution. We need to re-focus on delivering something valuable to the client.

A principal mistake we have made in the discovery process is making far too many assumptions. This should be our opportunity to prototype, user test, wireframe and get our clients on board before we make assumptions. Adding this to the discovery process will make us all more pragmatic. What will work? What won't work? How much effort will it *really* take? And most importantly, work with the client to find out they want first, rather than when it's "done".

I'm proposing that the engineering presence in the discovery phase be focused on developing prototypes that gauge effort, test integrations and better prepare the client for the project they intend to start. Whether it's with us, or without. Prototypes are not intended to be used in the actual build, they're intended to prove a concept and be thrown away. This will eliminate assumptions, give us confidence in our estimates and really prepare us for the prospective project.

