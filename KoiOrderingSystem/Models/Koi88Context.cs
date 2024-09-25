using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KoiOrderingSystem.Models;

public partial class Koi88Context : DbContext
{
    public Koi88Context()
    {
    }

    public Koi88Context(DbContextOptions<Koi88Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingPayment> BookingPayments { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FormBooking> FormBookings { get; set; }

    public virtual DbSet<KoiFarm> KoiFarms { get; set; }

    public virtual DbSet<KoiFish> KoiFishes { get; set; }

    public virtual DbSet<Koipackage> Koipackages { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Po> Pos { get; set; }

    public virtual DbSet<Podetail> Podetails { get; set; }

    public virtual DbSet<Popayment> Popayments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SpecialVariety> SpecialVarieties { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<TripDetail> TripDetails { get; set; }

    public virtual DbSet<Variety> Varieties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=Koi88;Persist Security Info=True;User ID=sa;Password=12345;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CDBF91E552");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(100)
                .HasColumnName("imageUrl");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(100)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__5DE3A5B1EA871982");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingPaymentId).HasColumnName("booking_payment_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.FormOrderId).HasColumnName("form_order_id");
            entity.Property(e => e.KoiDeliveryDate).HasColumnName("koi_delivery_date");
            entity.Property(e => e.KoiDeliveryTime).HasColumnName("koi_delivery_time");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(200)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.TripId).HasColumnName("trip_id");

            entity.HasOne(d => d.BookingPayment).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BookingPaymentId)
                .HasConstraintName("FK_Booking_Payment");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Booking_Customer");

            entity.HasOne(d => d.Feedback).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK_Booking_Feedback");

            entity.HasOne(d => d.FormOrder).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FormOrderId)
                .HasConstraintName("FK_Booking_FormBooking");

