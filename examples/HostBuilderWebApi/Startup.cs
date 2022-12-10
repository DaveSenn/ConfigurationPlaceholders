public sealed class Startup
{
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure( IApplicationBuilder app )
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();

        app.UseEndpoints( endpoints => { endpoints.MapControllerRoute( "default", "{controller=Home}/{action=Index}" ); } );
    }

    public void ConfigureServices( IServiceCollection services )
    {
        services.AddMvc();
    }
}