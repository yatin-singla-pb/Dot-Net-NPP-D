using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Middleware;
using SendGrid;
using OfficeOpenXml;
using Serilog;

// Set EPPlus license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for file + console logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/npp-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddHttpContextAccessor();

// Configure Entity Framework with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=DESKTOP-0EM04K6;Database=InterflexNPP;Uid=sa;Password=software@123;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<IDistributorRepository, DistributorRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractVersionRepository, ContractVersionRepository>();
builder.Services.AddScoped<IIndustryRepository, IndustryRepository>();
builder.Services.AddScoped<IOpCoRepository, OpCoRepository>();
builder.Services.AddScoped<IMemberAccountRepository, MemberAccountRepository>();
builder.Services.AddScoped<ICustomerAccountRepository, CustomerAccountRepository>();
builder.Services.AddScoped<IDistributorProductCodeRepository, DistributorProductCodeRepository>();
builder.Services.AddScoped<IUserManufacturerRepository, UserManufacturerRepository>();

builder.Services.AddScoped<IContractPriceRepository, ContractPriceRepository>();
builder.Services.AddScoped<IContractIndustryVersionRepository, ContractIndustryVersionRepository>();
builder.Services.AddScoped<IContractManufacturerVersionRepository, ContractManufacturerVersionRepository>();
builder.Services.AddScoped<IContractOpCoVersionRepository, ContractOpCoVersionRepository>();
builder.Services.AddScoped<IContractDistributorVersionRepository, ContractDistributorVersionRepository>();
builder.Services.AddScoped<IContractVersionProductRepository, ContractVersionProductRepository>();
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
builder.Services.AddScoped<IVelocityRepository, VelocityRepository>();

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IDistributorService, DistributorService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IIndustryService, IndustryService>();
builder.Services.AddScoped<IContractIndustryVersionService, ContractIndustryVersionService>();
builder.Services.AddScoped<IContractManufacturerVersionService, ContractManufacturerVersionService>();
builder.Services.AddScoped<IContractOpCoVersionService, ContractOpCoVersionService>();
builder.Services.AddScoped<IContractDistributorVersionService, ContractDistributorVersionService>();
builder.Services.AddScoped<IContractVersionProductService, ContractVersionProductService>();

builder.Services.AddScoped<IOpCoService, OpCoService>();
builder.Services.AddScoped<IProposalService, ProposalService>();

builder.Services.AddScoped<IMemberAccountService, MemberAccountService>();
builder.Services.AddScoped<ICustomerAccountService, CustomerAccountService>();
builder.Services.AddScoped<IDistributorProductCodeService, DistributorProductCodeService>();
builder.Services.AddScoped<IContractAssignmentService, ContractAssignmentService>();

builder.Services.AddScoped<IContractPriceService, ContractPriceService>();
builder.Services.AddScoped<IVelocityService, VelocityService>();
builder.Services.AddScoped<IVelocityCsvParser, VelocityCsvParser>();
builder.Services.AddScoped<IVelocityExcelParser, VelocityExcelParser>();
builder.Services.AddScoped<IVelocityUsageReportService, VelocityUsageReportService>();
builder.Services.AddHostedService<VelocityBackgroundProcessor>(); // Auto-resume incomplete jobs on startup
builder.Services.AddScoped<IBulkRenewalService, BulkRenewalService>();
builder.Services.AddScoped<IProposalProductExcelService, ProposalProductExcelService>();
builder.Services.AddScoped<IContractOverTermReportService, ContractOverTermReportService>();
builder.Services.AddScoped<IContractPricingReportService, ContractPricingReportService>();
builder.Services.AddScoped<IConflictDetectionService, ConflictDetectionService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Configure SendGrid
var sendGridApiKey = builder.Configuration["SendGrid:ApiKey"] ?? "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";
builder.Services.AddSingleton<ISendGridClient>(provider => new SendGridClient(sendGridApiKey));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // Allow both local development and production origins
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4201",
                "http://localhost:4202",
                "http://34.9.77.60:8080",  // Production frontend
                "http://34.9.77.60:4200"   // Alternative production port
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NPP Contract Management API",
        Version = "v1.0",
        Description = "API for NPP Contract Management System",
        Contact = new OpenApiContact
        {
            Name = "NPP Contract Management Team",
            Email = "support@nppcontractmanagement.com"
        }
    });

        // Use fully-qualified names to avoid schema ID conflicts (e.g., multiple 'CreateRequest' types)
        c.CustomSchemaIds(t => t.FullName);


    // Configure JWT Authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Seed admin user if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Apply pending migrations on startup
    try { context.Database.Migrate(); } catch (Exception ex) { logger.LogError(ex, "Error applying migrations on startup"); }

    try
    {
        // Check if admin user exists
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == "admin");
        if (adminUser == null)
        {
            logger.LogInformation("Creating default admin user...");

            // Create admin user
            var user = new User
            {
                UserId = "admin",
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@nppcontractmanagement.com",
                PasswordHash = "$2a$12$A2njs.w2dAu/Lamur/KBFuRb71mU/a0qHMIJOxOLJ1LZw..1SyMqG",
                IsActive = true,
                EmailConfirmed = true,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assign System Administrator role
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "System Administrator");
            if (adminRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = adminRole.Id,
                    AssignedBy = "System",
                    AssignedDate = DateTime.UtcNow
                };

                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();

                logger.LogInformation("Admin user created successfully with System Administrator role");
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating admin user");
    }
}

// Configure the HTTP request pipeline.

// Only force HTTPS redirection in production so local HTTP (e.g., http://localhost:5143)
// can be used by the Angular app without being redirected to HTTPS with a dev certificate.
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NPP Contract Management API v1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    c.DocumentTitle = "NPP Contract Management API";
    c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapse operations by default
});

app.UseGlobalExceptionHandler();
app.UseRequestLoggingWithBody();
app.Use(async (ctx, next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    await next();
    sw.Stop();
    var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RequestLogger");
    logger.LogInformation("{Method} {Path} => {StatusCode} in {Elapsed}ms", ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
});

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();