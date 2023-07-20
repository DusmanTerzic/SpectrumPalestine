﻿// <auto-generated />
using FrekvencijeProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FrekvencijeProject.Migrations.AllocationTermsDB
{
    [DbContext(typeof(AllocationTermsDBContext))]
    partial class AllocationTermsDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FrekvencijeProject.Models.AllocationTerms.AllocationTermDb", b =>
                {
                    b.Property<int>("AllocationTermId")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AllocationTermId");

                    b.ToTable("AllocationTermDb");
                });
#pragma warning restore 612, 618
        }
    }
}
