using Microsoft.EntityFrameworkCore;

using (var db = new MyDbContext())
{
    db.Database.EnsureCreated();
    db.Persons.ExecuteDelete();
    db.Boats.ExecuteDelete();

    var joe = new Person
    {
        Name = "Joe Doe",
        Location = new Location
        {
            Address1 = "Jumpy Rd. 1",
            City = "Duville",
        },
    };
    db.Persons.Add(joe);

    var boat1 = new Boat
    {
        Passengers = [new PassengerInfo { Name = joe.Name, Location = joe.Location }],
    };
    var boat2 = new Boat
    { 
        Passengers = [new PassengerInfo { Name = joe.Name, Location = joe.Location }],
    };
    var boat3 = new Boat
    {
        Passengers = [new PassengerInfo { Name = joe.Name, Location = joe.Location }],
    };
    db.Boats.AddRange(boat1, boat2, boat3);
    db.SaveChanges();
}

using (var db = new MyDbContext())
{
    foreach (var boat in db.Boats)
    {
        Console.WriteLine(boat);
        // Boat 5: Passenger: Joe Doe @
        // Boat 6: Passenger: Joe Doe @
        // Boat 7: Passenger: Joe Doe @ Jumpy Rd. 1 in Duville
        // Id	Passengers
        // 5    [{ "Name":"Joe Doe","Location":null}]
        // 6    [{ "Name":"Joe Doe","Location":null}]
        // 7    [{ "Name":"Joe Doe","Location":{ "Address1":"Jumpy Rd. 1","City":"Duville"} }]
    }
}

public class Location
{
    public string Address1 { get; set; } = null!;
    public string City { get; set; } = null!;
    public override string ToString() => $"{Address1} in {City}";
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public override string ToString() => $"Person {Id}: {Name} @ {Location}";
}

public class PassengerInfo
{
    public string Name { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public override string ToString() => $"Passenger: {Name} @ {Location}";
}

public class Boat
{
    public int Id { get; set; }
    public ICollection<PassengerInfo> Passengers { get; set; } = null!;
    public override string ToString() => $"Boat {Id}: {string.Join(" ; ", Passengers)}";
}

public class MyDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; } = null!;
    public DbSet<Boat> Boats { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Owned<Location>();

        builder.Entity<Boat>()
            .OwnsMany(b => b.Passengers).ToJson();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Data Source=.;Initial Catalog=MissingLocationJsonRepro;Integrated Security=True;Encrypt=False");
}
