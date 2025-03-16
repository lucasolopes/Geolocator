using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SubDistricts_DistrictId",
                table: "SubDistricts",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_MicroRegions_MesoregionId",
                table: "MicroRegions",
                column: "MesoregionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MicroRegions_Mesoregions_MesoregionId",
                table: "MicroRegions",
                column: "MesoregionId",
                principalTable: "Mesoregions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubDistricts_Districts_DistrictId",
                table: "SubDistricts",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MicroRegions_Mesoregions_MesoregionId",
                table: "MicroRegions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubDistricts_Districts_DistrictId",
                table: "SubDistricts");

            migrationBuilder.DropIndex(
                name: "IX_SubDistricts_DistrictId",
                table: "SubDistricts");

            migrationBuilder.DropIndex(
                name: "IX_MicroRegions_MesoregionId",
                table: "MicroRegions");
        }
    }
}
