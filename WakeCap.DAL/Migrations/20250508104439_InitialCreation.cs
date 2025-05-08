using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WakeCap.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "upload_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    valid_rows = table.Column<int>(type: "integer", nullable: false),
                    error_rows = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    ErrorDetailsJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_upload_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "worker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "zone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "worker_zone_assignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worker_id = table.Column<int>(type: "integer", nullable: false),
                    zone_id = table.Column<int>(type: "integer", nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_zone_assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_worker_zone_assignment_worker_worker_id",
                        column: x => x.worker_id,
                        principalTable: "worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_worker_zone_assignment_zone_zone_id",
                        column: x => x.zone_id,
                        principalTable: "zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_upload_log_UploadedAt",
                table: "upload_log",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_worker_Code",
                table: "worker",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_worker_zone_assignment_worker_id_zone_id_effective_date",
                table: "worker_zone_assignment",
                columns: new[] { "worker_id", "zone_id", "effective_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_worker_zone_assignment_zone_id",
                table: "worker_zone_assignment",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_zone_Code",
                table: "zone",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "upload_log");

            migrationBuilder.DropTable(
                name: "worker_zone_assignment");

            migrationBuilder.DropTable(
                name: "worker");

            migrationBuilder.DropTable(
                name: "zone");
        }
    }
}
