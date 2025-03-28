﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250326071751_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Api.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("FirebaseId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("firebase_id");

                    b.ComplexProperty<Dictionary<string, object>>("Email", "Api.Domain.Users.User.Email#Email", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("email");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Name", "Api.Domain.Users.User.Name#Name", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("name");
                        });

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
