# Implementing CQRS and Mediator Patterns with MediatR in an ASP.NET Core Web API

This documentation provides a step-by-step guide to implementing the CQRS and Mediator patterns using the MediatR library in an ASP.NET Core Web API application.

## Introduction to CQRS and the Mediator Pattern

### Key Concepts

- **CQRS stands for "Command Query Responsibility Segregation". It separates the responsibility of commands (writes) and queries (reads) into different models, improving scalability and performance.
- **Mediator Pattern**: Centralizes communication between objects, promoting loose coupling and simplifying dependency management.

**Key Points:**
- Commands modify data, while queries read data.
- Reduces complexity by separating read and write operations.
- Enhances performance by allowing optimization for reads and writes independently.

### Mediator Pattern
The Mediator pattern defines an object that encapsulates how objects interact, promoting loose coupling. Components interact through a mediator, reducing direct dependencies and simplifying testing and maintenance.

---

## How MediatR Facilitates CQRS and Mediator Patterns

MediatR is an "in-process" Mediator implementation for building CQRS systems. It handles all communication between the user interface and the data store within the same process.

**Important Notes:**
- MediatR is suitable for single-process applications.
- For distributed systems, consider using message brokers like Kafka or Azure Service Bus.

---

## Setting Up an ASP.NET Core API with MediatR

### Project Setup

1. **Create a New ASP.NET Core Web API Application**
   - Open Visual Studio.
   - Create a new project and name it `CQRSExample`.

2. **Install MediatR Package**
   - Open the Package Manager Console.
   - Run the command: `PM> install-package MediatR`.

3. **Configure MediatR in `Program.cs`**
   - Add the following line to register MediatR:
     ```csharp
     builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
     ```

4. **Update `launchSettings.json`**
   - Modify the `launchSettings.json` file to set the application URL and environment variables.

---

## Creating the Controller

1. **Add `ProductsController`**
   - Create a new API controller named `ProductsController.cs` in the Controllers folder.
   - Initialize an `IMediator` instance in the constructor:
     ```csharp
     [Route("api/products")]
     [ApiController]
     public class ProductsController : ControllerBase
     {
         private readonly IMediator _mediator;
         public ProductsController(IMediator mediator) => _mediator = mediator;
     }
     ```

---

## Creating the Data Store

1. **Define the `Product` Class**
   ```csharp
   public class Product
   {
       public int Id { get; set; }
       public string Name { get; set; }
   }


2. **Implement `FakeDataStore`**
   ```csharp
   public class FakeDataStore
   {
       private static List<Product> _products;

       public FakeDataStore()
       {
           _products = new List<Product>
           {
               new Product { Id = 1, Name = "Test Product 1" },
               new Product { Id = 2, Name = "Test Product 2" },
               new Product { Id = 3, Name = "Test Product 3" }
           };
       }

       public async Task AddProduct(Product product)
       {
           _products.Add(product);
           await Task.CompletedTask;
       }

       public async Task<IEnumerable<Product>> GetAllProducts() => await Task.FromResult(_products);
   }

3. **Register `FakeDataStore` as a Singleton**
   - Add the following line in `Program.cs`:
     ```csharp
     builder.Services.AddSingleton<FakeDataStore>();
     ```

---

### Queries

1. **Define `GetProductsQuery`**
   - Create a `Queries` folder.
   - Add the following class:
     ```csharp
     public record GetProductsQuery() : IRequest<IEnumerable<Product>>;
     ```

2. **Implement `GetProductsHandler`**
   - Create a `Handlers` folder.
   - Add the following class:
     ```csharp
     public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
     {
         private readonly FakeDataStore _fakeDataStore;
         public GetProductsHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

         public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken) 
             => await _fakeDataStore.GetAllProducts();
     }
     ```

3. **Add `GetProducts` Method in `ProductsController`**
   ```csharp
   [HttpGet]
   public async Task<ActionResult> GetProducts()
   {
       var products = await _mediator.Send(new GetProductsQuery());
       return Ok(products);
   }

### Commands

1. **Define `AddProductCommand`**
   - Create a `Commands` folder.
   - Add the following record:
     ```csharp
     public record AddProductCommand(Product Product) : IRequest;
     ```

2. **Implement `AddProductHandler`**
   - Add the following class in the `Handlers` folder:
     ```csharp
     public class AddProductHandler : IRequestHandler<AddProductCommand>
     {
         private readonly FakeDataStore _fakeDataStore;
         public AddProductHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

         public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
         {
             await _fakeDataStore.AddProduct(request.Product);
             return;
         }
     }
     ```

3. **Add `AddProduct` Method in `ProductsController`**
   ```csharp
   [HttpPost]
   public async Task<ActionResult> AddProduct([FromBody] Product product)
   {
       await _mediator.Send(new AddProductCommand(product));
       return StatusCode(201);
   }


### Benefits

- **Separation of Concerns**: The CQRS pattern divides the application into distinct command and query models, ensuring that read and write operations do not interfere with each other. This leads to a cleaner, more organized codebase.
  
- **Enhanced Performance**: By separating queries and commands, each side can be optimized independently. This allows for more efficient data retrieval and modification, improving overall application performance.

- **Scalability**: Independent scaling of read and write operations becomes possible, enabling better resource management and the ability to handle larger workloads.

- **Maintainability**: The use of the Mediator pattern via MediatR simplifies communication between components, reducing direct dependencies. This results in easier maintenance, testing, and evolution of the code.

- **Loose Coupling**: The Mediator pattern ensures that components are loosely coupled, making the system more flexible and adaptable to change. This is particularly beneficial in large, complex applications.

- **Simplified Testing**: Clear separation of command and query responsibilities, along with centralized handling of interactions, makes it easier to write unit tests and ensure code reliability.
