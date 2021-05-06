﻿
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
