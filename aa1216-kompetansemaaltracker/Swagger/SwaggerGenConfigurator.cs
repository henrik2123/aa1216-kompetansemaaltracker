using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace aa1216_kompetansemaaltracker.Swagger;

public class SwaggerGenConfigurator(IApiVersionDescriptionProvider apiVersionDescriptionProvider, IConfiguration config) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions c)
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions) 
        {
            c.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "aa1216_kompetansemaaltracker",
                Version = description.ApiVersion.ToString()
            });
        }

        c.AddSecurityDefinition("Azure AD", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize"),
                    TokenUrl = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { $"api://{config["AzureAd:ClientId"]}/user_impersonation", "Access aa1216_kompetansemaaltracker" }
                    }
                }
            },
            Description = "`Leave client_secret blank`",
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Azure AD" }
                },
                new[] { $"api://{config["AzureAd:ClientId"]}/user_impersonation" }
            }
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    }
}
