using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomerRepository>();    //add the Repository as a singelton in the app live time

var app = builder.Build();

app.MapGet("/customers", ([FromServices] CustomerRepository repo) =>
{
    return repo.GetAll();
});

app.MapGet("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id) =>
{
    var customer = repo.GetById(id);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
});

app.MapPost("/customers", ([FromServices] CustomerRepository repo, Customer customer) =>
{
    repo.Create(customer);
    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapPut("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id, Customer updateCustomer) =>
{
    var customer = repo.GetById(id);
    if (customer is null)
        return Results.NotFound();

    repo.Update(updateCustomer);
    return Results.Ok(updateCustomer);
});

app.MapDelete("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id) =>
{
    repo.Delete(id);
    return Results.Ok();
});

app.Run();

record Customer(Guid Id, string FullName);

class CustomerRepository    //data access layer
{
    private readonly Dictionary<Guid, Customer> _customers = new(); // fake "database" :)

    public void Create(Customer customer)
    {
        if (customer is null)
            return;
        _customers[customer.Id] = customer;
    }

    public Customer GetById(Guid id)
    {
        return _customers[id];
    }

    public List<Customer> GetAll()
    {
        return _customers.Values.ToList();
    }

    public void Update(Customer customer)
    {
        var existingCustomer = GetById(customer.Id);
        if (existingCustomer is null)
            return;
        _customers[customer.Id] = customer;
    }

    public void Delete(Guid id)
    {
        _customers.Remove(id);
    }
}