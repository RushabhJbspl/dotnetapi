using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class Limit31102018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeSellerList_TrnNo",
                table: "TradeSellerList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeSellerList",
                table: "TradeSellerList");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeBuyerList_PoolID_TrnNo",
                table: "TradeBuyerList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList");

            //migrationBuilder.DropColumn(
            //    name: "EndTime",
            //    table: "WalletLimitConfiguration");

            //migrationBuilder.DropColumn(
            //    name: "StartTime",
            //    table: "WalletLimitConfiguration");

            migrationBuilder.RenameColumn(
                name: "BuyReqID",
                table: "TradeSellerList",
                newName: "SellServiceID");

            migrationBuilder.RenameColumn(
                name: "PoolID",
                table: "TradeBuyerList",
                newName: "ServiceID");

            //migrationBuilder.AlterColumn<double>(
            //    name: "StartTime",
            //    table: "WalletLimitConfigurationMaster",
            //    nullable: false,
            //    oldClrType: typeof(TimeSpan));

            //migrationBuilder.AlterColumn<double>(
            //    name: "EndTime",
            //    table: "WalletLimitConfigurationMaster",
            //    nullable: false,
            //    oldClrType: typeof(TimeSpan));

            migrationBuilder.AddColumn<double>(
                name: "EndTimeUnix",
                table: "WalletLimitConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StartTimeUnix",
                table: "WalletLimitConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PoolID",
                table: "TradeSellerList",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BuyServiceID",
                table: "TradeSellerList",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BuyReqID",
                table: "TradeBuyerList",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PaidServiceID",
                table: "TradeBuyerList",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeSellerList_PoolID_TrnNo",
                table: "TradeSellerList",
                columns: new[] { "PoolID", "TrnNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeSellerList",
                table: "TradeSellerList",
                columns: new[] { "TrnNo", "PoolID" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeBuyerList_TrnNo",
                table: "TradeBuyerList",
                column: "TrnNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList",
                columns: new[] { "Id", "TrnNo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeSellerList_PoolID_TrnNo",
                table: "TradeSellerList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeSellerList",
                table: "TradeSellerList");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradeBuyerList_TrnNo",
                table: "TradeBuyerList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList");

            migrationBuilder.DropColumn(
                name: "EndTimeUnix",
                table: "WalletLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "StartTimeUnix",
                table: "WalletLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "PoolID",
                table: "TradeSellerList");

            migrationBuilder.DropColumn(
                name: "BuyServiceID",
                table: "TradeSellerList");

            migrationBuilder.DropColumn(
                name: "BuyReqID",
                table: "TradeBuyerList");

            migrationBuilder.DropColumn(
                name: "PaidServiceID",
                table: "TradeBuyerList");

            migrationBuilder.RenameColumn(
                name: "SellServiceID",
                table: "TradeSellerList",
                newName: "BuyReqID");

            migrationBuilder.RenameColumn(
                name: "ServiceID",
                table: "TradeBuyerList",
                newName: "PoolID");

            //migrationBuilder.AlterColumn<TimeSpan>(
            //    name: "StartTime",
            //    table: "WalletLimitConfigurationMaster",
            //    nullable: false,
            //    oldClrType: typeof(double));

            //migrationBuilder.AlterColumn<TimeSpan>(
            //    name: "EndTime",
            //    table: "WalletLimitConfigurationMaster",
            //    nullable: false,
            //    oldClrType: typeof(double));

            //migrationBuilder.AddColumn<TimeSpan>(
            //    name: "EndTime",
            //    table: "WalletLimitConfiguration",
            //    nullable: true);

            //migrationBuilder.AddColumn<TimeSpan>(
            //    name: "StartTime",
            //    table: "WalletLimitConfiguration",
            //    nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeSellerList_TrnNo",
                table: "TradeSellerList",
                column: "TrnNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeSellerList",
                table: "TradeSellerList",
                columns: new[] { "Id", "TrnNo" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradeBuyerList_PoolID_TrnNo",
                table: "TradeBuyerList",
                columns: new[] { "PoolID", "TrnNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeBuyerList",
                table: "TradeBuyerList",
                columns: new[] { "TrnNo", "PoolID" });
        }
    }
}
