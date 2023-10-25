using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace apiEv2oct27.Models;

public partial class ApiContext : DbContext
{
    public ApiContext()
    {
    }

    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }
    public object Ventas { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
  //      => optionsBuilder.UseSqlServer("Server=DESKTOP-I62LDV5\\SQLEXPRESS; Database=API;Trusted_Connection=SSPI;MultipleActiveResultSets=true;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.ToTable("PRODUCTO");

            entity.Property(e => e.IdProducto).HasColumnName("ID_PRODUCTO");
            entity.Property(e => e.DescProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESC_PRODUCTO");
            entity.Property(e => e.Precio).HasColumnName("PRECIO");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.NomUsuario);

            entity.ToTable("USUARIO");

            entity.Property(e => e.NomUsuario)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NOM_USUARIO");
            entity.Property(e => e.Estado).HasColumnName("ESTADO");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.IdVenta);

            entity.ToTable("VENTA");

            entity.Property(e => e.IdVenta).HasColumnName("ID_VENTA");
            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD");
            entity.Property(e => e.Estado).HasColumnName("ESTADO");
            entity.Property(e => e.FechaVenta)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_VENTA");
            entity.Property(e => e.IdProducto).HasColumnName("ID_PRODUCTO");
            entity.Property(e => e.NomUsuario)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NOM_USUARIO");
            entity.Property(e => e.Total).HasColumnName("TOTAL");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VENTA_PRODUCTO");

            entity.HasOne(d => d.NomUsuarioNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.NomUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VENTA_USUARIO");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
