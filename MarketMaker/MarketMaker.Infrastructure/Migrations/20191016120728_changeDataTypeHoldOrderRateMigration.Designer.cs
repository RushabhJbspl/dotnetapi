// <auto-generated />
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
    [Migration("20191016120728_changeDataTypeHoldOrderRateMigration")]
    partial class changeDataTypeHoldOrderRateMigration
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

                    b.Property<decimal>("BuyDownPercentage")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("decimal(28,18)");

                    b.Property<long>("BuyLTPPrefProID");

                    b.Property<int>("BuyLTPRangeType");

                    b.Property<decimal>("BuyThreshold")
                        .HasColumnType("decimal(28,18)");

                    b.Property<decimal>("BuyUpPercentage")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("decimal(28,18)");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("GUID");

                    b.Property<string>("HoldOrderRateChange")
                        .HasColumnType("varchar(200)");

                    b.Property<long>("PairId");

                    b.Property<decimal>("SellDownPercentage")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("decimal(28,18)");

                    b.Property<long>("SellLTPPrefProID");

                    b.Property<int>("SellLTPRangeType");

                    b.Property<decimal>("SellThreshold")
                        .HasColumnType("decimal(28,18)");

                    b.Property<decimal>("SellUpPercentage")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("decimal(28,18)");

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

                    b.Property<decimal>("RangeMax");

                    b.Property<decimal>("RangeMin");

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
