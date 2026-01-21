using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD.Data.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DesignSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeamAllignmentAngleTolerance = table.Column<double>(type: "REAL", nullable: false),
                    BeamRotationAngleTolerance = table.Column<double>(type: "REAL", nullable: false),
                    BeamRestraintAngleTolerance = table.Column<double>(type: "REAL", nullable: false),
                    BeamMinStations = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FemFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FemFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeamPropertySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FemFileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLateralRestraint = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeamPropertySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeamPropertySettings_FemFiles_FemFileId",
                        column: x => x.FemFileId,
                        principalTable: "FemFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeamPropertySettings_FemFileId",
                table: "BeamPropertySettings",
                column: "FemFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeamPropertySettings");

            migrationBuilder.DropTable(
                name: "DesignSettings");

            migrationBuilder.DropTable(
                name: "FemFiles");
        }
    }
}
