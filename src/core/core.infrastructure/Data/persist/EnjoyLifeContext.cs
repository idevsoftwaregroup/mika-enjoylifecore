using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using core.domain.DomainModelDTOs.WarehouseDTOs;
using core.domain.entity;
using core.domain.entity.Concierge;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.financialModels;
using core.domain.entity.log;
using core.domain.entity.partyModels;
using core.domain.entity.ReservationModels.JointModels;
using core.domain.entity.ReservationModels.ReservationModels;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using core.domain.entity.TicketModels;
using core.domain.entity.WebSocketModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace core.infrastructure.Data.persist
{
    public class EnjoyLifeContext : DbContext
    {
        public DbSet<ComplexModel> Complexes { get; set; }
        public DbSet<UnitModel> Units { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ResidentModel> Residents { get; set; }
        public DbSet<OwnerModel> Owners { get; set; }
        public DbSet<ManagerModel> Managers { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<ExpensesModel> Expenses { get; set; }
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<TransactionRequestModel> TransactionRequests { get; set; }
        public DbSet<TransactionResponseModel> TransactionResponses { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<EventSessionModel> EventSessions { get; set; }
        public DbSet<EventTicketModel> EventTickets { get; set; }
        public DbSet<EventContentModel> EventContents { get; set; }
        public DbSet<EventMediaModel> EventMedia { get; set; }
        public DbSet<EventPartyModel> EventParty { get; set; }
        public DbSet<StorageLot> StorageLots { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<MenuModel> Menus { get; set; }
        public DbSet<RoleMenuModel> RoleMenus { get; set; }
        public DbSet<UserConnectionModel> UserConnections { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }
        public DbSet<TicketMessageModel> TicketMessages { get; set; }
        public DbSet<TicketLogModel> TicketsLog { get; set; }
        public DbSet<TicketSeenModel> TicketsSeen { get; set; }
        public DbSet<TicketStatusModel> TicketsStatus { get; set; }
        public DbSet<UserLookUpModel> UserLookUp { get; set; }
        public DbSet<ActivityLookup> ActivityLookups { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ActionLookup> ActionLookups { get; set; }
        public DbSet<JointStatus> JointStatuses { get; set; }
        public DbSet<Joint> Joints { get; set; }
        public DbSet<JointMultiMedia> JointMultiMedias { get; set; }
        public DbSet<JointDailyActivityHour> JointDailyActivityHours { get; set; }
        public DbSet<JointSession> JointSessions { get; set; }
        public DbSet<JointSessionAcceptableUnit> JointSessionAcceptableUnits { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<JointSessionCancellationHour> JointSessionCancellationHours { get; set; }
        public DbSet<ConciergeModel> Concierge { get; set; }
        public DbSet<LobbyAttendantModel> LobbyAttendants { get; set; }
        public DbSet<ItemList> WarehouseItemLists { get; set; }
        public DbSet<EnjoylifeItems> Warehouse { get; set; }
        public DbSet<Units> UnitsWarehouse { get; set; }
        public DbSet<Groups> GroupsWarehouse { get; set; }
        public DbSet<MIKAMarketingBooking> MIKAMarketing_Bookings { get; set; }
        public DbSet<MIKAMarketingProjectsDTO> MIKAMarketing_Projects { get; set; }
        public DbSet<MIKAMarketingUserProfile> MIKAMarketing_UserProfiles { get; set; }
        public DbSet<ProjectFilesDTO> MIKAMarketing_ProjectFiles { get; set; }
        public DbSet<BillModel> Bills { get; set; }


        public EnjoyLifeContext(DbContextOptions<EnjoyLifeContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            EventSeed(modelBuilder);



            #region Marketing


            //modelBuilder.Entity<MIKAMarketingProjectsDTO>()
            //    .Property(p => p.ProjectUnitsDetails)
            //    .HasConversion(
            //        v => string.Join(",", v),  // Convert List<string> to a comma-separated string
            //        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());  // Convert string back to List<string>

            //modelBuilder.Entity<MIKAMarketingProjectsDTO>()
            //    .Property(p => p.ProjectUnitFiles)
            //    .HasConversion(
            //        v => string.Join(",", v),
            //        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            //modelBuilder.Entity<MIKAMarketingProjectsDTO>()
            //    .Property(p => p.ProjectBuildingTerraces)
            //    .HasConversion(
            //        v => string.Join(",", v),
            //        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());


            //modelBuilder.Entity<MIKAMarketingProjectsDTO>()
            //    .Property(p => p.ProjectBuildingFiles)
            //    .HasConversion(
            //        v => string.Join(",", v),
            //        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            #endregion





            #region Concierge
            modelBuilder.Entity<ConciergeModel>();
            #endregion
            #region Expense
            modelBuilder.Entity<ExpensesModel>()
                .HasOne(e => e.Unit)
                .WithMany()
                .HasForeignKey(e => e.UnitModelId)
                .IsRequired(false);
            #endregion
            #region Role
            modelBuilder.Entity<RoleModel>().
                HasMany(x => x.RoleMenus).WithOne(x => x.Role).HasForeignKey(e => e.RoleId);
            #endregion
            #region Menu
            modelBuilder.Entity<MenuModel>().
               HasMany(x => x.RoleMenus).WithOne(x => x.Menu).HasForeignKey(e => e.MenuId);
            modelBuilder.Entity<MenuModel>().
                  HasOne(x => x.SelfMenu).WithMany().HasForeignKey(x => x.ParentId);
            #endregion
            #region User
            modelBuilder.Entity<UserModel>().HasMany(x => x.UserConnections).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserModel>().HasMany(x => x.TicketMessages).WithOne(x => x.Author).HasForeignKey(x => x.AuthorId);
            modelBuilder.Entity<UserModel>().HasMany(x => x.Tickets).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserModel>().HasMany(x => x.TicketsTechnicians).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId);
            modelBuilder.Entity<UserModel>().HasMany(x => x.TicketsSeens).WithOne(x => x.Technician).HasForeignKey(x => x.SeenBy);
            modelBuilder.Entity<UserModel>().HasMany(x => x.LookUps).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserModel>().HasMany(x => x.Reservations).WithOne(x => x.ReservedBy).HasForeignKey(x => x.ReservedById);
            #endregion
            #region Unit
            modelBuilder.Entity<UnitModel>().HasMany(x => x.Tickets).WithOne(x => x.Unit).HasForeignKey(x => x.UnitId);
            modelBuilder.Entity<UnitModel>().HasMany(x => x.AcceptableUnits).WithOne(x => x.Unit).HasForeignKey(x => x.UnitId);
            modelBuilder.Entity<UnitModel>().HasMany(x => x.Reservations).WithOne(x => x.ReservedForUnit).HasForeignKey(x => x.ReservedForUnitId);
            modelBuilder.Entity<UnitModel>().HasMany(x => x.Bills).WithOne(x => x.Unit).HasForeignKey(x => x.UnitId);

            #endregion

            #region User Connection

            modelBuilder.Entity<UserConnectionModel>().Property(x => x.Connection).IsRequired(false);

            modelBuilder.Entity<UserConnectionModel>().Property(x => x.ConnectionDate).IsRequired(false);

            #endregion

            #region User LookUp
            modelBuilder.Entity<UserLookUpModel>().Property(x => x.Key).IsRequired(true);

            modelBuilder.Entity<UserLookUpModel>().Property(x => x.StringValue).IsRequired(false);

            modelBuilder.Entity<UserLookUpModel>().Property(x => x.IntegerValue).IsRequired(false);

            modelBuilder.Entity<UserLookUpModel>().Property(x => x.LongValue).IsRequired(false);

            modelBuilder.Entity<UserLookUpModel>().Property(x => x.DecimalValue).IsRequired(false);

            modelBuilder.Entity<UserLookUpModel>().Property(x => x.BooleanValue).IsRequired(false);

            #endregion

            #region Ticket

            modelBuilder.Entity<TicketModel>().HasMany(x => x.Seens).WithOne(x => x.Ticket).HasForeignKey(x => x.TicketId);

            modelBuilder.Entity<TicketModel>().HasMany(x => x.Messages).WithOne(x => x.Ticket).HasForeignKey(x => x.TicketId);

            modelBuilder.Entity<TicketModel>().HasMany(x => x.Logs).WithOne(x => x.Ticket).HasForeignKey(x => x.TicketId);

            modelBuilder.Entity<TicketModel>().Property(x => x.TrackingCode).IsRequired(false);

            modelBuilder.Entity<TicketModel>().Property(x => x.TicketNumber).IsRequired(false);

            modelBuilder.Entity<TicketModel>().Property(x => x.ModifyDate).IsRequired(false);

            modelBuilder.Entity<TicketModel>().Property(x => x.TechnicianId).IsRequired(false);

            #endregion

            #region Ticket Log

            modelBuilder.Entity<TicketLogModel>().Property(x => x.Message).IsRequired(false);

            #endregion

            #region Ticket Message

            modelBuilder.Entity<TicketMessageModel>().Property(x => x.Text).IsRequired(false);

            modelBuilder.Entity<TicketMessageModel>().Property(x => x.AttachmentId).IsRequired(false);

            #endregion

            #region Ticket Status

            modelBuilder.Entity<TicketStatusModel>().HasMany(x => x.Tickets).WithOne(x => x.Status).HasForeignKey(x => x.StatusId);

            modelBuilder.Entity<TicketStatusModel>().HasMany(x => x.Logs).WithOne(x => x.Status).HasForeignKey(x => x.StatusId);

            #endregion

            #region ActivityLookup

            modelBuilder.Entity<ActivityLookup>().Property(x => x.UserCoreId).IsRequired();
            modelBuilder.Entity<ActivityLookup>().Property(x => x.LoginDate).IsRequired();
            modelBuilder.Entity<ActivityLookup>().Property(x => x.LogoutDate).IsRequired(false);
            modelBuilder.Entity<ActivityLookup>().HasMany(x => x.ActionLookups).WithOne(x => x.ActivityLookup).HasForeignKey(x => x.ActivityLookupId);

            #endregion

            #region Module

            modelBuilder.Entity<Module>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Module>().Property(x => x.DisplayName).IsRequired();
            modelBuilder.Entity<Module>().HasMany(x => x.ActionLookups).WithOne(x => x.Module).HasForeignKey(x => x.ModuleId);

            #endregion

            #region ActionLookup

            modelBuilder.Entity<ActionLookup>().Property(x => x.ActionName).IsRequired();
            modelBuilder.Entity<ActionLookup>().Property(x => x.ActionDescription).IsRequired(false);
            modelBuilder.Entity<ActionLookup>().Property(x => x.Key).IsRequired();
            modelBuilder.Entity<ActionLookup>().Property(x => x.Value).IsRequired(false);
            modelBuilder.Entity<ActionLookup>().Property(x => x.LogDate).IsRequired();

            #endregion

            #region Reservation

            modelBuilder.Entity<ComplexModel>().HasMany(x => x.Joints).WithOne(x => x.Complex).HasForeignKey(x => x.ComplexId);

            modelBuilder.Entity<JointStatus>(jointStatus =>
            {
                jointStatus.Property(x => x.Title).IsRequired();
                jointStatus.Property(x => x.DisplayTitle).IsRequired();
                jointStatus.HasMany(x => x.Joints).WithOne(x => x.JointStatus).HasForeignKey(x => x.JointId);
            });

            modelBuilder.Entity<Joint>(joint =>
            {
                joint.Property(x => x.Title).IsRequired();
                joint.Property(x => x.Location).IsRequired(false);
                joint.Property(x => x.PhoneNumbers).IsRequired(false);
                joint.Property(x => x.Description).IsRequired(false);
                joint.Property(x => x.TermsText).IsRequired(false);
                joint.Property(x => x.TermsFileUrl).IsRequired(false);
                joint.Property(x => x.ThumbnailUrl).IsRequired(false);
                joint.Property(x => x.DailyUnitReservationCount).IsRequired(false);
                joint.Property(x => x.WeeklyUnitReservationCount).IsRequired(false);
                joint.Property(x => x.MonthlyUnitReservationCount).IsRequired(false);
                joint.Property(x => x.YearlyUnitReservationCount).IsRequired(false);
                joint.HasMany(x => x.JointMultiMedias).WithOne(x => x.Joint).HasForeignKey(x => x.JointId);
                joint.HasMany(x => x.JointDailyActivityHours).WithOne(x => x.Joint).HasForeignKey(x => x.JointId);
                joint.HasMany(x => x.JointSessions).WithOne(x => x.Joint).HasForeignKey(x => x.JointId);
            });

            modelBuilder.Entity<JointMultiMedia>(jointMultiMedia =>
            {
                jointMultiMedia.Property(x => x.Url).IsRequired();
                jointMultiMedia.Property(x => x.MediaType).IsRequired();
                jointMultiMedia.Property(x => x.Alt).IsRequired(false);
            });

            modelBuilder.Entity<JointDailyActivityHour>(jointDailyActivityHour =>
            {
                jointDailyActivityHour.Property(x => x.PartialStartTime).IsRequired();
                jointDailyActivityHour.Property(x => x.PartialEndTime).IsRequired();
            });

            modelBuilder.Entity<JointSession>(session =>
            {
                session.Property(x => x.Title).IsRequired(false);
                session.Property(x => x.Description).IsRequired(false);
                session.Property(x => x.SessionDate).IsRequired();
                session.Property(x => x.StartTime).IsRequired(false);
                session.Property(x => x.EndTime).IsRequired(false);
                session.Property(x => x.StartReservationDate).IsRequired(false);
                session.Property(x => x.EndReservationDate).IsRequired(false);
                session.Property(x => x.MinimumReservationMinutes).IsRequired(false);
                session.Property(x => x.MaximumReservationMinutes).IsRequired(false);
                session.Property(x => x.SessionCost).IsRequired(false);
                session.Property(x => x.PublicSessionGender).IsRequired(false);
                session.Property(x => x.CreationDate).IsRequired();
                session.Property(x => x.Creator).IsRequired();
                session.Property(x => x.LastModificationDate).IsRequired(false);
                session.Property(x => x.LastModifier).IsRequired(false);
                session.Property(x => x.UnitExtraReservationCost).IsRequired(false);
                session.Property(x => x.Capacity).IsRequired(false);
                session.Property(x => x.GuestCapacity).IsRequired(false);
                session.HasMany(x => x.AcceptableUnits).WithOne(x => x.JointSession).HasForeignKey(x => x.JointSessionId);
                session.HasMany(x => x.Reservations).WithOne(x => x.JointSession).HasForeignKey(x => x.JointSessionId);
                session.HasMany(x => x.JointSessionCancellationHours).WithOne(x => x.JointSession).HasForeignKey(x => x.JointSessionId);
            });

            modelBuilder.Entity<Reservation>(reservation =>
            {
                reservation.Property(x => x.StartDate).IsRequired();
                reservation.Property(x => x.EndDate).IsRequired();
                reservation.Property(x => x.CancellationDescription).IsRequired(false);
                reservation.Property(x => x.LastModifier).IsRequired(false);
                reservation.Property(x => x.LastModificationDate).IsRequired(false);
            });

            modelBuilder.Entity<JointSessionCancellationHour>(hour => hour.Property(x => x.Hour).IsRequired());
            #endregion

            #region LobbyAttendant

            modelBuilder.Entity<LobbyAttendantModel>(lobbyAttendant =>
            {
                lobbyAttendant.Property(x => x.UserName).IsRequired();
                lobbyAttendant.Property(x => x.Password).IsRequired();
                lobbyAttendant.Property(x => x.Active).IsRequired();
                lobbyAttendant.Property(x => x.FirstName).IsRequired();
                lobbyAttendant.Property(x => x.LastName).IsRequired();
                lobbyAttendant.Property(x => x.PhoneNumber).IsRequired(false);
                lobbyAttendant.Property(x => x.Creator).IsRequired();
                lobbyAttendant.Property(x => x.CreationDate).IsRequired();
                lobbyAttendant.Property(x => x.LastModifier).IsRequired(false);
                lobbyAttendant.Property(x => x.LastModificationDate).IsRequired(false);
            });

            #endregion

            #region WareHouse
            //modelBuilder.Entity<ItemList>(ItemList =>
            //{
            //    ItemList.Property(m => m.ItemListCode).IsRequired();
            //    ItemList.Property(m => m.Group).IsRequired();
            //    ItemList.Property(m => m.Unit).IsRequired();
            //    ItemList.Property(m => m.Name).IsRequired();
            //});
            //modelBuilder.Entity<ItemList>()
            //    .HasKey(m => m.ItemListCode);

            modelBuilder.Entity<EnjoylifeItems>(EnjoyItemList =>
            {
                EnjoyItemList.Property(n => n.Id).IsRequired(true); // Id is about to identity
            });

            modelBuilder.Entity<EnjoylifeItems>()
                            .HasKey(e => e.Id); // Set the primary key
            #endregion

            #region Bill

            modelBuilder.Entity<BillModel>().Property(x => x.Description).IsRequired();

            #endregion

            modelBuilder.ApplyConfiguration(new ComplexConfiguration());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(modelBuilder);
        }

        private void EventSeed(ModelBuilder modelBuilder)
        {
            //List<UserModel> users = new() { new UserModel() { Id = 1, FirstName = "u1" },
            //                                new UserModel() { Id = 2, FirstName = "u2" },
            //                                new UserModel() { Id = 3, FirstName = "u3" },
            //                                new UserModel() { Id = 4, FirstName = "u4" },
            //                                new UserModel() { Id = 5, FirstName = "u5" }};

            //modelBuilder.Entity<UserModel>().HasData(users);

            //modelBuilder.Entity<ComplexModel>().HasData(new ComplexModel() { Id = 1, Title = "c1" });

            //List<UnitModel> units = new() { new UnitModel() { Id = 1, Name = "u1" } , new UnitModel() { Id = 2, Name = "u2" } };

            //modelBuilder.Entity<UnitModel>().HasData(units);


            //modelBuilder.Entity<ResidentModel>().HasData(new ResidentModel() { Id = 1, Unit = units[0] , User = users[0], IsHead = true },
            //                                             new ResidentModel() {Id = 2 , Unit = units[0] , User = users[1], IsHead = false },
            //                                             new ResidentModel() { Id = 2, Unit = units[0], User = users[2], IsHead = false });
            //modelBuilder.Entity<OwnerModel>().HasData( new OwnerModel() { Id = 1, Unit = units[0], User = users[3] } ,
            //                                           new OwnerModel() { Id = 2 , Unit = units[1] , User = users[4] } );

        }



    }


}
