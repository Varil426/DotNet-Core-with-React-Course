using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Persistence.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Values",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("MySQL:ValueGenerationStratedy", MySQLValueGenerationStrategy.IdentityColumn)
						.Annotation("SqlServer:ValueGenerationStratedy", SqlServerValueGenerationStrategy.IdentityColumn)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Values", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Values");
		}
	}
}
