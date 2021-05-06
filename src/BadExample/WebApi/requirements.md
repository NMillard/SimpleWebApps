
## Intro
Create a simple aspnetcore app with some simple controllers and operations.
This app will use some common "bad" practices, such as:
- Generic repository
- Entities inheriting from common base class
- Entities with too many concerns

## Requirements
Experience with C#
Some experience with aspnet or aspnetcore

### Models
[] Entity base
[] Simple user
[] Article

### Database access
[] DbContext
[] Migrations
[] Migrate database

### Settings
[] appsettings.json

### Operations
Users controller
[] Get user
[] Create user - if authorized
[] Log user in

Articles controller
[] Get all articles - JSON | CSV
[] Get single article
    Not found should throw an error
[] Delete an article



# Scene 1 - Solution introduction
Open JetBrains
Create Project SimpleWebApi

# Scene 2 - Models
1. Create Models folder
2. Create User, Article and EntityBase

# Scene 3 - Database
1. Create AppDbContext
2. Install nuget packages
3. override OnConfiguring
4. Add DbSets

# Scene 4 - Repositories
1. Create RepositoryBase<T>, UserRepository and ArticleRepository
2. Override Get() in UserRepository to use Include

# Scene 5 - Register services
1. Register DbContext and repositories with DI container
   

