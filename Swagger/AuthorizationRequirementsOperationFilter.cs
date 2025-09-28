using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace SocialWebsite.Swagger;

/// <summary>
/// OperationFilter để hiển thị yêu cầu Authorization/Role trên Swagger
/// </summary>
public class AuthorizationRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        if (metadata is null || metadata.Count == 0)
            return;

        bool allowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();
        var authorizeAttributes = metadata.OfType<AuthorizeAttribute>().ToList();
        if (allowAnonymous || authorizeAttributes.Count == 0)
            return;

        // Đánh dấu yêu cầu bảo mật Bearer
        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [ new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            } ] = new List<string>()
        });

        // Trích xuất policy yêu cầu role tối thiểu
        var policies = authorizeAttributes
            .Select(a => a.Policy)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        var rolePolicies = policies
            .Where(p => p!.StartsWith("RequireRole_", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        if (rolePolicies.Count > 0)
        {
            var requiredRoles = string.Join(", ", rolePolicies.Select(p => p!.Replace("RequireRole_", "")));
            var note = $"Required minimum role: {requiredRoles}.";

            if (string.IsNullOrWhiteSpace(operation.Description))
                operation.Description = note;
            else
                operation.Description += $"\n\n{note}";
        }
    }
}
