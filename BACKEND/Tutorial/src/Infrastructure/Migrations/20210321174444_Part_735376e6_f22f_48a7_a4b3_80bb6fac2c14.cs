using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tutorial.Infrastructure.Migrations
{
    public partial class Part_735376e6_f22f_48a7_a4b3_80bb6fac2c14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainRecordId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parts");
        }
    }
}
