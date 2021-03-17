﻿// <auto-generated />
using System;
using MarketMaker.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketMaker.Infrastructure.Migrations
{
    [DbContext(typeof(MarketMakerContext))]
    [Migration("20190926075829_InitMigration")]
    partial class InitMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MarketMaker.Domain.Entities.Common", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("GUID");

                    b.Property<int>("Status");

                    b.Property<long>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("Id");

                    b.ToTable("Commons");
                });

            modelBuilder.Entity("MarketMaker.Domain.Entities.MarketMakerPreference", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BuyDownPercentage");

                    b.Property<int>("BuyDownThreshold");

                    b.Property<long>("BuyLTPPrefProID");

                    b.Property<int>("BuyUpPercentage");

                    b.Property<int>("BuyUpThreshold");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("GUID");

                    b.Property<int>("PairId");

                    b.Property<int>("RangeType");

                    b.Property<int>("SellDownPercentage");

                    b.Property<int>("SellDownThreshold");

                    b.Property<long>("SellLTPPrefProID");

                    b.Property<int>("SellUpPercentage");

                    b.Property<int>("SellUpThreshold");

                    b.Property<int>("Status");

                    b.Property<long>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("MarketMakerPreferences");
                });

            modelBuilder.Entity("MarketMaker.Domain.Entities.MarketMakerRangeDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("GUID");

                    b.Property<long>("PreferenceId");

                    b.Property<float>("RangeMax");

                    b.Property<float>("RangeMin");

                    b.Property<int>("Status");

                    b.Property<long>("UpdatedBy");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("Id");

                    b.ToTable("MarketMakerRangeDetails");
                });
#pragma warning restore 612, 618
        }
    }
}