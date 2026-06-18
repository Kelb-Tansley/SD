using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD.Data.Migrations
{
    public partial class AddStableIdToFemFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StableId",
                table: "FemFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newsequentialid()");

            // Add FemFileStableId to BeamKValues (nullable to allow backfill)
            migrationBuilder.AddColumn<Guid>(
                name: "FemFileStableId",
                table: "BeamKValues",
                type: "uniqueidentifier",
                nullable: true);

            // NOTE: Backfill/mapping logic should be implemented in a custom script/tool
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FemFileStableId",
                table: "BeamKValues");

            migrationBuilder.DropColumn(
                name: "StableId",
                table: "FemFiles");
        }
    }
}
