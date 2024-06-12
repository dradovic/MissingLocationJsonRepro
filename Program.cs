using Microsoft.EntityFrameworkCore;

using (var db = new MyDbContext())
{
    db.Database.EnsureCreated();
    db.Boats.ExecuteDelete();

    var location = new Location
    {
        Address1 = "Jumpy Rd. 1",
        City = "Duville",
    };

    var boat1 = new Boat
    {
        Passengers = [location],
    };
    var boat2 = new Boat
    { 
        Passengers = [location],
    };
    var boat3 = new Boat
    {
        Passengers = [location],
    };
    db.Boats.AddRange(boat1, boat2, boat3);
    db.SaveChanges();
}

using (var db = new MyDbContext())
{
    foreach (var boat in db.Boats)
    {
        Console.WriteLine(boat);
        // Boat 8:
        // Boat 9:
        // Boat 10: Jumpy Rd. 1 in Duville
        // Id	Passengers
        // 8    []
        // 9    []
        // 10   [{ "Address1":"Jumpy Rd. 1","City":"Duville"}]
    }
}

public class Location
{
    public string Address1 { get; set; } = null!;
    public string City { get; set; } = null!;
    public override string ToString() => $"{Address1} in {City}";
}

public class Boat
{
    public int Id { get; set; }
    public ICollection<Location> Passengers { get; set; } = null!;
    public override string ToString() => $"Boat {Id}: {string.Join(" ; ", Passengers)}";
}

public class MyDbContext : DbContext
{
    public DbSet<Boat> Boats { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Owned<Location>();

        builder.Entity<Boat>()
            .OwnsMany(b => b.Passengers).ToJson();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Data Source=.;Initial Catalog=MissingLocationJsonRepro;Integrated Security=True;Encrypt=False");
}
