﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApp;

#nullable disable

namespace TodoApp.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20220706160410_202207071")]
    partial class _202207071
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TodoApp.Entities.TodoItem", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)")
                        .HasComment("姓名");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)")
                        .HasComment("事情");

                    b.HasKey("Id");

                    b.ToTable("TodoItem", (string)null);
                });

            modelBuilder.Entity("TodoApp.Entities.TodoTest", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasComment("id");

                    b.Property<string>("Test")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)")
                        .HasComment("测试");

                    b.HasKey("Id");

                    b.ToTable("TodoTest", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
