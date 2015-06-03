namespace Homeschooledsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(),
                        Text = c.String(),
                        IsTrue = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        Importance = c.Int(nullable: false),
                        Text = c.String(),
                        Test_Id = c.Int(),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.Tests", t => t.Test_Id)
                .Index(t => t.Test_Id);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastLectureId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Lecture_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lectures", t => t.Lecture_Id)
                .Index(t => t.Lecture_Id);
            
            CreateTable(
                "dbo.Lectures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        Name = c.String(),
                        Text = c.String(),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        isFinished = c.Boolean(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        PublishDate = c.DateTime(nullable: false),
                        Estimation = c.Double(),
                        EstimationCount = c.Int(),
                        activated = c.Boolean(nullable: false),
                        sertificate = c.Boolean(nullable: false),
                        Author_Id = c.String(maxLength: 128),
                        Category_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .ForeignKey("dbo.Categories", t => t.Category_id)
                .Index(t => t.Author_Id)
                .Index(t => t.Category_id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        ConfirmationToken = c.String(),
                        IsConfirmed = c.Boolean(),
                        Skype = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        text = c.String(),
                        course_id = c.Int(),
                        previosComment_id = c.Int(),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Courses", t => t.course_id)
                .ForeignKey("dbo.Comments", t => t.previosComment_id)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.course_id)
                .Index(t => t.previosComment_id)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.CourseMarks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        mark = c.Int(nullable: false),
                        course_id = c.Int(),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Courses", t => t.course_id)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.course_id)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.Marks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        Student_Id = c.String(maxLength: 128),
                        Test_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.Student_Id)
                .ForeignKey("dbo.Tests", t => t.Test_Id)
                .Index(t => t.Student_Id)
                .Index(t => t.Test_Id);
            
            CreateTable(
                "dbo.ResetTokens",
                c => new
                    {
                        Token = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Token);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Int(nullable: false, identity: true),
                        Course_id = c.Int(),
                        Subscriber_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.Courses", t => t.Course_id)
                .ForeignKey("dbo.AspNetUsers", t => t.Subscriber_Id)
                .Index(t => t.Course_id)
                .Index(t => t.Subscriber_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "Subscriber_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subscriptions", "Course_id", "dbo.Courses");
            DropForeignKey("dbo.Marks", "Test_Id", "dbo.Tests");
            DropForeignKey("dbo.Marks", "Student_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseMarks", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseMarks", "course_id", "dbo.Courses");
            DropForeignKey("dbo.Comments", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "previosComment_id", "dbo.Comments");
            DropForeignKey("dbo.Comments", "course_id", "dbo.Courses");
            DropForeignKey("dbo.Questions", "Test_Id", "dbo.Tests");
            DropForeignKey("dbo.Tests", "Lecture_Id", "dbo.Lectures");
            DropForeignKey("dbo.Lectures", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Courses", "Category_id", "dbo.Categories");
            DropForeignKey("dbo.Courses", "Author_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropIndex("dbo.Subscriptions", new[] { "Subscriber_Id" });
            DropIndex("dbo.Subscriptions", new[] { "Course_id" });
            DropIndex("dbo.Marks", new[] { "Test_Id" });
            DropIndex("dbo.Marks", new[] { "Student_Id" });
            DropIndex("dbo.CourseMarks", new[] { "user_Id" });
            DropIndex("dbo.CourseMarks", new[] { "course_id" });
            DropIndex("dbo.Comments", new[] { "user_Id" });
            DropIndex("dbo.Comments", new[] { "previosComment_id" });
            DropIndex("dbo.Comments", new[] { "course_id" });
            DropIndex("dbo.Questions", new[] { "Test_Id" });
            DropIndex("dbo.Tests", new[] { "Lecture_Id" });
            DropIndex("dbo.Lectures", new[] { "CourseId" });
            DropIndex("dbo.Courses", new[] { "Category_id" });
            DropIndex("dbo.Courses", new[] { "Author_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropTable("dbo.Subscriptions");
            DropTable("dbo.ResetTokens");
            DropTable("dbo.Marks");
            DropTable("dbo.CourseMarks");
            DropTable("dbo.Comments");
            DropTable("dbo.Categories");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Courses");
            DropTable("dbo.Lectures");
            DropTable("dbo.Tests");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
