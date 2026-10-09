using AssetTracker.Data;
using Core.Interfaces;
using Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Interfaces;
using UI.Forms;

namespace UI
{
    public partial class frmSplash : Form
    {
        public frmSplash()
        {
            InitializeComponent();
            
                this.Shown += FrmSplash_Shown!;

            
            

        }
        private async void FrmSplash_Shown(object sender, EventArgs e)
        {
            await InitializeApp();
        }

        private async Task InitializeApp()
        {
            await Task.Run(async () =>
            {
                // Create Database Folder
                var dbFolder = Path.Combine(AppContext.BaseDirectory, "Database");
                Directory.CreateDirectory(dbFolder);

                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        var connection = context.Configuration.GetConnectionString("DefaultConnection");

                        services.AddDbContext<AppDbContext>(options =>
                            options.UseSqlite(connection));
                        services.AddTransient<frmLogin>();
                        services.AddScoped<IAuthenticationService, AuthenticationService>();
                        services.AddScoped<IPersonService, PersonService>();
                    })
                    .Build();

                Program.ServiceProvider = host.Services;

                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await db.Database.MigrateAsync();
                   

                    await SeedData.InitializeAsync(db);
                }
            });

            var mainForm = Program.ServiceProvider!.GetRequiredService<frmLogin>();
            this.Hide();
            mainForm.ShowDialog();
            this.Close();
        }
      
    }
}
