using ars_portal6.Models;
using ars_portal6.Models.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
/// <summary>
/// Gets the edm model.
/// </summary>
/// <returns></returns>
static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    var types = GetTypes();

    foreach (Type item in types)
    {
        EntityTypeConfiguration entityType = builder.AddEntityType(item);
        builder.AddEntitySet(item.Name, entityType);
    }

    EdmModel model = (EdmModel)builder.GetEdmModel();

    foreach (Type item in types)
    {
        IEdmEntityType edmEntity = (IEdmEntityType)model.FindDeclaredType(item.FullName);
        DisplayAttribute itemdisplayattribute = item.GetCustomAttribute<DisplayAttribute>();
        AddAnnotations(model, (EdmElement)edmEntity, itemdisplayattribute);
        foreach (PropertyInfo propertyInfo in item.GetProperties())
        {
            EdmElement property = (EdmElement)edmEntity.FindProperty(propertyInfo.Name);
            DisplayAttribute displayattribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            if (property != null)
                AddAnnotations(model, property, displayattribute);
        }
    }
    return model;
}
/// <summary>
/// Adds the annotations.
/// </summary>
/// <param name="model">The model.</param>
/// <param name="edmElement">The edm element.</param>
/// <param name="displayattribute">The displayattribute.</param>
static void AddAnnotations(EdmModel model, EdmElement edmElement, DisplayAttribute displayattribute)
{
    if (displayattribute != null)
    {
        var stringType = EdmCoreModel.Instance.GetString(true);
        var value = new EdmStringConstant(stringType, displayattribute.Name);
        model.SetAnnotationValue(edmElement, "", "label", value);
        stringType = EdmCoreModel.Instance.GetString(true);
        value = new EdmStringConstant(stringType, displayattribute.Description);
        model.SetAnnotationValue(edmElement, "", "quickinfo", value);
    }
}
/// <summary>
/// Gets the types.
/// </summary>
/// <returns></returns>
static List<Type> GetTypes()
{
    return new List<Type>()
            {
                typeof(EmployeesDetailedData),
                typeof(EmployeesPrimaryData),
                typeof(Skill)
            };
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbContextEmployees>(options =>
{
    string dbConn = "Server=P3NWPLSK12SQL-v09.shr.prod.phx3.secureserver.net,1433;Initial Catalog=ph19899789431_;Persist Security Info=False;User ID=dev7216;Password=Anjurs21@06;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
    options.UseSqlServer(dbConn);

}, ServiceLifetime.Transient);
builder.Services.AddControllers().AddOData(opt => opt.AddRouteComponents(GetEdmModel()).Filter().Select().Expand());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ars_portal", Version = "v1" });

});
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ODataTutorial v1"));

}
app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
