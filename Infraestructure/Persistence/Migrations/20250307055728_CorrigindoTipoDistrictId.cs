using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindoTipoDistrictId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                         migrationBuilder.AddColumn<int>(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                type: "integer",
                nullable: true);
        
                         migrationBuilder.Sql("UPDATE \"SubDistricts\" SET \"DistrictIdTemp\" = \"DistrictId\"::integer WHERE \"DistrictId\" ~ '^\\d+$'");
    
                         migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "SubDistricts");
        
                         migrationBuilder.RenameColumn(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                newName: "DistrictId");
        
                         migrationBuilder.AlterColumn<int>(
                name: "DistrictId",
                table: "SubDistricts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                         migrationBuilder.AddColumn<string>(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                type: "text",
                nullable: true);
        
                         migrationBuilder.Sql("UPDATE \"SubDistricts\" SET \"DistrictIdTemp\" = \"DistrictId\"::text");
    
                         migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "SubDistricts");
        
                         migrationBuilder.RenameColumn(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                newName: "DistrictId");
        
                         migrationBuilder.AlterColumn<string>(
                name: "DistrictId",
                table: "SubDistricts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
