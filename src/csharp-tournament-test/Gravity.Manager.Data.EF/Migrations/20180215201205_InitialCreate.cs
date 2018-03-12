using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Gravity.Manager.Data.EF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AwsAccount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwsAccount", x => x.Id);
                    table.UniqueConstraint("AK_AwsAccount_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ParentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_Organization_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscoverySession",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AwsAccountId = table.Column<long>(nullable: false),
                    RunDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoverySession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscoverySession_AwsAccount_AwsAccountId",
                        column: x => x.AwsAccountId,
                        principalTable: "AwsAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EMail = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OrganizationId = table.Column<long>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AwsInstance",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscoverySessionId = table.Column<long>(nullable: false),
                    IpAddressBytes = table.Column<byte[]>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwsInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AwsInstance_DiscoverySession_DiscoverySessionId",
                        column: x => x.DiscoverySessionId,
                        principalTable: "DiscoverySession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditEntry",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    EntityName = table.Column<string>(maxLength: 100, nullable: false),
                    NewValue = table.Column<string>(type: "VARCHAR(65535)", nullable: true),
                    OldValue = table.Column<string>(type: "VARCHAR(65535)", nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditEntry_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dependency",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceAwsInstanceId = table.Column<long>(nullable: false),
                    TargetAwsInstanceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dependency_AwsInstance_SourceAwsInstanceId",
                        column: x => x.SourceAwsInstanceId,
                        principalTable: "AwsInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dependency_AwsInstance_TargetAwsInstanceId",
                        column: x => x.TargetAwsInstanceId,
                        principalTable: "AwsInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DependencyFinding",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DependencyId = table.Column<long>(nullable: false),
                    FileName = table.Column<string>(maxLength: 4096, nullable: false),
                    Text = table.Column<string>(maxLength: 8192, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependencyFinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DependencyFinding_Dependency_DependencyId",
                        column: x => x.DependencyId,
                        principalTable: "Dependency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntry_UserId",
                table: "AuditEntry",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AwsInstance_DiscoverySessionId",
                table: "AwsInstance",
                column: "DiscoverySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependency_SourceAwsInstanceId",
                table: "Dependency",
                column: "SourceAwsInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependency_TargetAwsInstanceId",
                table: "Dependency",
                column: "TargetAwsInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DependencyFinding_DependencyId",
                table: "DependencyFinding",
                column: "DependencyId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscoverySession_AwsAccountId",
                table: "DiscoverySession",
                column: "AwsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ParentId",
                table: "Organization",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_User_OrganizationId",
                table: "User",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEntry");

            migrationBuilder.DropTable(
                name: "DependencyFinding");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Dependency");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "AwsInstance");

            migrationBuilder.DropTable(
                name: "DiscoverySession");

            migrationBuilder.DropTable(
                name: "AwsAccount");
        }
    }
}
