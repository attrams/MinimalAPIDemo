using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Post CRUD operations using minimal API.
var list = new List<Post>()
{
    new (){Id = 1, Title = "First Post", Content = "Hello World"},
    new (){Id = 2, Title = "Second Post", Content = "Hello Again"},
    new (){Id = 3, Title = "Third Post", Content = "Goodbye World"},
};

app.MapGet("/posts", () => list)
.WithName("GetPosts")
.WithOpenApi()
.WithTags("Posts");

app.MapPost("/posts", (Post post) =>
{
    list.Add(post);
    return Results.Created($"/posts/{post.Id}", post);
})
.WithName("CreatePost")
.WithOpenApi()
.WithTags("Posts");

app.MapGet("/posts/{id}", (int id) =>
{
    var post = list.FirstOrDefault(post => post.Id == id);
    return post is null ? Results.NotFound() : Results.Ok(post);
})
.WithName("GetPost")
.WithOpenApi()
.WithTags("Posts");

app.MapPut("/posts/{id}", (int id, Post post) =>
{
    var index = list.FindIndex(post => post.Id == id);
    if (index == -1)
    {
        return Results.NotFound();
    }

    list[index] = post;

    return Results.Ok(post);
})
.WithName("UpdatePost")
.WithOpenApi()
.WithTags("Posts");

app.MapDelete("/posts/{id}", (int id) =>
{
    var post = list.FirstOrDefault(post => post.Id == id);
    if (post is null)
    {
        return Results.NotFound();
    }
    list.Remove(post);

    return Results.Ok();
})
.WithName("DeletePost")
.WithOpenApi()
.WithTags("Posts");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/// <summary>
/// This represents a post entity.
/// </summary>
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}