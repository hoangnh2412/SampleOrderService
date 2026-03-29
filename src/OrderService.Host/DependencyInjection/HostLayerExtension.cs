using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderService.Application.Abstractions;
using OrderService.Application.DependencyInjection;
using OrderService.Domain.Shared.Enums;
using OrderService.Domain.Shared.Extensions;
using OrderService.Host.Filters;
using OrderService.Host.Models;
using OrderService.Host.Security;
using OrderService.Infrastructure.DependencyInjection;
using OrderService.Host.Mapping;
using OrderService.Host.Swagger;

namespace OrderService.Host.DependencyInjection;

public static class HostLayerExtension
{
    public static WebApplicationBuilder AddHostLayer(this WebApplicationBuilder builder)
    {
        builder.AddApplicationLayer();
        builder.AddInfrastructureLayer();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserContext, HttpUserContext>();
        builder.Services.AddSingleton<JwtTokenIssuer>();
        builder.Services.AddAutoMapper(typeof(OrderApiMappingProfile).Assembly);

        var jwt = builder.Configuration.GetSection("Jwt");
        var signingKey = jwt["SigningKey"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:SigningKey' is required.");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        var http = context.HttpContext;
                        var requestId = http.Items["RequestId"] as string ?? Guid.NewGuid().ToString();
                        http.Response.Headers.TryAdd("RequestId", requestId);
                        http.Response.Headers.TryAdd("TraceId", Activity.Current?.Id ?? http.TraceIdentifier);
                        http.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        http.Response.ContentType = "application/json";
                        await http.Response.WriteAsJsonAsync(new ResponseEnvelop
                        {
                            Code = ErrorCodes.Unauthorized.ToWireCode(),
                            Message = "Unauthorized",
                            Errors = []
                        });
                    }
                };
            });
        builder.Services.AddAuthorization();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(kvp => kvp.Value is { Errors.Count: > 0 })
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e =>
                    {
                        string message;
                        if (string.IsNullOrEmpty(e.ErrorMessage))
                            message = "Invalid value.";
                        else
                            message = e.ErrorMessage;
                        return new FieldErrorDto(kvp.Key, message);
                    }))
                    .ToList();
                return new BadRequestObjectResult(new ResponseEnvelop
                {
                    Code = ErrorCodes.InvalidRequestBody.ToWireCode(),
                    Message = "Invalid request body",
                    Errors = errors
                });
            };
        });

        builder.Services.AddControllers(options => options.Filters.Add<CorrelationHeadersFilter>());

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT từ POST /api/v1/auth/token — dán giá trị `data.accessToken` (Swagger tự thêm tiền tố Bearer).",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
            var xml = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xml))
                options.IncludeXmlComments(xml, includeControllerXmlComments: true);
            options.ParameterFilter<IdempotentIdHeaderParameterFilter>();
        });

        return builder;
    }

    /// <summary>
    /// Swagger / Swagger UI (chỉ Development): mỗi lần Try it out POST <c>/api/v1/orders</c> tự gắn header <c>IdempotentId</c> = UUID mới.
    /// </summary>
    public static WebApplication UseHostSwaggerInDevelopment(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service v1");
            // OpenAPI default/example (IdempotentIdHeaderParameterFilter) để ô header có sẵn UUID khi Try it out, tránh lỗi required trên UI.
            // Interceptor ghi đè lại UUID mỗi lần Execute POST để test idempotent sạch.
            options.UseRequestInterceptor(
                "(req) => { var u = typeof req.url === 'string' ? req.url : ''; " +
                "if (req.method === 'POST' && u.indexOf('/api/v1/orders') !== -1) { " +
                "req.headers['IdempotentId'] = crypto.randomUUID(); } return req; }");
        });

        return app;
    }
}
