using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordStore.Lib.Migrations
{
    public partial class ModelExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Credentials",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            migrationBuilder.Sql("update Credentials set CreateDate=datetime('now')");

            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneNumberAuth",
                table: "Credentials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Credentials",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Credentials",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SecretQuestion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CredentialId = table.Column<long>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: false),
                    Answer = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecretQuestion_Credentials_CredentialId",
                        column: x => x.CredentialId,
                        principalTable: "Credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecretQuestion_CredentialId",
                table: "SecretQuestion",
                column: "CredentialId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecretQuestion");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Credentials");

            migrationBuilder.DropColumn(
                name: "IsPhoneNumberAuth",
                table: "Credentials");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Credentials");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Credentials");
        }
    }
}
