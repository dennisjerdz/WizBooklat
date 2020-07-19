namespace WizBooklat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AuthorKey = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.BookAuthors",
                c => new
                    {
                        BookAuthorId = c.Int(nullable: false, identity: true),
                        BookTemplateId = c.Int(nullable: false),
                        AuthorId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookAuthorId)
                .ForeignKey("dbo.Authors", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.BookTemplates", t => t.BookTemplateId, cascadeDelete: true)
                .Index(t => t.BookTemplateId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.BookTemplates",
                c => new
                    {
                        BookTemplateId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        ImageLocation = c.String(),
                        Type = c.Short(nullable: false),
                        LoanPeriod = c.Int(nullable: false),
                        ISBN = c.String(),
                        OLKey = c.String(),
                        PublishDate = c.DateTime(),
                        InitialQuantity = c.Int(nullable: false),
                        ActualQuantity = c.Int(nullable: false),
                        Genres = c.String(),
                        Authors = c.String(),
                        IsFeatured = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookTemplateId);
            
            CreateTable(
                "dbo.BookGenres",
                c => new
                    {
                        BookGenreId = c.Int(nullable: false, identity: true),
                        BookTemplateId = c.Int(nullable: false),
                        GenreId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookGenreId)
                .ForeignKey("dbo.BookTemplates", t => t.BookTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.Genres", t => t.GenreId, cascadeDelete: true)
                .Index(t => t.BookTemplateId)
                .Index(t => t.GenreId);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GenreId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        Rate = c.Short(nullable: false),
                        Comment = c.String(),
                        UserId = c.String(maxLength: 128),
                        FirstName = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        BookTemplate_BookTemplateId = c.Int(),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.BookTemplates", t => t.BookTemplate_BookTemplateId)
                .Index(t => t.UserId)
                .Index(t => t.BookTemplate_BookTemplateId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        StudentNumber = c.String(),
                        BranchId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        LoanId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        BookId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        IsLate = c.Boolean(nullable: false),
                        IsDamaged = c.Boolean(nullable: false),
                        LostReported = c.DateTime(),
                        LostConfirmed = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LoanId)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        InternalCode = c.String(),
                        BranchId = c.Int(nullable: false),
                        BookTemplateId = c.Int(nullable: false),
                        BookStatus = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.BookId)
                .ForeignKey("dbo.BookTemplates", t => t.BookTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.BookTemplateId);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        BranchId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BranchId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PointHistories",
                c => new
                    {
                        PointHistoryId = c.Int(nullable: false, identity: true),
                        Points = c.Int(nullable: false),
                        Type = c.Short(nullable: false),
                        Description = c.String(),
                        UserId = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PointHistoryId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        ClaimId = c.Int(nullable: false, identity: true),
                        RewardId = c.Int(nullable: false),
                        Code = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ClaimId)
                .ForeignKey("dbo.Rewards", t => t.RewardId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.RewardId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Rewards",
                c => new
                    {
                        RewardId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ImageLocation = c.String(),
                        Description = c.String(),
                        RankId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RewardId)
                .ForeignKey("dbo.Ranks", t => t.RankId, cascadeDelete: true)
                .Index(t => t.RankId);
            
            CreateTable(
                "dbo.Ranks",
                c => new
                    {
                        RankId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Points = c.Int(nullable: false),
                        Color = c.String(),
                    })
                .PrimaryKey(t => t.RankId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        VisitId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VisitId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.BranchId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        SettingId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Value = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SettingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Reviews", "BookTemplate_BookTemplateId", "dbo.BookTemplates");
            DropForeignKey("dbo.Visits", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Visits", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Claims", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Claims", "RewardId", "dbo.Rewards");
            DropForeignKey("dbo.Rewards", "RankId", "dbo.Ranks");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PointHistories", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Loans", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Loans", "BookId", "dbo.Books");
            DropForeignKey("dbo.Books", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Books", "BookTemplateId", "dbo.BookTemplates");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookGenres", "GenreId", "dbo.Genres");
            DropForeignKey("dbo.BookGenres", "BookTemplateId", "dbo.BookTemplates");
            DropForeignKey("dbo.BookAuthors", "BookTemplateId", "dbo.BookTemplates");
            DropForeignKey("dbo.BookAuthors", "AuthorId", "dbo.Authors");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Visits", new[] { "UserId" });
            DropIndex("dbo.Visits", new[] { "BranchId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Rewards", new[] { "RankId" });
            DropIndex("dbo.Claims", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Claims", new[] { "RewardId" });
            DropIndex("dbo.PointHistories", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Books", new[] { "BookTemplateId" });
            DropIndex("dbo.Books", new[] { "BranchId" });
            DropIndex("dbo.Loans", new[] { "BookId" });
            DropIndex("dbo.Loans", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Reviews", new[] { "BookTemplate_BookTemplateId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.BookGenres", new[] { "GenreId" });
            DropIndex("dbo.BookGenres", new[] { "BookTemplateId" });
            DropIndex("dbo.BookAuthors", new[] { "AuthorId" });
            DropIndex("dbo.BookAuthors", new[] { "BookTemplateId" });
            DropTable("dbo.Settings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Visits");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Ranks");
            DropTable("dbo.Rewards");
            DropTable("dbo.Claims");
            DropTable("dbo.PointHistories");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Branches");
            DropTable("dbo.Books");
            DropTable("dbo.Loans");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Reviews");
            DropTable("dbo.Genres");
            DropTable("dbo.BookGenres");
            DropTable("dbo.BookTemplates");
            DropTable("dbo.BookAuthors");
            DropTable("dbo.Authors");
        }
    }
}
