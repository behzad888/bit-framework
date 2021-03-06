﻿using Bit.Data.Implementations;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    public static class MigrationBuilderExtensions
    {
        /// <summary>
        /// <seealso cref="SqlServerJsonLogStore"/>
        /// </summary>
        public static void CreateSqlServerJsonLogStoreTable(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Contents = table.Column<string>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        public static void CreateHangfireSqlObjects(this MigrationBuilder migrationBuilder)
        {
            using (Stream hangfireJobsDatabaseStream = Assembly.Load("Bit.Hangfire").GetManifestResourceStream("Bit.Hangfire.Hangfire-Database-Script.sql"))
            {
                using (StreamReader reader = new StreamReader(hangfireJobsDatabaseStream))
                {
                    string sql = reader.ReadToEnd();
                    migrationBuilder.Sql(sql);
                }
            }
        }
    }
}
