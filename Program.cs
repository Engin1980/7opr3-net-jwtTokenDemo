using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opt =>
{
  opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
  opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
  {
    IssuerSigningKey = key,
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    ClockSkew= TimeSpan.Zero
  };
}
  );

var app = builder.Build();

app.UseRouting();
//app.UseAuthentication(); //TODO what is this good for?
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//app.MapControllers();
app.Run();

/*
 * var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<AppUserService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
  options =>
  {
    options.TokenValidationParameters = new()
    {
      ValidateIssuer = false,
      ValidateAudience = false,
      ValidateLifetime = false,
      ValidateIssuerSigningKey = false,
      ValidIssuer = AppUserService.ISSUER,
      ValidAudience = AppUserService.AUDIENCE,
      IssuerSigningKey = AppUserService.GetSigningKey()
    };
  });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();

  app.UseDeveloperExceptionPage();
}


app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();

*/
