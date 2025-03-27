using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "created_by",
                schema: "public",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on_utc",
                schema: "public",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_on_utc",
                schema: "public",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_on_utc",
                schema: "public",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_on_utc",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "deleted_on_utc",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_modified_on_utc",
                schema: "public",
                table: "users");
        }
    }
}
