// Program.cs
using ME_Laboratorio_Programacao.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Configuração de Autenticação por Cookie (Ideal para interagir com Front-End estático via Fetch/Axios)
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "AuthToken";
        options.Cookie.SameSite = SameSiteMode.None; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true; // Segurança contra XSS (JS não consegue ler)
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        // Retorna 401 Unauthorized em vez de redirecionar para uma View de Login inexistente
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// 3. Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Minha API de Autenticação", Version = "v1" });
});

// 4. Configuração do CORS (Ajuste a URL para o endereço do seu Front-end HTML/CSS/JS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndPolicy", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") // Exemplo do Live Server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Obrigatório para enviar e receber Cookies de sessão
    });
});

var app = builder.Build();

// Ambiente de Desenvolvimento com Swagger ativo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEndPolicy");

// A ordem dos middlewares importa!
app.UseAuthentication();
app.UseAuthorization();


app.UseDefaultFiles();   // serve index.html por padrão
app.UseStaticFiles();    // wwwroot
app.MapControllers();

app.Run();