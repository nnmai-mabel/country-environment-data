using Assig1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assig1.Data
{
    public partial class EnvDataContext : DbContext
    {
        public EnvDataContext()
        {
        }

        public EnvDataContext(DbContextOptions<EnvDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AirQualityData> AirQualityData { get; set; } = null!;
        public virtual DbSet<AirQualityStation> AirQualityStations { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<CountryEmission> CountryEmissions { get; set; } = null!;
        public virtual DbSet<Element> Elements { get; set; } = null!;
        public virtual DbSet<Item> Items { get; set; } = null!;
        public virtual DbSet<ItemElement> ItemElements { get; set; } = null!;
        public virtual DbSet<MonitorStationType> MonitorStationTypes { get; set; } = null!;
        public virtual DbSet<Region> Regions { get; set; } = null!;
        public virtual DbSet<TemperatureData> TemperatureData { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirQualityData>(entity =>
            {
                entity.HasKey(e => e.AqdId)
                    .HasName("AirQualityData_PK");

                entity.HasIndex(e => e.RowId, "AirQualityData_UK")
                    .IsUnique();

                entity.Property(e => e.AqdId).HasColumnName("aqdID");

                entity.Property(e => e.AnnualMean).HasColumnName("annualMean");

                entity.Property(e => e.AnnualMeanPm10)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("annualMeanPM10");

                entity.Property(e => e.AnnualMeanPm25)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("annualMeanPM25");

                entity.Property(e => e.AnnualMeanUgm3).HasColumnName("annualMean_ugm3");

                entity.Property(e => e.CityId).HasColumnName("cityID");

                entity.Property(e => e.DbYear).HasColumnName("dbYear");

                entity.Property(e => e.Reference)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("reference");

                entity.Property(e => e.RowId).HasColumnName("rowID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TemporalCoverage1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TemporalCoverage2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.AirQualityData)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AirQualityData_Cities_FK");
            });

            modelBuilder.Entity<AirQualityStation>(entity =>
            {
                entity.HasKey(e => new { e.StationTypeId, e.AqdId })
                    .HasName("AirQualityStations_PK");

                entity.Property(e => e.StationTypeId).HasColumnName("stationTypeID");

                entity.Property(e => e.AqdId).HasColumnName("aqdID");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.HasOne(d => d.Aqd)
                    .WithMany(p => p.AirQualityStations)
                    .HasForeignKey(d => d.AqdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AirQualityStations_Data");

                entity.HasOne(d => d.StationType)
                    .WithMany(p => p.AirQualityStations)
                    .HasForeignKey(d => d.StationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AirQualityStations_Stations_FK");
            });


            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.CityId).HasColumnName("cityID");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("cityName");

                entity.Property(e => e.CountryId).HasColumnName("countryID");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cities_countries_FK");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(e => e.CountryName, "countries_countryName_UK")
                    .IsUnique();

                entity.Property(e => e.CountryId).HasColumnName("countryID");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("countryName");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.Iso3)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("iso3");

                entity.Property(e => e.RegionId).HasColumnName("regionID");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.Countries)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("countries_Region_FK");
            });

            modelBuilder.Entity<CountryEmission>(entity =>
            {
                entity.HasKey(e => e.CeId)
                    .HasName("countryEmissions_PK");

                entity.Property(e => e.CeId).HasColumnName("ceID");

                entity.Property(e => e.CountryId).HasColumnName("countryID");

                entity.Property(e => e.ElementId).HasColumnName("elementID");

                entity.Property(e => e.ItemId).HasColumnName("itemID");

                entity.Property(e => e.Value)
                    .HasColumnType("decimal(15, 4)")
                    .HasColumnName("value");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CountryEmissions)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("countryEmissions_country_FK");

                entity.HasOne(d => d.ItemElement)
                    .WithMany(p => p.CountryEmissions)
                    .HasForeignKey(d => new { d.ItemId, d.ElementId })
                    .HasConstraintName("countryEmissions_ItemElement_FK");
            });

            modelBuilder.Entity<Element>(entity =>
            {
                entity.HasIndex(e => e.ElementName, "Elements_UK")
                    .IsUnique();

                entity.Property(e => e.ElementId).HasColumnName("elementID");

                entity.Property(e => e.ElementName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("elementName");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("unit");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.ItemId).HasColumnName("itemID");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.ItemName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("itemName");

                entity.Property(e => e.ParentId).HasColumnName("parentID");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("Items_Parent_FK");
            });

            modelBuilder.Entity<ItemElement>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.ElementId })
                    .HasName("itemElements_PK");

                entity.Property(e => e.ItemId).HasColumnName("itemID");

                entity.Property(e => e.ElementId).HasColumnName("elementID");

                entity.HasOne(d => d.Element)
                    .WithMany(p => p.ItemElements)
                    .HasForeignKey(d => d.ElementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("itemElements_element_FK");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemElements)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("itemElements_item_FK");
            });

            modelBuilder.Entity<MonitorStationType>(entity =>
            {
                entity.HasKey(e => e.StationTypeId)
                    .HasName("monitorStationTypes_PK");

                entity.HasIndex(e => e.StationType, "monitorStationTypes_UK")
                    .IsUnique();

                entity.Property(e => e.StationTypeId).HasColumnName("stationTypeID");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.StationType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("stationType");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasIndex(e => e.RegionName, "regions_UK")
                    .IsUnique();

                entity.Property(e => e.RegionId).HasColumnName("regionID");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.RegionName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("regionName");
            });

            modelBuilder.Entity<TemperatureData>(entity =>
            {
                entity.HasKey(e => new { e.CountryId, e.Year })
                    .HasName("temperatureData_PK");

                entity.Property(e => e.CountryId).HasColumnName("countryID");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.Change)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("change");

                entity.Property(e => e.ObjectId).HasColumnName("objectID");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("unit");

                entity.Property(e => e.Value)
                    .HasColumnType("decimal(5, 3)")
                    .HasColumnName("value");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.TemperatureData)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("temperatureData_Countries_FK");
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
