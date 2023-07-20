﻿// <auto-generated />
using FrekvencijeProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FrekvencijeProject.Migrations.ApplicationTermsDB
{
    [DbContext(typeof(ApplicationTermsDBContext))]
    [Migration("20220718142750_AddNewTableApplicationTerm")]
    partial class AddNewTableApplicationTerm
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FrekvencijeProject.Models.ApplicationTerms.ApplicationTermsDB", b =>
                {
                    b.Property<int>("ApplicationTermsDBId")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApplicationTermsDBId");

                    b.ToTable("RootApplicationTermsDB");
                });
#pragma warning restore 612, 618
        }
    }
}