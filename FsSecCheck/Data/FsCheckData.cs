using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace GuildfordBoroughCouncil.FsSecCheck.Data
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string Machine { get; set; }
        public string Path { get; set; }
        public string Object { get; set; }
        public string Type { get; set; }
        public string Rights { get; set; }
        public bool Inherited { get; set; }
        public string Inheritance { get; set; }
        public string Propagation { get; set; }
        public Guid Session { get; set; }
    }

    public class NoAuthorisation
    {
        [Key]
        public int Id { get; set; }
        public string Machine { get; set; }
        public string Path { get; set; }
        public Guid Session { get; set; }
    }

    public class FsSecCheckData : DbContext
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<NoAuthorisation> NoAuthorisations { get; set; }
    }
}
