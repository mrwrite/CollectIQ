using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CollectIQ.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddItemTypeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8b"), "Sneaker" },
                    { new Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8c"), "Cologne" },
                    { new Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8d"), "Watch" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: new Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8b"));

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: new Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8c"));

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: new Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8d"));
        }
    }
}
