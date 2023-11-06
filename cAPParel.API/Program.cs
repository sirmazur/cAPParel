using cAPParel.API.DbContexts;
using cAPParel.API.Entities;
using cAPParel.API.Services.Basic;
using cAPParel.API.Services.CategoryServices;
using cAPParel.API.Services.FieldsValidationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;
using Marvin.Cache.Headers;
using cAPParel.API.Services.UserServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = 
    new CamelCasePropertyNamesContractResolver();
})
.AddXmlDataContractSerializerFormatters()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetailsFactory = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                       context.HttpContext,
                                  context.ModelState);

        problemDetails.Type = "https://capparel.com/modelvalidationproblem";
        problemDetails.Title = "One or more model validation errors occurred.";
        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
        problemDetails.Detail = "See the errors property for details.";
        problemDetails.Instance = context.HttpContext.Request.Path;

        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.Configure<MvcOptions>(options =>
{
    var newtonsoftJsonOutputFormatter = options.OutputFormatters
        .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

    //if (newtonsoftJsonOutputFormatter != null)
    //{
    //    newtonsoftJsonOutputFormatter.SupportedMediaTypes
    //        .Add("application/vnd.capparel.hateoas+json");
    //}
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IFieldsValidationService, FieldsValidationService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBasicRepository<Category>, BasicRepository<Category>>();
builder.Services.AddScoped<IBasicRepository<User>, BasicRepository<User>>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<cAPParelContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:cAPParelDbConnectionString"]));
builder.Services.AddResponseCaching();
builder.Services.AddHttpCacheHeaders(
       (expirationModelOptions) =>
       {
        expirationModelOptions.MaxAge = 180;
        expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
       },
       (validationModelOptions) =>
       {
        validationModelOptions.MustRevalidate = true;
       });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpCacheHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
