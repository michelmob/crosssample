using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Gravity.Manager.Data.EF.Migrations
{
    public partial class AddReportLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportLine",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AwsInstanceId = table.Column<long>(nullable: false),
                    IsTable = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<uint>(nullable: false),
                    ParentId = table.Column<long>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportLine_AwsInstance_AwsInstanceId",
                        column: x => x.AwsInstanceId,
                        principalTable: "AwsInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportLine_ReportLine_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ReportLine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportLine_AwsInstanceId",
                table: "ReportLine",
                column: "AwsInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportLine_ParentId",
                table: "ReportLine",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportLine");
        }
    }
}
