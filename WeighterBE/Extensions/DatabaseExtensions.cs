using Microsoft.EntityFrameworkCore;
using WeighterBE.Data;
using WeighterBE.Models;

namespace WeighterBE.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this IHost app)
        {
            //  in order to work do:
            //  remove Migrations directory
            //  dotnet ef migrations add InitialCreate

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("Starting Database Initialization...");

                bool canConnect = await context.Database.CanConnectAsync();
                logger.LogInformation("Database ConnectionSstatus: {CanConnect}", canConnect);

                if (!canConnect)
                {
                    logger.LogError("Cannot Connect To Database. Check Connection String");
                    
                    return;
                }

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var pendingList = pendingMigrations.ToList();

                if (pendingList.Count != 0)
                {
                    logger.LogInformation("Found {Count} Pending Migrations: {Migrations}", pendingList.Count, string.Join(", ", pendingList));
                }
                else
                {
                    logger.LogInformation("No Pending Migrations");
                }

                logger.LogInformation("Applying Database Migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database Migrations Applied Successfully");


                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
                logger.LogInformation("Applied Migrations Count: {Count}", appliedMigrations.Count());

                bool tablesExist = await CheckTablesExistAsync(context, logger);

                if (tablesExist)
                {
                    logger.LogInformation("Database Tables Verified Successfully");

                    //  optional - await SeedDataAsync(context, logger);
                }
                else
                {
                    logger.LogError("Tables Were Not Created After Migration. Check Migration Files.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An Error Occurred While Initializing The Database");
            }
        }

        private static async Task<bool> CheckTablesExistAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Verifying database schema...");

                // Try to query each table - will fail if they don't exist
                var userCount = await context.Users.CountAsync();
                logger.LogInformation("Users table exists with {Count} records", userCount);

                var weightCount = await context.Weights.CountAsync();
                logger.LogInformation("Weights table exists with {Count} records", weightCount);

                logger.LogInformation("All required tables verified successfully.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error verifying tables: {Message}", ex.Message);
                return false;
            }
        }

        private static async Task SeedDataAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                if (!await context.Users.AnyAsync())
                {
                    logger.LogInformation("Seeding Initial 'users' Data...");

                    await context.Users.AddRangeAsync(
                        new User { Username = "testuser1", Email = "test1@arik.com", Password = "123456" },
                        new User { Username = "testuser2", Email = "test2@arik.com", Password = "abcdefg" }
                    );

                    await context.SaveChangesAsync();
                    logger.LogInformation("'users' Data Seeded successfully");
                }

                if (!await context.Weights.AnyAsync())
                {
                    logger.LogInformation("Seeding Initial 'weights' Data ...");

                    await context.Weights.AddRangeAsync(
                        new Weight { Value = 75.5, WeightAt = DateTime.UtcNow },
                        new Weight { Value = 80.0, WeightAt = DateTime.UtcNow }
                    );

                    await context.SaveChangesAsync();
                    logger.LogInformation("'weights' Data Seeded Successfully");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed Seeding Data");

                return;
            }
        }
    }
}