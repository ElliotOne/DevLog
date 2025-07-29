# DevLog

DevLog project entails the development of a personalized website built on the ASP.NET Core framework. The website serves as a platform for showcasing personal information, professional achievements, and blog posts. With a focus on customization and user interaction, the website provides visitors with an engaging experience and facilitates effective communication.

## Key Features

- **Certification Management:** Implemented a dedicated section to showcase professional certifications and qualifications.
- **Contact Form:** Integrated an intuitive contact form for easy communication with visitors and receive their inquiries.
- **Skill Visualization:** Utilized progress bars to showcase skills and expertise with 100% scaling.
- **Blog Functionality:** 
   - Developed a feature-rich blog with post categories, allowing for multiple post tags.
   - Each blog post includes a main picture for visual representation.
   - Users can create, edit, and delete posts with ease.
   - Allows visitors to comment on posts for interactive engagement.
- **Donation Page:**
   - Implemented a dedicated page to display cryptocurrency wallet addresses set in the admin settings.
   - Users can view the wallet addresses provided for cryptocurrency donations on this page.
- **Settings Management:**
   - Developed an admin-exclusive section for managing website settings.
   - Admin can configure website preferences such as contact numbers, addresses, and other relevant information.
- **User Management:**
   - Established user roles such as admin and writers for efficient content management.
   - Implemented role-based access control (RBAC) for secure user authentication and authorization.

## Key Highlights

- Engineered a personalized ASP.NET Core CMS for my personal website.
- Developed a feature-rich blog with post categories, an HTML text editor, and image uploads.
- Enabled seamless upload and display of professional certificates.
- Implemented an intuitive contact form for effective communication.
- Utilized progress bars for visual representation and quantification of skills.
- Implemented a personalized settings section for easy customization.
- Established user roles (writers, admin, super admin) for efficient content management.

## Database Schema

DevLog project utilizes a SQL Server database with the following schema:

- **AspNetRoleClaims:** Stores claims associated with user roles.
- **AspNetRoles:** Manages user roles for authorization.
- **AspNetUserClaims:** Stores claims associated with user accounts.
- **AspNetUserLogins:** Manages external logins for user accounts.
- **AspNetUserRoles:** Maps user accounts to roles.
- **AspNetUsers:** Stores user account information.
- **AspNetUserTokens:** Stores authentication tokens for users.
- **Certificates:** Manages information about professional certificates.
- **Contacts:** Stores contact messages submitted via the website.
- **PostCategories:** Manages categories for blog posts.
- **Posts:** Stores blog posts, including content, tags, and category.
- **PostComments:** Records comments on blog posts.
- **ProgressBar:** Tracks topics and their percentage completion for visual skill representation.
- **Settings:** Stores various website settings, including contact information, social media links, and cryptocurrency wallet addresses.

## Used Technologies

DevLog is developed using the following technologies:

- **Backend:**
   - ASP.NET Core
   - Entity Framework Core (EF Core)
   - SQL Server
   - C#
   - AutoMapper
   - ElmahCore
   - Microsoft.AspNetCore.Mvc.NewtonsoftJson
   - SmartBreadcrumbs
- **Frontend:**
   - HTML
   - CSS
   - JavaScript
   - Bootstrap
   - jQuery
   - jQuery Ripples
   - jQuery DataTables
   - jQuery Validation
   - CKEditor
   - Font Awesome
   - Jarallax
   - Owl Carousel
   - Select2
   - Shave.js
   - Sortable.js
   - Typewriter.js

## Screenshots

Here are some screenshots of DevLog:

1. **Database Diagram**<br/>
   <img src="screenshots/db-diagram.png"/>

2. **Home Page - Frontend**<br/>
   <img src="screenshots/front-home.png"/>

3. **Blog Page - Frontend**<br/>
   <img src="screenshots/front-blog.png"/>

4. **About Page - Frontend**<br/>
   <img src="screenshots/front-about.png"/>   

5. **Dashboard - Admin Panel**<br/>
   <img src="screenshots/panel-dashboard.png"/>

6. **Blog Categories Managment - Admin Panel**<br/>
   <img src="screenshots/panel-categories.png"/>

7. **Progress Bars Management - Admin Panel**<br/>
   <img src="screenshots/panel-progresses.png"/>
