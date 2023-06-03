# .NET Clean Architecture 

This project was created by [Advanced Software Solution](https://advancedsoftware.io) and licensed under the MIT license. If you would like to take advantage of the premium version where we extend it to take heavier load and customize it for your organization needs, please contact us at info@advancedsoftware.io so we can understand your needs better or you may [book a free consultation](https://booking.setmore.com/scheduleappointment/ab5af8aa-8378-405c-b7ce-e9e353e26599/services/sdff2b3034d69c0461948ed096578fd7a3b3a5dc5?source=easyshare) so we can help you more.
| | Free Version |Premium Version  |
|--|--|--|
|Full CRUD with Bulk Insert  | Yes  |Yes |
|Authentication|JWT|JWT, Azure AD|
|Database Support|SQL Server|SQL Server, Postgres, Oracle, MySQL|
|Caching|None|Redis, InMemory|
|Search Support| None| Lucen, Elastic|
|Roslyn Analyzer|None|Visual Studio Extension|
|Frontend|Blazor|Blazor, React, Angular, Vue.JS|
|Docker|To be supported later on | Ready|
|Ticket and Support|Only Github Issues| Email, Phone Support|


# Content
 1. Intro
 2. Project Structure
 3. Development Guideline
	 1. Entity
	 2. Validation
	 3. Operation
	 4. Mapping
	 5. Service
	 6. Injection for Operation and Services
	 7. API Controllers
	 8. OData
4. Deployment
	1. Local IIS
	2. Azure
	3. AWS

## Intro

The .NET Clean Architecture was created to help any .NET developer have a powerful start when creating a new ASP.NET Core application. We intended to make this not only an easy-to-use template, as well as easy to develop upon as a project that can be later modified and updated. This project was created based on our previous experiences with many development projects and we found this a good way for you to develop software using .NET.
As this for sure is an ongoing project, we are happy to take your opinion and consideration into it. As this repo will be always free and available for anyone to use for their work. 
We hope you enjoy using it and love to hear your feedback and what can be done better to support the work.

## Project Structure

The Solution contains the following projects:

 1. CleanBase
	 1. The main project for the entities and the shared classes between the Frontend and the backend
2. CleanOperation
   1. Contains the EF Mapping and the low level operations for your project. Usually here we place the I/O operations or the integrations with external services so we have a common code for these things among our services
3. CleanBusiness
	  1. Here we implement our services that contains the logic and the flow we need to complete a business need. 
4. CleanAPI
		1. This is where we connect all the other projects to produce a RESTFul service that communicate with all the layers and do the operations needed
5. CleanFront
	1. The Blazor Project that connects to the backend to allow user to interact with the system. This is using Blazor WebAssembly Hosting for a much smoother experience. You can use any Frontend framework such as Angular or React. 

> Important Note! We suggest to use Blazor if you have a limited number
> of users or using it internally in a private network as it uses a lot
> of bandwidth

6. DatabaseApp
	1. A project that contains all the schema definations for the basic database. You may use this projec to control your database.

## Development Guide

You are free to customize the project as you like, and we intend to give you a good way to use the project so it becomes easier and easier to use. This guide will help you understand the project and have a consistent work across the project when using it.

### 1. Entity
When you want to store the data of your application, for sure you will define a table to store the data inside it. Now, to have access to this table you will create a Entity class that define the columns of the table and map it to a C# class.

Usually you place your entities in the **CleanBase** project, and inside a folder representing a module.

The entity class must inherit the **EntityRoot** class. This class contains two common properties, the Id and the Rowversion. 
>Note: In case your entity is a many to many mapping table, you don't need to
> inherit the EntityRoot class.

Inside the Entity, it is always recommended not implement or use any attributes on the propertises. For example, the **Required** attribute. We can config the validations in the validtors and no need to add any custom property. Also, this will make our application much faster and have much cleaner code. If you have configuration specifically for an entity, you can configure them in the **CleanOperation** or the **CleanAPI**.

Below is a sample entity class:

    public class TodoList : EntityRoot
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public IList<TodoItem>? TodoItems { get; set; }
    }
Note: For the navigation property, we recommend to have the entity property to be nullable
### 2. Validation
To keep our database safe from incorrect inputs, we have [FluentValidation](https://github.com/FluentValidation/FluentValidation) Package that we use for the frontend validations in the form and also in the backend in the API level.

When you implement the validation, it will be reflected into any form in the Blazor Frontend and also in the backend. 

The validation is usually implemented on the entity level, to so we will define a class with the name of the entity and end it with validator, example: TodoValidator, after that you can define the validation rules. If you want a property to be ignored just mark it as nullable.

    public class TodoListValidation : AbstractValidator<TodoList>
    {
        public TodoListValidation()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.Now);
    
        }
    } 
