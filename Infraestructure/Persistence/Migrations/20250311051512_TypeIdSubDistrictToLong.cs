using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TypeIdSubDistrictToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SubDistricts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "RegionId",
                table: "States",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "States",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Regions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "MicroRegionId",
                table: "Municipalities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Municipalities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "MesoregionId",
                table: "MicroRegions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MicroRegions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "StateId",
                table: "Mesoregions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Mesoregions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "MunicipalityId",
                table: "Districts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Districts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_States_RegionId",
                table: "States",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_MicroRegionId",
                table: "Municipalities",
                column: "MicroRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesoregions_StateId",
                table: "Mesoregions",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_MunicipalityId",
                table: "Districts",
                column: "MunicipalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Districts_Municipalities_MunicipalityId",
                table: "Districts",
                column: "MunicipalityId",
                principalTable: "Municipalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesoregions_States_StateId",
                table: "Mesoregions",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Municipalities_MicroRegions_MicroRegionId",
                table: "Municipalities",
                column: "MicroRegionId",
                principalTable: "MicroRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_States_Regions_RegionId",
                table: "States",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Districts_Municipalities_MunicipalityId",
                table: "Districts");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesoregions_States_StateId",
                table: "Mesoregions");

            migrationBuilder.DropForeignKey(
                name: "FK_Municipalities_MicroRegions_MicroRegionId",
                table: "Municipalities");

            migrationBuilder.DropForeignKey(
                name: "FK_States_Regions_RegionId",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_States_RegionId",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_Municipalities_MicroRegionId",
                table: "Municipalities");

            migrationBuilder.DropIndex(
                name: "IX_Mesoregions_StateId",
                table: "Mesoregions");

            migrationBuilder.DropIndex(
                name: "IX_Districts_MunicipalityId",
                table: "Districts");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SubDistricts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "States",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "States",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Regions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "MicroRegionId",
                table: "Municipalities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Municipalities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "MesoregionId",
                table: "MicroRegions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MicroRegions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "StateId",
                table: "Mesoregions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Mesoregions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "MunicipalityId",
                table: "Districts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Districts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
