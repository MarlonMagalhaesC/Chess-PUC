var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços para o controlador e documentação Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors();


// Ativa Swagger na interface
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configura HTTPS e roteamento
app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia controllers (isso é FUNDAMENTAL!)
app.MapControllers();

app.Run();
