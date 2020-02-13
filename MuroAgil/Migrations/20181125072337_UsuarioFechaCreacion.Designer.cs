﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MuroAgil.Models;
using System;

namespace MuroAgil.Migrations
{
    [DbContext(typeof(MuroAgilContext))]
    [Migration("20181125072337_UsuarioFechaCreacion")]
    partial class UsuarioFechaCreacion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MuroAgil.Models.Etapa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<int>("IdMuro")
                        .HasColumnName("ID_MURO");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnName("NOMBRE")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<short>("Posicion")
                        .HasColumnName("POSICION");

                    b.HasKey("Id");

                    b.HasIndex("IdMuro");

                    b.ToTable("ETAPA");
                });

            modelBuilder.Entity("MuroAgil.Models.Muro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnName("FECHA_CREACION")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("FechaUltimaModificacion")
                        .HasColumnName("FECHA_ULTIMA_MODIFICACION")
                        .HasColumnType("datetime");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnName("NOMBRE")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("MURO");
                });

            modelBuilder.Entity("MuroAgil.Models.Tarea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<short>("Blue")
                        .HasColumnName("BLUE");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnName("DESCRIPCION")
                        .IsUnicode(false);

                    b.Property<short>("Familia")
                        .HasColumnName("FAMILIA");

                    b.Property<short>("Green")
                        .HasColumnName("GREEN");

                    b.Property<int>("IdEtapa")
                        .HasColumnName("ID_ETAPA");

                    b.Property<short>("Posicion")
                        .HasColumnName("POSICION");

                    b.Property<short>("Red")
                        .HasColumnName("RED");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnName("TITULO")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("IdEtapa");

                    b.ToTable("TAREA");
                });

            modelBuilder.Entity("MuroAgil.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnName("CORREO")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnName("FECHA_CREACION")
                        .HasColumnType("datetime");

                    b.Property<string>("HashContrasenna")
                        .IsRequired()
                        .HasColumnName("HASH_CONTRASENNA")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("HashValidacionCorreo")
                        .HasColumnName("HASH_VALIDACION")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnName("NOMBRE")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Correo")
                        .IsUnique()
                        .HasName("UQ__USUARIO__CC87E1260BB751F7");

                    b.ToTable("USUARIO");
                });

            modelBuilder.Entity("MuroAgil.Models.UsuarioMuro", b =>
                {
                    b.Property<int>("IdDuenno")
                        .HasColumnName("ID_DUENNO");

                    b.Property<int>("IdMuro")
                        .HasColumnName("ID_MURO");

                    b.Property<short>("Permiso")
                        .HasColumnName("PERMISO");

                    b.HasKey("IdDuenno", "IdMuro");

                    b.HasIndex("IdMuro");

                    b.ToTable("USUARIO_MURO");
                });

            modelBuilder.Entity("MuroAgil.Models.Etapa", b =>
                {
                    b.HasOne("MuroAgil.Models.Muro", "IdMuroNavigation")
                        .WithMany("Etapa")
                        .HasForeignKey("IdMuro")
                        .HasConstraintName("FK__ETAPA__ID_MURO__22401542")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MuroAgil.Models.Tarea", b =>
                {
                    b.HasOne("MuroAgil.Models.Etapa", "Etapa")
                        .WithMany("Tarea")
                        .HasForeignKey("IdEtapa")
                        .HasConstraintName("FK__TAREA__ID_ETAPA__251C81ED")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MuroAgil.Models.UsuarioMuro", b =>
                {
                    b.HasOne("MuroAgil.Models.Usuario", "IdDuennoNavigation")
                        .WithMany("UsuarioMuro")
                        .HasForeignKey("IdDuenno")
                        .HasConstraintName("FK__USUARIO_M__ID_DU__1E6F845E")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MuroAgil.Models.Muro", "IdMuroNavigation")
                        .WithMany("UsuarioMuro")
                        .HasForeignKey("IdMuro")
                        .HasConstraintName("FK__USUARIO_M__ID_MU__1F63A897")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
