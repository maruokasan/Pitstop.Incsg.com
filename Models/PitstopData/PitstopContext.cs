using Microsoft.EntityFrameworkCore;

namespace Pitstop.Models.PitstopData;

public partial class PitstopContext : DbContext
{

    public PitstopContext()
    {
    }

    public PitstopContext(DbContextOptions<PitstopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DataControl> DataControls { get; set; }

    public virtual DbSet<EmailSubmitted> EmailSubmitteds { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<System> Systems { get; set; }

    public virtual DbSet<SystemPermission> SystemPermissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSystemMapping> UserSystemMappings { get; set; }
    public virtual DbSet<UserRoles> UserRoles { get; set; }
    public virtual DbSet<UserToken> UserTokens { get; set; }
    public virtual DbSet<Section1Media> Section1Media { get; set; }
    public virtual DbSet<Section2Media> Section2Media { get; set; }
    public virtual DbSet<Product> Product { get; set; }
    public virtual DbSet<FeaturedItem> FeaturedItems { get; set; }
    public virtual DbSet<ProductMedia> ProductMedia{ get; set; }
    public virtual DbSet<OurStorys> OurStorys{ get; set; }
    public virtual DbSet<Cart> Carts{ get; set; }
    public virtual DbSet<Testimonial> Testimonial { get; set; }
    public virtual DbSet<Category> Category{ get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataControl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DataCont__3214EC07CB7D7FDA");

            entity.ToTable("DataControl");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Parent).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmailSubmitted>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailSub__3214EC07758631F9");

            entity.ToTable("EmailSubmitted");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
            entity.HasKey(e => e.Id).HasName("PK_AspNetRoles");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AspNetRoleClaims");
            entity.ToTable("RolePermission");
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
        });

        modelBuilder.Entity<System>(entity =>
        {
            entity.ToTable("System");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CssbackgroundColor)
                .HasMaxLength(50)
                .HasColumnName("CSSBackgroundColor");
            entity.Property(e => e.LogoPath).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(200);
        });

        modelBuilder.Entity<SystemPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemPe__3214EC074514DC92");

            entity.ToTable("SystemPermission");

            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SystemId).HasMaxLength(128);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.System).WithMany(p => p.SystemPermissions)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemRole_SystemRole");
        });

        modelBuilder.Entity<UserRoles>().HasKey(table => new
        {
            table.RoleId,
            table.UserId
        });
        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.ToTable("UserRole");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(e => e.Id).HasName("PK_AspNetUsers");

            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LastAccessDate).HasColumnType("datetime");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(256);

    modelBuilder.Entity<User>()
        .HasMany(u => u.UserRoles)
        .WithOne(ur => ur.User)
        .HasForeignKey(ur => ur.UserId);
        });

        modelBuilder.Entity<UserSystemMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSyst__3214EC070BAC4160");

            entity.ToTable("UserSystemMapping");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.SystemId).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.System).WithMany(p => p.UserSystemMappings)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemIdSystemMapping");

            entity.HasOne(d => d.User).WithMany(p => p.UserSystemMappings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserIdSystemMapping");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserToke__3214EC072722039A");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LoginProvider).HasMaxLength(450);
            entity.Property(e => e.Name).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserIdUserToken");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