            entity.HasOne(d => d.Po).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PoId)
                .HasConstraintName("FK_Booking_PO");

            entity.HasOne(d => d.Role).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Booking_Role");

            entity.HasOne(d => d.Trip).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_Booking_Trip");
        });

        modelBuilder.Entity<BookingPayment>(entity =>
        {
            entity.HasKey(e => e.BookingPaymentId).HasName("PK__BookingP__DB3FBE579DC6A68C");

            entity.ToTable("BookingPayment");

            entity.Property(e => e.BookingPaymentId).HasColumnName("booking_payment_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingPayments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingPayment_Booking");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.BookingPayments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK_BookingPayment_PaymentMethod");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85A85A9FC0");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");

            entity.HasOne(d => d.Account).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Customer_Account");

            entity.HasOne(d => d.Booking).WithMany(p => p.Customers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Customer_Booking");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8C00CE3BF4");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Comments)
                .HasMaxLength(1000)
                .HasColumnName("comments");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Booking).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Feedback_Booking");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Feedback_Customer");
        });

        modelBuilder.Entity<FormBooking>(entity =>
        {
            entity.HasKey(e => e.FormBookingId).HasName("PK__FormBook__180C306CD3977F95");

            entity.ToTable("FormBooking");

            entity.Property(e => e.FormBookingId).HasColumnName("form_booking_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EstimatedCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("estimatedCost");
            entity.Property(e => e.EstimatedDepartureDate).HasColumnName("estimatedDepartureDate");
            entity.Property(e => e.FavoriteKoi)
                .HasMaxLength(200)
                .HasColumnName("favoriteKoi");
            entity.Property(e => e.Favoritefarm)
                .HasMaxLength(100)
                .HasColumnName("favoritefarm");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.HotelAccommodation)
                .HasMaxLength(100)
                .HasColumnName("hotel_accommodation");
            entity.Property(e => e.Note)
                .HasMaxLength(1000)
                .HasColumnName("note");
            entity.Property(e => e.Phone)
                .HasMaxLength(100)
                .HasColumnName("phone");
            entity.Property(e => e.ReturnDate).HasColumnName("returnDate");

            entity.HasOne(d => d.Booking).WithMany(p => p.FormBookings)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_FormBooking_Booking");

            entity.HasOne(d => d.Customer).WithMany(p => p.FormBookings)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_FormBooking_Customer");
        });

        modelBuilder.Entity<KoiFarm>(entity =>
        {
            entity.HasKey(e => e.FarmId).HasName("PK__KoiFarm__23F321B4F7BE2F2D");

            entity.ToTable("KoiFarm");

            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(200)
                .HasColumnName("contact_info");
            entity.Property(e => e.FarmName)
                .HasMaxLength(100)
                .HasColumnName("farm_name");
            entity.Property(e => e.KoiId).HasColumnName("koi_id");
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .HasColumnName("location");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.SpecialVarietyId).HasColumnName("special_variety_id");
            entity.Property(e => e.TripDetailId).HasColumnName("trip_detail_id");

            entity.HasOne(d => d.Koi).WithMany(p => p.KoiFarms)
                .HasForeignKey(d => d.KoiId)
                .HasConstraintName("FK_KoiFarm_KoiFish");

            entity.HasOne(d => d.Package).WithMany(p => p.KoiFarms)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_KoiFarm_KOIPackage");

            entity.HasOne(d => d.Po).WithMany(p => p.KoiFarms)
                .HasForeignKey(d => d.PoId)
                .HasConstraintName("FK_KoiFarm_PO");

            entity.HasOne(d => d.SpecialVariety).WithMany(p => p.KoiFarms)
                .HasForeignKey(d => d.SpecialVarietyId)
                .HasConstraintName("FK_KoiFarm_SpecialVariety");

            entity.HasOne(d => d.TripDetail).WithMany(p => p.KoiFarms)
                .HasForeignKey(d => d.TripDetailId)
                .HasConstraintName("FK_KoiFarm_TripDetail");
        });

        modelBuilder.Entity<KoiFish>(entity =>
        {
            entity.HasKey(e => e.KoiId).HasName("PK__KoiFish__8D4905E7DBDA437D");

            entity.ToTable("KoiFish");

            entity.Property(e => e.KoiId).HasColumnName("koi_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.PoDetailId).HasColumnName("po_detail_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .HasColumnName("size");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.VarietyId).HasColumnName("variety_id");

            entity.HasOne(d => d.Farm).WithMany(p => p.KoiFishes)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("FK_KoiFish_Farm");

            entity.HasOne(d => d.Package).WithMany(p => p.KoiFishes)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_KoiFish_Package");

            entity.HasOne(d => d.PoDetail).WithMany(p => p.KoiFishes)
                .HasForeignKey(d => d.PoDetailId)
                .HasConstraintName("FK_KoiFish_PODetail");

            entity.HasOne(d => d.Variety).WithMany(p => p.KoiFishes)
                .HasForeignKey(d => d.VarietyId)
                .HasConstraintName("FK_KoiFish_Variety");
        });

        modelBuilder.Entity<Koipackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__KOIPacka__63846AE8EBB08B9B");

            entity.ToTable("KOIPackage");

            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.KoiId).HasColumnName("koi_id");
            entity.Property(e => e.PackageName)
                .HasMaxLength(100)
                .HasColumnName("package_name");
            entity.Property(e => e.PoDetailId).HasColumnName("po_detail_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.VarietyId).HasColumnName("variety_id");

            entity.HasOne(d => d.Farm).WithMany(p => p.Koipackages)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("FK_KOIPackage_Farm");

            entity.HasOne(d => d.Koi).WithMany(p => p.Koipackages)
                .HasForeignKey(d => d.KoiId)
                .HasConstraintName("FK_KOIPackage_KoiFish");

            entity.HasOne(d => d.PoDetail).WithMany(p => p.Koipackages)
                .HasForeignKey(d => d.PoDetailId)
                .HasConstraintName("FK_KOIPackage_PODetail");

            entity.HasOne(d => d.Variety).WithMany(p => p.Koipackages)
                .HasForeignKey(d => d.VarietyId)
                .HasConstraintName("FK_KOIPackage_Variety");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__8A3EA9EBC21019F6");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.BookingPaymentId).HasColumnName("booking_payment_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.MethodName)
                .HasMaxLength(50)
                .HasColumnName("method_name");
            entity.Property(e => e.PoPaymentId).HasColumnName("po_payment_id");

            entity.HasOne(d => d.BookingPayment).WithMany(p => p.PaymentMethods)
                .HasForeignKey(d => d.BookingPaymentId)
                .HasConstraintName("FK_PaymentMethod_BookingPayment");

            entity.HasOne(d => d.PoPayment).WithMany(p => p.PaymentMethods)
                .HasForeignKey(d => d.PoPaymentId)
                .HasConstraintName("FK_PaymentMethod_POPayment");
        });

        modelBuilder.Entity<Po>(entity =>
        {
            entity.HasKey(e => e.PoId).HasName("PK__PO__368DA7F082872A70");

            entity.ToTable("PO");

            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.PoDetailId).HasColumnName("po_detail_id");
            entity.Property(e => e.PoPaymentId).HasColumnName("po_payment_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Booking).WithMany(p => p.Pos)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_PO_Booking");

            entity.HasOne(d => d.PoDetail).WithMany(p => p.Pos)
                .HasForeignKey(d => d.PoDetailId)
                .HasConstraintName("FK_PO_PODetail");

            entity.HasOne(d => d.PoPayment).WithMany(p => p.Pos)
                .HasForeignKey(d => d.PoPaymentId)
                .HasConstraintName("FK_PO_POPayment");
        });

        modelBuilder.Entity<Podetail>(entity =>
        {
            entity.HasKey(e => e.PoDetailId).HasName("PK__PODetail__9E6103B205012F07");

            entity.ToTable("PODetail");

            entity.Property(e => e.PoDetailId).HasColumnName("po_detail_id");
            entity.Property(e => e.Deposit)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("deposit");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(1000)
                .HasColumnName("imageUrl");
            entity.Property(e => e.KoiId).HasColumnName("koi_id");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalKoiPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_koi_price");

            entity.HasOne(d => d.Farm).WithMany(p => p.Podetails)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("FK_PODetail_Farm");

            entity.HasOne(d => d.Koi).WithMany(p => p.Podetails)
                .HasForeignKey(d => d.KoiId)
                .HasConstraintName("FK_PODetail_KoiFish");

            entity.HasOne(d => d.Package).WithMany(p => p.Podetails)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_PODetail_Package");

            entity.HasOne(d => d.Po).WithMany(p => p.Podetails)
                .HasForeignKey(d => d.PoId)
                .HasConstraintName("FK_PODetail_PO");
        });

        modelBuilder.Entity<Popayment>(entity =>
        {
            entity.HasKey(e => e.PoPaymentId).HasName("PK__POPaymen__D958441B0758AB18");

            entity.ToTable("POPayment");

            entity.Property(e => e.PoPaymentId).HasColumnName("po_payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.PoId).HasColumnName("po_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Popayments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK_POPayment_PaymentMethod");

            entity.HasOne(d => d.Po).WithMany(p => p.Popayments)
                .HasForeignKey(d => d.PoId)
                .HasConstraintName("FK_POPayment_PO");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CC9469A8F5");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Account).WithMany(p => p.Roles)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Role_Account");

            entity.HasOne(d => d.Booking).WithMany(p => p.Roles)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Role_Booking");
        });

        modelBuilder.Entity<SpecialVariety>(entity =>
        {
            entity.HasKey(e => e.SpecialVarietyId).HasName("PK__SpecialV__5969FE8D490EAF26");

            entity.ToTable("SpecialVariety");

            entity.Property(e => e.SpecialVarietyId).HasColumnName("special_variety_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.VarietyId).HasColumnName("variety_id");

            entity.HasOne(d => d.Farm).WithMany(p => p.SpecialVarieties)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("FK_SpecialVariety_Farm");

            entity.HasOne(d => d.Variety).WithMany(p => p.SpecialVarieties)
                .HasForeignKey(d => d.VarietyId)
                .HasConstraintName("FK_SpecialVariety_Variety");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK__Trip__302A5D9E70D0162E");

            entity.ToTable("Trip");

            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.TripDetailId).HasColumnName("trip_detail_id");
            entity.Property(e => e.TripName)
                .HasMaxLength(100)
                .HasColumnName("trip_name");

            entity.HasOne(d => d.Booking).WithMany(p => p.Trips)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Trip_Booking");

            entity.HasOne(d => d.TripDetail).WithMany(p => p.Trips)
                .HasForeignKey(d => d.TripDetailId)
                .HasConstraintName("FK_Trip_TripDetail");
        });

        modelBuilder.Entity<TripDetail>(entity =>
        {
            entity.HasKey(e => e.TripDetailId).HasName("PK__TripDeta__FA0AB2428192BF0A");

            entity.ToTable("TripDetail");

            entity.Property(e => e.TripDetailId).HasColumnName("trip_detail_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.MainTopic)
                .HasMaxLength(200)
                .HasColumnName("main_topic");
            entity.Property(e => e.NotePrice)
                .HasMaxLength(1000)
                .HasColumnName("note_price");
            entity.Property(e => e.Status)
                .HasMaxLength(200)
                .HasColumnName("status");
            entity.Property(e => e.SubTopic)
                .HasMaxLength(1000)
                .HasColumnName("sub_topic");
            entity.Property(e => e.TripId).HasColumnName("trip_id");

            entity.HasOne(d => d.Farm).WithMany(p => p.TripDetails)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("FK_TripDetail_KoiFarm");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripDetails)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_TripDetail_Trip");
        });

        modelBuilder.Entity<Variety>(entity =>
        {
            entity.HasKey(e => e.VarietyId).HasName("PK__Variety__20A0CFC5C48B2BD4");

            entity.ToTable("Variety");

            entity.Property(e => e.VarietyId).HasColumnName("variety_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.KoiId).HasColumnName("koi_id");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.SpecialVarietyId).HasColumnName("special_variety_id");
            entity.Property(e => e.VarietyName)
                .HasMaxLength(100)
                .HasColumnName("variety_name");

            entity.HasOne(d => d.Koi).WithMany(p => p.Varieties)
                .HasForeignKey(d => d.KoiId)
                .HasConstraintName("FK_Variety_KoiFish");

            entity.HasOne(d => d.Package).WithMany(p => p.Varieties)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_Variety_KOIPackage");

            entity.HasOne(d => d.SpecialVariety).WithMany(p => p.Varieties)
                .HasForeignKey(d => d.SpecialVarietyId)
                .HasConstraintName("FK_Variety_SpecialVariety");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
