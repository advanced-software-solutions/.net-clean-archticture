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
	 5. Repository
	 6. Actors
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

5. DatabaseApp
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
You can can control the validation from the **Program.cs** where you can configure it further more. 

    //Current configuration at Program.cs
    .AddFluentValidation(r =>
    {
     r.RegisterValidatorsFromAssemblyContaining<TodoListValidation>(lifetime: ServiceLifetime.Scoped);
     r.AutomaticValidationEnabled = false;
     r.ImplicitlyValidateChildProperties = false;
    })

### 3. Operation

We wanted to keep our low level access code (i.e. The code that does operation non business related) on a tier that is shared across multiple layers and we can access it from multiple services or locations. By creating the Operation layer we place the common code such as RESTFul calls to outside services in a place we can access it multiple times without facing circular dependency issues. 

Also, we choosed this layer to hold the **DBContext** and the **Repository** class as we will not have to worry about any conflicts in the injections across multiple services.

### 4. Mapping
#### AppDataContext
Where we define our mapping between EF and the tables in the database. We map the **Entity** class we created in the **CleanBase** and we set the relevant table that connects to this entity. We recommend to name the table with Pluarl nouns and the Entity in Singular so we can keep an eye while mapping the **Entity** and the **Table**.

We choose to user FluentAPI not DbSet to map our entities with entity framework. This way we are keeping our DbContext class clean and easy to manage and read while we do all the mapping and sometimes the diffcult configurations via FluentAPI. And also, this way we don't need to add extra attributes on the class of the entity.

Let's see how we mapped the TodoList entity with the relevant table:

    modelBuilder.Entity<TodoList>().ToTable("TodoLists", "Core");
For the other propertises such as the Id or the Row version, we don't need to include them here as we already have them configured in the **EntityPropertyMapper** method.

### 5. Repository
To enhance our data access and make it simple, we have the Repository class that contains many methods will make it very simple for us to access the tables in our database and make it easier to CRUD the data. The following mehods are implemented:

    T? Get(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
            T? Get(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
            T Insert(T entity);
            EntityEntry<T> Update(T entity);
            void Delete(T entity);
            void Delete(int id);
            Task<T?> GetAsync(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
            Task<T?> GetAsync(Expression<Func<T, bool>> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includedNavigations = null);
            Task<T?> InsertAsync(T entity);
            List<T> Insert(List<T> entities);
            Task InsertAsync(List<T> entity);
            void Update(List<T> entities);
            void Delete(List<T> entities);
            IQueryable<T?> Query();

These methods contians the basic access methods for any entity and you can access them by injecting the relevant entity with the repository class. It is always encouraged to inject multiple repository rather than services into each other to avoid circular dependency issues. 

For you to understand how the repository class works, let's examin a method that does an insert:

    public T Insert(T entity)
    {
                Guard.Against.Null(entity);
                return Aspect(() =>
                {
                    _dataContext.Add(entity);
                    return entity;
                });
     }

 Notice that we send a generic parameter on our repository class so we can handle all the different types. Next, we added a Guard to make sure our input is valid, other wise it will raise an exception. Now, we added an Aspect wround the actual method, but what is an Aspect? It is a an action method that we implemented we use it to wrapp our methods to eliminate redundent code such as "try...Catch" and enabling the transactions for saving the data on the database.
 
 So we wrapped the DbContext method with this Aspect, and after it executes it will save the changes and commit the transaction the database. Take a look inside the aspect method to see more details:

        public override T Aspect<T>(Func<T> operation)
        {
            if (_dataContext.Database.CurrentTransaction != null)
            {
                return base.Aspect(operation);
            }
            else
            {
                using (var txn = _dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        var items = base.Aspect(operation);
                        _dataContext.SaveChanges();
                        txn.Commit();
                        return items;
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }
Just before we jump on, please note that since we are already using OData in most of the time, we will lower the amount of typical code. Also, the other common operations are already implemented such as insert, insert bulk.

### 6. Actors
Here begins the actual work for our business implementation. Here, we define our logic and how the actions must be done. Remember that we prefer to inject the repository rather than other actors; the repository will handle all the data insertion. This way, we have a much cleaner code and no issue with circular dependency. Note that with the Version 2.x, there is no need to create an actor unless you need to implement a more custom logic rather than simple CRUD.  

Note that this section is pending, but still, you can create an untyped actor or whatever actor you like and inject it in the system.


### 7. API Controller
The nice thing about 2.x release is that you no longer need to create a separate controller for each entity; the CleanAPIGenerator will create the controller for you with different flavours. For the basic read, you will have the regular OData ready for you. For the update, delete, and insert, we will also generate more methods for you using FastEndpoints, so the process is much faster and has better performance.

Notice that the URL route will be the name of the entity. For example: 'api/TodoList' with using the right Http Request Type (GET, POST, PUT, DELETE).

### 8. OData
To easy the query of entities, we can use OData to query the data as we like. For example, if we want to query the TodoList where it is past due we just send the following URL: 'api/TodoList?$filter=DueDate gt 2023-06-10 ' and we shall get our results.
You can get more information about [OData from their official website](https://www.odata.org/).
