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
        Location = location,
    };
    var boat2 = new Boat
    { 
        Location = location,
    };
    var boat3 = new Boat
    {
        Location = location,
    };
    db.Boats.AddRange(boat1, boat2, boat3);
    db.SaveChanges();
    // SqlException: Cannot insert the value NULL into column 'Passenger', table 'MissingLocationJsonRepro.dbo.Boats'; column does not allow nulls.INSERT fails.
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
    public Location Location { get; set; } = null!;
    public override string ToString() => $"Boat {Id}: {Location}";
}

public class MyDbContext : DbContext
{
    public DbSet<Boat> Boats { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Owned<Location>();

        builder.Entity<Boat>()
            .OwnsOne(b => b.Location).ToJson();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Data Source=.;Initial Catalog=MissingLocationJsonRepro;Integrated Security=True;Encrypt=False");
}
