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
            // Primeiro crie uma coluna temporária
            migrationBuilder.AddColumn<int>(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                type: "integer",
                nullable: true);
        
            // Popule a coluna temporária com os valores convertidos
            migrationBuilder.Sql("UPDATE \"SubDistricts\" SET \"DistrictIdTemp\" = \"DistrictId\"::integer WHERE \"DistrictId\" ~ '^\\d+$'");
    
            // Remova a coluna antiga
            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "SubDistricts");
        
            // Renomeie a coluna temporária para DistrictId
            migrationBuilder.RenameColumn(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                newName: "DistrictId");
        
            // Faça a coluna não ser nula
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
            // Adicione uma coluna temporária do tipo text
            migrationBuilder.AddColumn<string>(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                type: "text",
                nullable: true);
        
            // Converta os valores inteiros para string
            migrationBuilder.Sql("UPDATE \"SubDistricts\" SET \"DistrictIdTemp\" = \"DistrictId\"::text");
    
            // Remova a coluna antiga
            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "SubDistricts");
        
            // Renomeie a coluna temporária para DistrictId
            migrationBuilder.RenameColumn(
                name: "DistrictIdTemp",
                table: "SubDistricts",
                newName: "DistrictId");
        
            // Configure a coluna DistrictId como não nula
            migrationBuilder.AlterColumn<string>(
                name: "DistrictId",
                table: "SubDistricts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
