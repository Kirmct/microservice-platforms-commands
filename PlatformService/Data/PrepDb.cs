using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    //in this class we are populating the inMemory database without migrations
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var servicesScope = app.ApplicationServices.CreateScope())
        {
            //we get the data context and pass
            //we dont use constructur to this cause its a static class
            SeedData(servicesScope.ServiceProvider.GetService<AppDbContext>());
        }
    }

    private static void SeedData(AppDbContext context)
    {
        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding Data...");
            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernets", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data");
        }
    }
}