﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchyMappingApp.Models;

public class HierarchyDb : DbContext
{
    public DbSet<Person> People { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public HierarchyDb(DbContextOptions<HierarchyDb> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            //.UseTphMappingStrategy();
            //.UseTptMappingStrategy();
            .UseTpcMappingStrategy()
            .Property(person => person.Id)
            .HasDefaultValueSql("NEXT VALUE FOR [PersonsIds]");

        modelBuilder.HasSequence<int>("PersonsIds", builder =>
        {
            builder.StartsAt(4);
        });

        Student p1 = new() { Id = 1, Name ="Roman Roy", Subject = "History" };

        Employee p2 = new() { Id = 2, Name = "Kendall Roy", HireDate = new (year: 2024, month: 4, day: 2) };

        Employee p3 = new() { Id = 3, Name = "Siobhan Roy", HireDate = new(year: 2020, month: 9, day: 12) };

        modelBuilder.Entity<Student>().HasData(p1);

        modelBuilder.Entity<Employee>().HasData(p2, p3);
    }
}
