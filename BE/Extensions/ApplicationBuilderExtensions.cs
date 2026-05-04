namespace BE.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseApplication(this WebApplication app)
        {

            app.UseMiddleware<Middlewares.ExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

            }
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            return app;
        }
    }
}
