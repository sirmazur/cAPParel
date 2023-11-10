using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cAPParel.API.Migrations
{
    public partial class addPieceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Piece_Items_ItemId",
                table: "Piece");

            migrationBuilder.DropForeignKey(
                name: "FK_Piece_Orders_OrderId",
                table: "Piece");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Piece",
                table: "Piece");

            migrationBuilder.RenameTable(
                name: "Piece",
                newName: "Pieces");

            migrationBuilder.RenameIndex(
                name: "IX_Piece_OrderId",
                table: "Pieces",
                newName: "IX_Pieces_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Piece_ItemId",
                table: "Pieces",
                newName: "IX_Pieces_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pieces",
                table: "Pieces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pieces_Items_ItemId",
                table: "Pieces",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pieces_Orders_OrderId",
                table: "Pieces",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pieces_Items_ItemId",
                table: "Pieces");

            migrationBuilder.DropForeignKey(
                name: "FK_Pieces_Orders_OrderId",
                table: "Pieces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pieces",
                table: "Pieces");

            migrationBuilder.RenameTable(
                name: "Pieces",
                newName: "Piece");

            migrationBuilder.RenameIndex(
                name: "IX_Pieces_OrderId",
                table: "Piece",
                newName: "IX_Piece_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Pieces_ItemId",
                table: "Piece",
                newName: "IX_Piece_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Piece",
                table: "Piece",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Piece_Items_ItemId",
                table: "Piece",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Piece_Orders_OrderId",
                table: "Piece",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
