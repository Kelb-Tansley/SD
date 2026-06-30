using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeamKValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FemFileStableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeamNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    K2 = table.Column<double>(type: "REAL", nullable: false),
                    K1 = table.Column<double>(type: "REAL", nullable: false),
                    Kz = table.Column<double>(type: "REAL", nullable: false),
                    KeTop = table.Column<double>(type: "REAL", nullable: false),
                    KeBottom = table.Column<double>(type: "REAL", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeamKValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FemFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileNameOnly = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FemFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelDesignSettings",
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
                    table.PrimaryKey("PK_ModelDesignSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SectionDesignSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FemFileStableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    WebStiffenerSpacing = table.Column<double>(type: "REAL", nullable: false),
                    NetAreaFactor = table.Column<double>(type: "REAL", nullable: false),
                    IsLateralRestraint = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTorsionalRestraint = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTopFlangeRestraint = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBottomFlangeRestraint = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPlateGirder = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBracedFrame = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionDesignSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeamKValues");

            migrationBuilder.DropTable(
                name: "FemFiles");

            migrationBuilder.DropTable(
                name: "ModelDesignSettings");

            migrationBuilder.DropTable(
                name: "SectionDesignSettings");
        }
    }
}
